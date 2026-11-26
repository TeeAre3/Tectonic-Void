using UnityEngine;
using UnityEngine.VFX;

[RequireComponent (typeof(Rigidbody), typeof(Health), typeof(PlatformRider))]
public class VoidRespawn : MonoBehaviour, IInvulnerabilityProvider
{
    [Header("Fall")]
    [SerializeField] private float fallDamage = 20f;
    [SerializeField] private float respawnInvulnerability = 1f;

    [Header("Safe position")]
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private float sampleInterval = 0.2f;
    [SerializeField] private float edgeMargin = 0.7f;

    private Rigidbody rb;
    private Health health;
    private PlatformRider rider;
    private PlayerDash dash;

    private Platform safePlatform;
    private Vector3 safeLocalPosition;
    private Vector3 fallbackPosition;
    private bool hasSafePosition;
    private float nextSampleTime;
    private float nextFallAllowed;
    private float invulnerableUntil;

    public bool IsInvulnerable => Time.time < invulnerableUntil;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
        rider = GetComponent<PlatformRider>();
        dash = GetComponent<PlayerDash>();
        fallbackPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (Time.time < nextSampleTime) return;
        nextSampleTime = Time.time + sampleInterval;

        Platform platform = rider.CurrentPlatform;
        if(platform == null) return;
        if (dash != null && dash.IsDashing) return;
        if (Mathf.Abs(rb.linearVelocity.y) > 0.1f) return;
        if (!IsFullySupported()) return;

        safePlatform = platform;
        safeLocalPosition = platform.transform.InverseTransformPoint(transform.position);
        hasSafePosition = true;
    }

    private bool IsFullySupported()
    {
        Vector3 p = transform.position;
        return GroundProbe.HasGround(p, platformMask)
            && GroundProbe.HasGround(p + Vector3.right * edgeMargin, platformMask)
            && GroundProbe.HasGround(p + Vector3.left * edgeMargin, platformMask)
            && GroundProbe.HasGround(p + Vector3.forward * edgeMargin, platformMask)
            && GroundProbe.HasGround(p + Vector3.back * edgeMargin, platformMask);
    }

    public void HandleFall()
    {
        if(Time.time < nextFallAllowed) return;
        nextFallAllowed = Time.time + 0.5f;

        health.TakeDamage(new DamageInfo(fallDamage, Vector3.zero, 0f, null, true));
        if (health.IsDead) return;

        Vector3 target = (hasSafePosition && safePlatform != null)
            ? safePlatform.transform.TransformPoint(safeLocalPosition)
            : fallbackPosition;

        rb.linearVelocity = Vector3.zero;
        rb.position = target;
        transform.position = target;
        invulnerableUntil = Time.time + respawnInvulnerability;

        Debug.Log($"Fell into the void: -{fallDamage} HP, respawn at {target}"); // tymczasowo
    }
}

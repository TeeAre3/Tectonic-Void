using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerMovement))]
public class PlayerDash : MonoBehaviour
{
    [Header("Dash")]
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.18f;
    [SerializeField] private float dashCooldown = 0.6f;
    [SerializeField] private float exitSpeed = 8f;

    [Header("Invulnerability")]
    [SerializeField] private float iFrameDuration = 0.22f;

    [Header("Input feel")]
    [SerializeField] private float inputBuffer = 0.12f;

    [Header("Debug (temporary)")]
    [SerializeField] private bool tintDuringIFrames = true;

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private Rigidbody rb;
    private PlayerMovement movement;
    private PlayerControls controls;

    private bool isDashing;
    private float dashEndTime;
    private float nextDashTime;
    private float invulnerableUntil;
    private float dashRequestedUntil = -1f;
    private Vector3 dashDirection;

    private Renderer[] renderers;
    private Color[] originalColors;
    private bool tinted;

    public bool IsDashing => isDashing;
    public bool IsInvulnerable => Time.time < invulnerableUntil;

    private float DashSpeed => dashDistance / dashDuration;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        controls = new PlayerControls();

        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            Material m = renderers[i].material;
            originalColors[i] = (m != null && m.HasProperty(BaseColorId))
                ? m.GetColor(BaseColorId)
                : Color.white;
        }
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Dash.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.Dash.performed -= OnDashPerformed;
            controls.Player.Disable();
        }
        if (isDashing) EndDash();
    }

    private void OnDestroy()
    {
        if (controls != null) controls.Dispose();
    }

    private void OnDashPerformed(InputAction.CallbackContext ctx)
    {
        dashRequestedUntil = Time.time + inputBuffer;
    }

    private void Update()
    {
        if (!tintDuringIFrames || renderers == null) return;

        bool invulnerable = IsInvulnerable;
        if (invulnerable == tinted) return;
        tinted = invulnerable;

        for (int i = 0; i < renderers.Length; i++)
        {
            Material m = renderers[i].material;
            if (m == null || !m.HasProperty(BaseColorId)) continue;
            m.SetColor(BaseColorId, invulnerable ? Color.cyan : originalColors[i]);
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            if (Time.time >= dashEndTime) EndDash();
            else rb.linearVelocity = dashDirection * DashSpeed;
            return;
        }

        if (Time.time <= dashRequestedUntil && Time.time >= nextDashTime)
            StartDash();
    }

    private void StartDash()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        dashDirection = input.sqrMagnitude > 0.01f
            ? new Vector3(input.x, 0f, input.y).normalized
            : transform.forward;

        isDashing = true;
        dashRequestedUntil = -1f;
        dashEndTime = Time.time + dashDuration;
        nextDashTime = Time.time + dashCooldown;
        invulnerableUntil = Time.time + iFrameDuration;

        movement.MovementLocked = true;
        rb.useGravity = false;
        rb.rotation = Quaternion.LookRotation(dashDirection);
        rb.linearVelocity = dashDirection * DashSpeed;
    }

    private void EndDash()
    {
        isDashing = false;
        movement.MovementLocked = false;
        rb.useGravity = true;
        rb.linearVelocity = dashDirection * exitSpeed;
    }
}
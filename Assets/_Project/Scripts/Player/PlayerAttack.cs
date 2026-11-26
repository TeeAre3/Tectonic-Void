using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerMovement), typeof(PlayerDash))]
public class PlayerAttack : MonoBehaviour
{
    private enum Phase { Idle, Windup, Active, Recovery }
    private enum AttackType { None, Light, Heavy }

    [Header("Attacks")]
    [SerializeField] private AttackData[] lightCombo;
    [SerializeField] private AttackData heavyAttack;
    [SerializeField] private float comboResetTime = 0.5f;
    [SerializeField] private float inputBuffer = 0.2f;

    [Header("Hit detection")]
    [SerializeField] private LayerMask hitMask = ~0;

    [Header("Aim assist")]
    [SerializeField] private bool assistEnabled = true;
    [SerializeField] private float assistRange = 3f;
    [SerializeField] private float assistHalfAngle = 60f;

    private Rigidbody rb;
    private PlayerMovement movement;
    private PlayerDash dash;
    private PlayerControls controls;

    private Phase phase = Phase.Idle;
    private AttackData current;
    private int lastIndex = -1;
    private float phaseEndTime;
    private float lastAttackEndTime = -999f;
    private AttackType buffered = AttackType.None;
    private float bufferedUntil = -1f;
    private readonly HashSet<Health> hitThisSwing = new HashSet<Health>();

    public bool IsAttacking => phase != Phase.Idle;
    private bool HasBuffered => buffered != AttackType.None && Time.time <= bufferedUntil;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<PlayerMovement>();
        dash = GetComponent<PlayerDash>();
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.LightAttack.performed += OnLightAttack;
        controls.Player.HeavyAttack.performed += OnHeavyAttack;
    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Player.LightAttack.performed -= OnLightAttack;
            controls.Player.HeavyAttack.performed -= OnHeavyAttack;
            controls.Player.Disable();
        }
        if (IsAttacking) CancelAttack();
    }

    private void OnDestroy()
    {
        if (controls != null) controls.Dispose();
    }

    private void OnLightAttack(InputAction.CallbackContext ctx) => Buffer(AttackType.Light);
    private void OnHeavyAttack(InputAction.CallbackContext ctx) => Buffer(AttackType.Heavy);

    private void Buffer(AttackType type)
    {
        buffered = type;
        bufferedUntil = Time.time + inputBuffer;
    }

    private void ClearBuffer()
    {
        buffered = AttackType.None;
        bufferedUntil = -1f;
    }

    private void FixedUpdate()
    {
        if (dash.IsDashing)
        {
            if (IsAttacking) CancelAttack();
            ClearBuffer();
            return;
        }

        switch (phase)
        {
            case Phase.Idle:
                if (HasBuffered) StartAttack(buffered);
                break;

            case Phase.Windup:
                if (Time.time >= phaseEndTime) EnterPhase(Phase.Active, current.active);
                break;

            case Phase.Active:
                DealDamage();
                if (Time.time >= phaseEndTime) EnterPhase(Phase.Recovery, current.recovery);
                break;

            case Phase.Recovery:
                if (Time.time >= phaseEndTime) EndAttack();
                break;
        }
    }

    private void StartAttack(AttackType type)
    {
        AttackData data;

        if (type == AttackType.Heavy)
        {
            if (heavyAttack == null) { ClearBuffer(); return; }
            data = heavyAttack;
            lastIndex = -1;
        }
        else
        {
            if (lightCombo == null || lightCombo.Length == 0) { ClearBuffer(); return; }

            bool inCombo = lastIndex >= 0 && Time.time - lastAttackEndTime <= comboResetTime;
            lastIndex = inCombo ? (lastIndex + 1) % lightCombo.Length : 0;
            data = lightCombo[lastIndex];
        }

        current = data;
        ClearBuffer();
        hitThisSwing.Clear();

        movement.SpeedMultiplier = current.moveSpeedMultiplier;
        movement.RotationLocked = true;

        AimAtTarget();

        Debug.Log(type == AttackType.Heavy ? "Heavy attack" : $"Light attack {lastIndex + 1}"); // tymczasowo
        EnterPhase(Phase.Windup, current.windup);
    }

    private void EnterPhase(Phase next, float duration)
    {
        phase = next;
        phaseEndTime = Time.time + duration;
    }

    private void EndAttack()
    {
        ResetState();
        lastAttackEndTime = Time.time;
        if (HasBuffered) StartAttack(buffered);
    }

    private void CancelAttack()
    {
        ResetState();
        lastIndex = -1;
        lastAttackEndTime = -999f;
    }

    private void ResetState()
    {
        phase = Phase.Idle;
        current = null;
        movement.SpeedMultiplier = 1f;
        movement.RotationLocked = false;
    }

    private void AimAtTarget()
    {
        Vector2 input = controls.Player.Move.ReadValue<Vector2>();
        Vector3 aimDir = input.sqrMagnitude > 0.01f
            ? new Vector3(input.x, 0f, input.y).normalized
            : transform.forward;

        Vector3 faceDir = aimDir;
        if (assistEnabled && TryFindAssistTarget(aimDir, out Vector3 toTarget))
            faceDir = toTarget;

        Quaternion look = Quaternion.LookRotation(faceDir);
        rb.rotation = look;
        transform.rotation = look;
    }

    private bool TryFindAssistTarget(Vector3 aimDir, out Vector3 direction)
    {
        direction = aimDir;
        Collider[] cols = Physics.OverlapSphere(transform.position, assistRange,
                                                hitMask, QueryTriggerInteraction.Ignore);

        float bestScore = float.MaxValue;
        bool found = false;

        foreach (Collider col in cols)
        {
            Health health = col.GetComponentInParent<Health>();
            if (health == null || health.IsDead || health.gameObject == gameObject) continue;

            Vector3 to = health.transform.position - transform.position;
            to.y = 0f;
            float dist = to.magnitude;
            if (dist < 0.01f) continue;
            to /= dist;

            float angle = Vector3.Angle(aimDir, to);
            if (angle > assistHalfAngle) continue;

            float score = (angle / assistHalfAngle) * 0.6f + (dist / assistRange) * 0.4f;
            if (score < bestScore)
            {
                bestScore = score;
                direction = to;
                found = true;
            }
        }

        return found;
    }

    private void DealDamage()
    {
        Vector3 forward = transform.forward;
        Vector3 center = transform.position + forward * (current.range * 0.5f);
        Vector3 halfExtents = new Vector3(current.width * 0.5f, current.height * 0.5f, current.range * 0.5f);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, transform.rotation,
                                             hitMask, QueryTriggerInteraction.Ignore);

        foreach (Collider col in hits)
        {
            Health health = col.GetComponentInParent<Health>();
            if (health == null || health.gameObject == gameObject) continue;
            if (!hitThisSwing.Add(health)) continue;

            Vector3 dir = health.transform.position - transform.position;
            dir.y = 0f;
            dir = dir.sqrMagnitude > 0.001f ? dir.normalized : forward;

            health.TakeDamage(new DamageInfo(current.damage, dir, current.knockbackForce, gameObject));
        }
    }

    private void OnDrawGizmos()
    {
        if (current == null || phase == Phase.Idle) return;

        Gizmos.color = phase == Phase.Active ? Color.red
                     : phase == Phase.Windup ? Color.yellow
                     : Color.gray;
        Gizmos.matrix = Matrix4x4.TRS(
            transform.position + transform.forward * (current.range * 0.5f),
            transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(current.width, current.height, current.range));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, assistRange);
    }
}
using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 80f;

    [Header("Rotation")]
    [SerializeField] private float turnSpeed = 720f;

    private Rigidbody rb;
    private PlayerControls controls;
    private Vector2 moveInput;
    private PlatformRider rider;
    private Vector3 lastPlatformVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();
        rider = GetComponent<PlatformRider>();
    }

    private void OnEnable() => controls.Player.Enable();
    private void OnDisable () => controls.Player.Disable();
    private void OnDestroy() => controls.Dispose();

    private void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>(); 
    }

    public bool MovementLocked { get; set; }
    public bool RotationLocked { get; set; }
    public float SpeedMultiplier { get; set; } = 1f;
    private void FixedUpdate()
    {
        if (MovementLocked)
        {
            lastPlatformVelocity = Vector3.zero;
            return;
        }

        Vector3 platformVelocity = rider != null ? rider.PlatformVelocity : Vector3.zero;
        platformVelocity.y = 0f;

        Vector3 velocity = rb.linearVelocity;

        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z) - lastPlatformVelocity;
        Vector3 desired = new Vector3(moveInput.x, 0f, moveInput.y) * maxSpeed * SpeedMultiplier;

        bool hasInput = moveInput.sqrMagnitude > 0.01f;
        float rate = hasInput ? acceleration : deceleration;
        horizontal = Vector3.MoveTowards(horizontal, desired, rate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(horizontal.x + platformVelocity.x, velocity.y,
                                        horizontal.z + platformVelocity.z);
        lastPlatformVelocity = platformVelocity;
        rb.angularVelocity = Vector3.zero;

        if (hasInput && !RotationLocked)
        {
            Quaternion target = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.y));
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, target, turnSpeed * Time.fixedDeltaTime));
        }
    }
}

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerControls();
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
        if (MovementLocked) return;

        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontal = new Vector3(velocity.x, 0f, velocity.z);
        Vector3 desired = new Vector3(moveInput.x, 0f, moveInput.y) * maxSpeed * SpeedMultiplier;

        bool hasInput = moveInput.sqrMagnitude > 0.01f;
        float rate = hasInput ? acceleration : deceleration;
        horizontal = Vector3.MoveTowards(horizontal, desired, rate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(horizontal.x, velocity.y, horizontal.z);
        rb.angularVelocity = Vector3.zero;

        if(hasInput && RotationLocked)
        {
            Quaternion target = Quaternion.LookRotation(new Vector3(moveInput.x, 0f, moveInput.y));
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, target, turnSpeed * Time.fixedDeltaTime));
        }
    }
}

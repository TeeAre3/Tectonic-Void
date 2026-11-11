using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlatformRider))]
public class PlatformControl : MonoBehaviour
{
    [SerializeField] private InputActionReference platformMoveAction;

    private PlatformRider rider;
    private Platform heldPlatform;

    private void Awake()
    {
        rider = GetComponent<PlatformRider>();
    }

    private void OnEnable()
    {
        if (platformMoveAction != null)
        {
            platformMoveAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        ReleaseHeldPlatform();

        if(platformMoveAction != null)
        {
            platformMoveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (platformMoveAction == null) return;

        Vector2 input = platformMoveAction.action.ReadValue<Vector2>();

        if (input == Vector2.zero)
        {
            ReleaseHeldPlatform();
            return;
        }

        Platform current = rider.CurrentPlatform;

        if (heldPlatform == null || (!heldPlatform.IsMoving && current != null && current != heldPlatform))
        {
            ReleaseHeldPlatform();
            heldPlatform = current;
        }

        if (heldPlatform == null) return;

        heldPlatform.SetHeldInput(input);
    }

    private void ReleaseHeldPlatform()
    {
        if (heldPlatform == null) return;

        heldPlatform.ReleaseHold();
        heldPlatform = null;
    }
}

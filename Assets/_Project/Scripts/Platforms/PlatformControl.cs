using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlatformRider))]
public class PlatformControl : MonoBehaviour
{
    [SerializeField] private InputActionReference platformMoveAction;

    private PlatformRider rider;
    private Vector2 previousInput;

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
        if(platformMoveAction != null)
        {
            platformMoveAction.action.Disable();
        }
    }

    private void Update()
    {
        if (platformMoveAction == null) return;

        Vector2 input = platformMoveAction.action.ReadValue<Vector2>();

        bool isNewPress = previousInput == Vector2.zero && input != Vector2.zero;
        previousInput = input;

        if (!isNewPress) return;
        if (rider.CurrentPlatform == null) return;

        rider.CurrentPlatform.TryPush(input);
    }
}

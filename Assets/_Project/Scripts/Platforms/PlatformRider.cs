using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlatformRider : MonoBehaviour
{
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float checkDistance = 0.4f;


    private Collider ownCollider;
    private CharacterController characterController;
    private Rigidbody body;

    public Platform CurrentPlatform { get; private set; }

    private void Awake()
    {
        ownCollider = GetComponent<Collider>();
        characterController = GetComponent<CharacterController>();
        body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        DetectPlatform();
    }

    private void DetectPlatform()
    {
        Bounds bounds = ownCollider.bounds;
        float rayLength = bounds.extents.y + checkDistance;

        Platform detected = null;

        if(Physics.Raycast(bounds.center, Vector3.down, out RaycastHit hit, rayLength, platformLayer, QueryTriggerInteraction.Ignore))
        {
            detected = hit.collider.GetComponent<Platform>();
        }

        if (detected == CurrentPlatform) return;

        if(CurrentPlatform != null)
        {
            CurrentPlatform.RemoveRider(this);
        }

        CurrentPlatform = detected;

        if(CurrentPlatform != null)
        {
            CurrentPlatform.AddRider(this);
        }
    }

    public void Carry(Vector3 delta)
    {
        if(characterController != null)
        {
            characterController.Move(delta);
        }
        else if(body != null)
        {
            body.position += delta;
        }
        else
        {
            transform.position += delta;
        }
    }

    private void OnDisable()
    {
        if(CurrentPlatform != null)
        {
            CurrentPlatform.RemoveRider(this);
            CurrentPlatform = null;
        }
    }
}

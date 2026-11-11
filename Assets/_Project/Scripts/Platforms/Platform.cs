using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class Platform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float slideSpeed = 20f;

    [Header("Collision")]
    [Tooltip("Small shrink of the cast box so platforms touching edge to edge do not count as overlapping.")]
    [SerializeField] private float skin = 0.02f;

    private readonly HashSet<PlatformRider> riders = new HashSet<PlatformRider>();

    private BoxCollider boxCollider;
    private Vector3 targetPosition;
    private Vector2 heldInput;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        Rigidbody body = GetComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        boxCollider = GetComponent<BoxCollider>();
        targetPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (!IsMoving) return;

        Vector3 previousPosition = transform.position;
        Vector3 newPosition = Vector3.MoveTowards(previousPosition, targetPosition, slideSpeed * Time.fixedDeltaTime);
        Vector3 delta = newPosition - previousPosition;

        transform.position = newPosition;

        Physics.SyncTransforms();

        foreach (PlatformRider rider in riders)
        {
            rider.Carry(delta);
        }

        if ((targetPosition - newPosition).sqrMagnitude < 0.0001f)
        {
            transform.position = targetPosition;
            IsMoving = false;

            if(heldInput != Vector2.zero)
            {
                TryPush(heldInput);
            }
        }
    }

    public void SetHeldInput(Vector2 input)
    {
        heldInput = input;

        if(!IsMoving && input != Vector2.zero)
        {
            TryPush(input);
        }
    }

    public void ReleaseHold()
    {
        heldInput = Vector2.zero;
    }

    public bool TryPush(Vector2 direction)
    {
        if (IsMoving) return false;
        if (direction == Vector2.zero) return false;

        Vector3 dir = Mathf.Abs(direction.x) >= Mathf.Abs(direction.y)
            ? new Vector3(Mathf.Sign(direction.x), 0f, 0f)
            : new Vector3(0f, 0f, Mathf.Sign(direction.y));

        float distance = GetAllowedDistance(dir);
        if (distance <= 0.001f) return false;

        targetPosition = transform.position + dir * distance;
        IsMoving = true;
        return true;
    }

    private float GetAllowedDistance(Vector3 dir)
    {
        Vector3 size = Vector3.Scale(boxCollider.size, transform.lossyScale);
        Vector3 center = transform.TransformPoint(boxCollider.center);

        float stepLength = Mathf.Abs(dir.x) > 0f ? size.x : size.z;

        Vector3 halfExtents = size * 0.5f - Vector3.one * skin;
        int mask = 1 << gameObject.layer;

        RaycastHit[] hits = Physics.BoxCastAll(
            center,
            halfExtents,
            dir,
            transform.rotation,
            stepLength,
            mask,
            QueryTriggerInteraction.Ignore);

        float allowed = stepLength;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == boxCollider) continue;

            float distanceToContact = hit.distance - skin;
            if(distanceToContact < allowed)
            {
                allowed = Mathf.Max(0f, distanceToContact);
            }
        }

        return allowed;
    }

    public void AddRider(PlatformRider rider)
    {
        riders.Add(rider);
    }

    public void RemoveRider(PlatformRider rider)
    {
        riders.Remove(rider);
    }
}

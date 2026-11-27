using UnityEngine;

public class EdgeGuard : MonoBehaviour
{
    [SerializeField] private LayerMask platformMask;
    [SerializeField] private float probeDistance = 0.6f;

    public Vector3 Constrain(Vector3 horizontalVelocity)
    {
        if (!enabled) return horizontalVelocity;

        Vector3 v = horizontalVelocity;
        Vector3 pos = transform.position;

        bool movesX = Mathf.Abs(v.x) > 0.01f;
        bool movesZ = Mathf.Abs(v.z) > 0.01f;
        float sx = Mathf.Sign(v.x);
        float sz = Mathf.Sign(v.z);

        if (movesX && !HasGroundAt(pos + new Vector3(sx * probeDistance, 0f, 0f)))
        {
            v.x = 0f;
            movesX = false;
        }

        if (movesZ && !HasGroundAt(pos + new Vector3(0f, 0f, sz * probeDistance)))
        {
            v.z = 0f;
            movesZ = false;
        }

        if (movesX && movesZ)
        {
            Vector3 diagonal = new Vector3(sx, 0f, sz).normalized * probeDistance;
            if (!HasGroundAt(pos + diagonal))
            {
                if (Mathf.Abs(v.x) >= Mathf.Abs(v.z)) v.z = 0f;
                else v.x = 0f;
            }
        }

        return v;
    }

    private bool HasGroundAt(Vector3 point)
    {
        return GroundProbe.HasGround(point, platformMask);
    }
}

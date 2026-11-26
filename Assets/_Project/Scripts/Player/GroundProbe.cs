using UnityEngine;

public static class GroundProbe
{
    public static bool HasGround(Vector3 origin, LayerMask mask, 
                                float radius = 0.15f, float distance = 1.6f)
    {
        return Physics.SphereCast(origin, radius, Vector3.down, out _, distance, mask, 
                                QueryTriggerInteraction.Ignore);
    }
}

using UnityEngine;

[RequireComponent (typeof(Collider))]
public class VoidZone : MonoBehaviour
{
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponentInParent<Health>();
        if (health == null) return;
        
        VoidRespawn respawn = health.GetComponent<VoidRespawn>();
        if (respawn != null) respawn.HandleFall();
        else health.InstaDeath();
    }
}

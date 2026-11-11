using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DamageZone : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float tickInterval = 0.5f;
    [SerializeField] private float knockbackForce = 0f;

    private readonly Dictionary<Health, float> nextHitTime = new Dictionary<Health, float>();

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        Health health = other.GetComponentInParent<Health>();
        if (health == null) return;

        if (nextHitTime.TryGetValue(health, out float t) && Time.time < t) return;

        Vector3 dir = other.transform.position - transform.position;
        dir.y = 0f;
        dir = dir.sqrMagnitude > 0.001f ? dir.normalized : Vector3.forward;

        if (health.TakeDamage(new DamageInfo(damage, dir, knockbackForce, gameObject)))
            nextHitTime[health] = Time.time + tickInterval;
    }
}

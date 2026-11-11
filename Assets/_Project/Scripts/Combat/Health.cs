using System;
using UnityEngine;

public struct DamageInfo
{
    public float Ammount;
    public Vector3 HitDirection;
    public float KnockbackForce;
    public GameObject Source;

    public DamageInfo(float ammount, Vector3 hitDirection = default, float knockbackForce = 0f, GameObject source = null)
    {
        Ammount = ammount;
        HitDirection = hitDirection;
        KnockbackForce = knockbackForce;
        Source = source;
    }
}

public interface IInvulnerabilityProvider
{
    bool IsInvulnerable { get; }
}

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float iFramesAfterHit = 0f;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private bool logChanges = true;

    private float currentHealth;
    private float hitInvulnerableUntil;
    private IInvulnerabilityProvider[] providers;

    public float Current => currentHealth;
    public float Max => maxHealth;
    public bool IsDead {  get; private set; }

    public event Action<float, float> Changed; //aktualne, maksymalne
    public event Action<DamageInfo> Damaged;
    public event Action Died;

    public bool IsInvulnerable
    {
        get
        {
            if (Time.time < hitInvulnerableUntil) return true;
            for (int i = 0; i < providers.Length; i++)
                if (providers[i].IsInvulnerable) return true;
            return false;
        }
    }

    private void Awake()
    {
        currentHealth = maxHealth;
        providers = GetComponents<IInvulnerabilityProvider>();
    }

    public bool TakeDamage(DamageInfo info)
    {
        if (IsDead || info.Ammount <= 0f || IsInvulnerable) return false;

        currentHealth = Mathf.Max(0f, currentHealth - info.Ammount);
        hitInvulnerableUntil = Time.time + iFramesAfterHit;

        if (logChanges)
            Debug.Log($"{name}: -{info.Ammount} HP ({currentHealth}/{maxHealth})");

        Damaged?.Invoke(info);
        Changed?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0f) Die();
        return true;
    }

    private void Die()
    {
        IsDead = true;
        if (logChanges) Debug.Log($"{name} died");

        Died?.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
    }

}

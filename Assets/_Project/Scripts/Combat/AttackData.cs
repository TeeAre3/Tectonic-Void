using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "TectonicVoid/NewAttack")]
public class AttackData : ScriptableObject
{
    [Header("Damage")]
    public float damage = 10f;
    public float knockbackForce = 0f;

    [Header("Timing (seconds)")]
    public float windup = 0.08f;
    public float active = 0.10f;
    public float recovery = 0.18f;

    [Header("Hitbox")]
    public float range = 1.8f;
    public float width = 1.6f;
    public float height = 1.5f;

    [Header("Movement while attacking")]
    [Range(0f, 1f)] public float moveSpeedMultiplier = 0.25f;

}

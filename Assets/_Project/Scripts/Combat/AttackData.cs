using UnityEngine;

[CreateAssetMenu(fileName = "NewAttack", menuName = "TectonicVoid/NewAttack")]
public class AttackData : ScriptableObject
{
    [Header("Damage")]
    public float damage = 10f;
    public float knockbackForce = 0f;

}

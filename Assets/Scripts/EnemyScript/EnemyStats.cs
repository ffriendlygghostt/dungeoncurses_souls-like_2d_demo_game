using UnityEngine;

[System.Serializable]
public class EnemyStats : MonoBehaviour
{
    [Header("Core Stats")]
    public int maxHealth = 60;
    public int damage = 20;
    public float walkSpeed = 1.5f;
    public float runSpeed = 3f;

    [Range(0f, 100f)]
    public float critChance = 5f;

    [Tooltip("Multiplier as percentage. 200% = double damage")]
    public float critMultiplier = 200f;

    [Header("Universal")]
    public float dashSpeed = 5f;
}

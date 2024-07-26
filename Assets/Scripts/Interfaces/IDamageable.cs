using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 0f, float knockbackDuration = 0.12f); // For EnemyStats
    void TakeDamage(float damage); // For BreakableProps
}

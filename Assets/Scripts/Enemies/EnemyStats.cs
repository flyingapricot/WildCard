using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed on Enemy Prefabs

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;

    // Current Enemy Stats
    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentSpeed; // Accessed by movement
    [HideInInspector] public float currentHealth;

    void Awake()
    {
        currentDamage = enemyData.Damage;
        currentSpeed = enemyData.Speed;
        currentHealth = enemyData.MaxHealth;
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Dies
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Debug.Log("Collision detected with: " + collision.gameObject.name + ", Tag: " + collision.gameObject.tag);
        if (collision.gameObject.CompareTag("Player"))
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.TakeDamage(currentDamage); // Refer to PlayerStats.cs
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed on Enemy Prefabs

public class EnemyStats : MonoBehaviour
{
    public EnemyScriptableObject enemyData;
    EnemySpawner es;
    float despawnDistance = 50f; // Distance from player for the enemy to despawn
    float enemyDistance; // Distance of enemy from player

    // Current Enemy Stats
    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentSpeed; // Accessed by movement
    [HideInInspector] public float currentHealth;

    void Awake()
    {
        es = FindObjectOfType<EnemySpawner>();

        currentDamage = enemyData.Damage;
        currentSpeed = enemyData.Speed;
        currentHealth = enemyData.MaxHealth;
    }

    void Update()
    {
        enemyDistance = Vector3.Distance(transform.position, PlayerStats.instance.transform.position);
        if (enemyDistance > despawnDistance)
        {
            // Respawn the despawned enemy near the player 
            transform.position = PlayerStats.instance.transform.position + es.GenerateRandomPosition();
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
        {
            es.OnEnemyKilled();
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

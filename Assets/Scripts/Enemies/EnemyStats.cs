using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed on Enemy Prefabs

public class EnemyStats : MonoBehaviour, IDamageable
{
    public EnemyScriptableObject enemyData;
    EnemySpawner es;
    EnemyMovement movement;
    readonly float despawnDistance = 50f; // Distance from player for the enemy to despawn
    float enemyDistance; // Distance of enemy from player

    // Current Enemy Stats
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentSpeed; // Accessed by movement

    [Header("Damage Feedback")]
    public Color damageColor = new(1,0,0,1); // Color of damage flash
    float damageFlashDuration = 0.2f; // How long the flash would last
    float deathFadeTime = 0.2f; // How long it takes for enemy to fade
    SpriteRenderer sprite;
    Color originalColor;

    void Awake()
    {
        es = FindObjectOfType<EnemySpawner>();
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject

        currentDamage = enemyData.Damage;
        currentSpeed = enemyData.Speed;
        currentHealth = enemyData.MaxHealth;
    }

    void Start()
    {
        originalColor = sprite.color;
        movement = GetComponent<EnemyMovement>();
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

    // Optional parameters for knockback since weapon might not always have knockback
    // Knockback duration is always 120 milliseconds
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 0f, float knockbackDuration = 0.12f)
    {
        currentHealth -= dmg;
        StartCoroutine(DamageFlash());

        // Create Damage text popup
        if (dmg > 0)
        {
            GameManager.GenerateDamageText(Mathf.FloorToInt(dmg).ToString(), transform);
        }

        // Apply knockback if its not 0
        if (knockbackForce > 0)
        {
            // Get direction of knockback
            Vector2 dir = (Vector2)transform.position - sourcePosition;
            movement.Knockback(dir.normalized * knockbackForce, knockbackDuration);
        }

        if (currentHealth <= 0) // Dies
        {
            Kill();
        }
    }
    // Coroutine function that makes the enemy flash when taking damage
    IEnumerator DamageFlash() 
    {
        sprite.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sprite.color = originalColor;
    }

    void Kill()
    {
        es.OnEnemyKilled(); // Updates enemy count
        StartCoroutine(KillFade());
    }
    // Coroutine function that makes the enemy fade away when killed
    IEnumerator KillFade()
    {
        // Waits for a single frame
        WaitForEndOfFrame w = new();
        float t = 0, origAlpha = sprite.color.a;

        // This is a loop that fires every frame
        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            // Make the sprite more transparent
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, (1 - t / deathFadeTime) * origAlpha);
        }

        Destroy(gameObject);
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

    public void TakeDamage()
    {
        // This method is left empty, for IDamageable
    }
}

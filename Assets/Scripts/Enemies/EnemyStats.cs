using System.Collections;
using UnityEngine;

// To be placed on Enemy Prefabs

public class EnemyStats : MonoBehaviour, IDamageable
{
    EnemyMovement movement;
    //float despawnDistance = 50f; // Distance from player for the enemy to despawn
    float enemyDistance; // Distance of enemy from player
    public static int count; // Track the number of enemies on the screen.

    // Current Enemy Stats
    public float currentHealth;
    public float currentDamage;
    public float currentSpeed; // Accessed by movement

    [Header("Damage Feedback")]
    public Color damageColor = new(1,0,0,1); // Color of damage flash
    float damageFlashDuration = 0.2f; // How long the flash would last
    float deathFadeTime = 0.2f; // How long it takes for enemy to fade
    SpriteRenderer sprite;
    Color originalColor;

    void Awake()
    {
        count++;
    }

    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject
        originalColor = sprite.color;
        movement = GetComponent<EnemyMovement>();
    }

    // This function always needs at least 2 values, the amount of damage dealt <dmg>, as well as where the damage is coming from, which is passed as <sourcePosition>
    // The <sourcePosition> is necessary because it is used to calculate the direction of the knockback
    // Optional parameters for knockback since weapon might not always have knockback. Knockback duration is always 120 milliseconds
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 0f, float knockbackDuration = 0.12f)
    {
        if (this != null)
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
    }
    // Coroutine function that makes the enemy flash when taking damage
    IEnumerator DamageFlash() 
    {
        sprite.color = damageColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sprite.color = originalColor;
    }

    public void Kill()
    {
        // Enable drops if the enemy is killed,
        // since drops are disabled by default.
        DropRateManager drops = GetComponent<DropRateManager>();
        if(drops) drops.active = true;

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

    void OnCollisionStay2D(Collision2D collision)
    {
        // Reference the script from the collided collider and deal damage using TakeDamage()
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats player = collision.gameObject.GetComponent<PlayerStats>();
            player.TakeDamage(currentDamage); // Make sure to use currentDamage instead of weaponData.Damage in case any damage multipliers in the future
        }
    }

    private void OnDestroy()
    {
        count--;
    }

    public void TakeDamage()
    {
        // This method is left empty, for IDamageable
    }
}

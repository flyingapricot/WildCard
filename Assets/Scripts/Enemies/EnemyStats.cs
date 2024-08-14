using System.Collections;
using UnityEngine;

// To be placed on Enemy Prefabs

public class EnemyStats : MonoBehaviour, IDamageable
{
    EnemyMovement movement;
    public static int count; // Track the number of enemies on the screen.
    public enum EnemyType { basic, elite, boss }; // Used to determine highscore
    public EnemyType enemyType;

    #region Stats
    [System.Serializable]
    public struct Resistances
    {
        [Range(0f, 1f)] public float freeze, kill, debuff;
        // To allow us to multiply the resistances.
        public static Resistances operator *(Resistances r, float factor)
        {
            r.freeze = Mathf.Min(1, r.freeze * factor);
            r.kill = Mathf.Min(1, r.kill * factor);
            r.debuff = Mathf.Min(1, r.debuff * factor);
            return r;
        }
    }

    [System.Serializable]
    public struct Stats
    {
        [Min(0)] public float maxHealth, damage, moveSpeed, knockbackMultiplier;
        public Resistances resistances;
        
        [System.Flags]
        public enum Boostable { health = 1, moveSpeed = 2, damage = 4, knockbackMultiplier = 8, resistances = 16 }
        public Boostable curseBoosts, levelBoosts;

        private static Stats Boost(Stats s1, float factor, Boostable boostable)
        {
            if ((boostable & Boostable.health) != 0) s1.maxHealth *= factor;
            if ((boostable & Boostable.moveSpeed) != 0) s1.moveSpeed *= factor;
            if ((boostable & Boostable.damage) != 0) s1.damage *= factor;
            if ((boostable & Boostable.knockbackMultiplier) != 0) s1.knockbackMultiplier /= factor;
            if ((boostable & Boostable.resistances) != 0) s1.resistances *= factor;
            return s1;
        }

        // Use the multiply operator for curse.
        public static Stats operator *(Stats s1, float factor) { return Boost(s1, factor, s1.curseBoosts); }

        // Use the XOR operator for level boosted stats.
        public static Stats operator ^(Stats s1, float factor) { return Boost(s1, factor, s1.levelBoosts); }
    }

    public Stats baseStats = new() { maxHealth = 10,  damage = 3, moveSpeed = 1, knockbackMultiplier = 1 };
    Stats actualStats;
    public Stats Actual
    {
        get { return actualStats; }
    }
    float currentHealth;
    #endregion

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
        movement = GetComponent<EnemyMovement>();
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject
        originalColor = sprite.color;
        RecalculateStats();
        currentHealth = actualStats.maxHealth;
    }

    // Calculates the actual stats of the enemy based on a variety of factors.
    public void RecalculateStats()
    {
        // Calculate curse boosts.
        float curse = GameManager.GetCumulativeCurse(),
        level = GameManager.GetCumulativeLevels();
        actualStats = (baseStats * curse) ^ level;
    }

    // This function always needs at least 2 values, the amount of damage dealt <dmg>, as well as where the damage is coming from, which is passed as <sourcePosition>
    // The <sourcePosition> is necessary because it is used to calculate the direction of the knockback
    // Optional parameters for knockback since weapon might not always have knockback. Knockback duration is always 120 milliseconds
    public void TakeDamage(float dmg, Vector2 sourcePosition, float knockbackForce = 0f, float knockbackDuration = 0.12f)
    {
        if (this != null)
        {
            // If damage is exactly equal to maximum health, we assume it is an insta-kill and 
            // check for the kill resistance to see if we can dodge this damage.
            if(Mathf.Approximately(dmg, actualStats.maxHealth))
            {
                // Roll a die to check if we can dodge the damage.
                // Gets a random value between 0 to 1, and if the number is 
                // below the kill resistance, then we avoid getting killed.
                if(Random.value < actualStats.resistances.kill)
                {
                    return; // Don't take damage.
                }
            }

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

        // Increment respective enemy type count
        if (enemyType == EnemyType.basic) { GameManager.instance.basicDefeated++; }
        if (enemyType == EnemyType.elite) { GameManager.instance.eliteDefeated++; }
        if (enemyType == EnemyType.boss) { GameManager.instance.bossDefeated++; }

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

    void OnCollisionStay2D(Collision2D col)
    {
        // Check for whether there is a PlayerStats object we can damage.
        if(col.collider.TryGetComponent(out PlayerStats p))
        {
            p.TakeDamage(Actual.damage);
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

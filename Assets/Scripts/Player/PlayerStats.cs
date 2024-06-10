using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Animation;
using UnityEngine;

// To be placed on Character Prefabs

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    CharacterScriptableObject characterData;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] HealthBar healthBar;
    [SerializeField] EXPBar expBar;

    // Current Player Stats
    [HideInInspector] public float currentMoveSpeed; // Accessed by movement
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentMaxHealth;
    [HideInInspector] public float currentRecovery;
    [HideInInspector] public float currentMight;
    [HideInInspector] public float currentProjectileSpeed;
    [HideInInspector] public float currentMagnet;


    [Header("Invincibility Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;


    [Header("Spawned Weapons")]
    public List<GameObject> weaponList;


    [Header("Experience / Level")]
    public int experience = 0;
    public int level = 1;
    public int experienceCap;

    // Class for defining level ranges and the corresponding increase in experience cap
    [System.Serializable]
    public class LevelRange
    {
        public int startLevel;
        public int endLevel;
        public int experienceCapIncrease;
    }
    public List<LevelRange> levelRanges;


    void Awake()
    {
        if (instance == null) // Singleton Pattern
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("EXTRA " + this + " DELETED");
        }

        characterData = CharacterSelector.GetData();
        CharacterSelector.instance.DestorySingleton();

        currentMoveSpeed = characterData.MoveSpeed;
        currentHealth = characterData.MaxHealth;
        currentMaxHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentMight = characterData.Might;
        currentProjectileSpeed = characterData.ProjectileSpeed;
        currentMagnet = characterData.Magnet;

        healthBar.InitializeHealthBar(currentHealth); // Initialize the health bar

        // Spawn the starting weapon
        SpawnWeapon(characterData.StartingWeapon);

        // Set sprite and animator from CharacterScriptableObject
        spriteRenderer.sprite = characterData.CharacterSprite;
        animator.runtimeAnimatorController = characterData.CharacterAnimation;
    }

    void Start()
    {
        // Initialize experience cap to prevent player from immediately leveling up
        experienceCap = 5;
        //experienceCap = levelRanges[0].experienceCapIncrease;
    }

    void Update()
    {
        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        else if (isInvincible) // Will enter once invincibilityTimer <= 0
        {
            isInvincible = false; // i-frame runs out
        }

        Recover();
    }

    public void GainExperience(int amount)
    {
        experience += amount;
        expBar.SetExp(experience, experienceCap);
        LevelUp();
    }
    // Level 1 = 5 XP required >> Level 2 = 15 XP >> Level 3 = 25 XP 
    // Increment of 10 XP to the experience cap until level 20 = 195 XP required.
    // Level 21 = 208 XP >> Level 40 = 455 XP, increments the cap by 13 XP each level
    // Level 41 = 471 XP onwards increments the cap by 16 XP each level.
    void LevelUp()
    {
        if (experience >= experienceCap) // Checks if enough XP to level up
        {
            level++;
            experience -= experienceCap;

            int experienceCapIncrease = 0;
            foreach (LevelRange range in levelRanges) // Updates experienceCapIncrease to the relevant level range
            {
                if (level >= range.startLevel && level <= range.endLevel)
                {
                    experienceCapIncrease = range.experienceCapIncrease;
                    break;
                }
            }
            experienceCap += experienceCapIncrease;
            expBar.SetExp(experience, experienceCap);

            // Stat increases
            currentMaxHealth++;
            Heal(currentMaxHealth); // fully heal
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible) // If player is not invincible, take dmg and start i-frame
        {
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            currentHealth -= dmg;
            if (currentHealth <= 0)
            {
                // Destroy(gameObject);
                Debug.Log("Player is Dead. GAME OVER.");
            }
            healthBar.SetHealth(currentHealth, currentMaxHealth); 
        }
    }

    public void Heal(float heal)
    {
        // Only heal if player health not max
        if (currentHealth < currentMaxHealth)
        {
            if (currentHealth <= 0)
            {
                return; // Does not heal if player is dead
            }

            currentHealth += heal;

            if (currentHealth > currentMaxHealth)
            {
                currentHealth = currentMaxHealth; // Prevent exceeding max health
            }
            healthBar.SetHealth(currentHealth, currentMaxHealth); // Update Health Bar
        }
    }

    void Recover()
    {
        if (currentHealth < currentMaxHealth)
        {
            currentHealth += currentRecovery * Time.deltaTime;

            if (currentHealth > currentMaxHealth)
            {
                currentHealth = currentMaxHealth; // Prevent exceeding max health
            }
            healthBar.SetHealth(currentHealth, currentMaxHealth); // Update Health Bar
        }
    }

    public void SpawnWeapon(GameObject weapon)
    {
        // Spawn the starting weapon
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); // Place weapon inside player gameobject
        weaponList.Add(spawnedWeapon); // Add it to the list of spawned weapons
    }
}

using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Animation;
using UnityEngine;

// To be placed on Character Prefabs

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    CharacterScriptableObject characterData;
    WeaponScriptableObject weaponData;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] HealthBar healthBar;
    [SerializeField] EXPBar expBar;

    // Current Player Stats
    [HideInInspector] public float currentMaxHealth;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentRecovery;
    [HideInInspector] public float currentArmour;
    [HideInInspector] public float currentMoveSpeed; // Accessed by movement
    [HideInInspector] public float currentMagnet;

    // Current Weapon Stats
    [HideInInspector] public float currentDamage;
    [HideInInspector] public float currentArea;
    [HideInInspector] public float currentProjectileSpeed;
    [HideInInspector] public float currentDuration;
    [HideInInspector] public float currentAmount; 
    [HideInInspector] public float currentCooldown;
    [HideInInspector] public float currentPierce;

    // Current Passive Stats
    [HideInInspector] public float currentLuck;
    [HideInInspector] public float currentGrowth;
    [HideInInspector] public float currentGreed;
    [HideInInspector] public float currentCurse;
    [HideInInspector] public float currentRevival; 
    [HideInInspector] public float currentReroll;
    [HideInInspector] public float currentSkip;
    [HideInInspector] public float currentBanish;


    [Header("Invincibility Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;


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


    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;

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

        characterData = CharacterSelector.GetCharacterData();
        weaponData = CharacterSelector.GetWeaponData();
        CharacterSelector.instance.DestroySingleton();

        inventory = GetComponent<InventoryManager>();

        currentHealth = characterData.MaxHealth;
        currentMaxHealth = characterData.MaxHealth;
        currentRecovery = characterData.Recovery;
        currentArmour = characterData.Armour;
        currentMoveSpeed = characterData.MoveSpeed;
        currentMagnet = characterData.Magnet;

        currentDamage = weaponData.Damage;
        currentArea = weaponData.Area;
        currentProjectileSpeed = weaponData.ProjectileSpeed;
        currentDuration = weaponData.Duration;
        currentAmount = weaponData.Amount;
        currentCooldown = weaponData.Cooldown;
        currentPierce = weaponData.Pierce;

        currentLuck = characterData.Luck;
        currentGrowth = characterData.Growth;
        currentGreed = characterData.Greed;
        currentCurse = characterData.Curse;
        currentRevival = characterData.Revival;
        currentReroll = characterData.Reroll;
        currentSkip = characterData.Skip;
        currentBanish = characterData.Banish;

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

    public void Heal(float heal) // Active healing
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

    void Recover() // Passive recovery
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
        if (weaponIndex >= inventory.weaponSlots.Count - 1)
        {
            Debug.LogError("Inventory Full.");
            return;
        }
        
        // Spawn the starting weapon
        GameObject spawnedWeapon = Instantiate(weapon, transform.position, Quaternion.identity);
        spawnedWeapon.transform.SetParent(transform); // Place weapon inside player gameobject
        inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); // Adds weapon to its inventory slot
        weaponIndex++;
    }

    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveItemSlots.Count - 1)
        {
            Debug.LogError("Inventory Full.");
            return;
        }

        // Spawn the starting passive item
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); // Place passive item inside player gameobject
        inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); // Adds passive item to its inventory slot
        passiveItemIndex++;
    }
}

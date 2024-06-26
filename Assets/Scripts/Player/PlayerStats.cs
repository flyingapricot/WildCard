using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Animation;
using UnityEngine;


public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    CharacterScriptableObject characterData;
    WeaponScriptableObject weaponData;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    [SerializeField] HealthBar healthBar;
    [SerializeField] EXPBar expBar;

    #region Current Player Stats
    float currentMaxHealth;
    float currentHealth;
    float currentRecovery;
    float currentArmour;
    float currentMoveSpeed; // Accessed by movement
    float currentMagnet;

    public float CurrentMaxHealth
    {
        get { return currentMaxHealth; }
        set
        {
            // Check if value has changed
            if (currentMaxHealth != value)
            {
                currentMaxHealth = value;
            }
        }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            // Clamp the value to ensure it's within valid range
            value = Mathf.Clamp(value, 0, CurrentMaxHealth);
            // Check if value has changed
            if (currentHealth != value)
            {
                currentHealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Health.text = "Health: " + Mathf.RoundToInt(currentHealth) + " / " + CurrentMaxHealth; // Round off to nearest int
                }
                healthBar.SetHealth(currentHealth, CurrentMaxHealth); 
            }
        }
    }

    public float CurrentRecovery
    {
        get { return currentRecovery; }
        set
        {
            // Check if value has changed
            if (currentRecovery != value)
            {
                currentRecovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Recovery.text = "Recovery: " + currentRecovery;
                }
            }
        }
    }

    public float CurrentArmour
    {
        get { return currentArmour; }
        set
        {
            // Check if value has changed
            if (currentArmour != value)
            {
                currentArmour = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Defence.text = "Defence: " + currentArmour;
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return currentMoveSpeed; }
        set
        {
            // Check if value has changed
            if (currentMoveSpeed != value)
            {
                currentMoveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Speed.text = "Speed: " + currentMoveSpeed;
                }
            }
        }
    }

    public float CurrentMagnet
    {
        get { return currentMagnet; }
        set
        {
            // Check if value has changed
            if (currentMagnet != value)
            {
                currentMagnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Magnet.text = "Magnet: " + currentMagnet;
                }
            }
        }
    }
    #endregion

    #region Current Weapon Stats
    float currentDamage;
    [HideInInspector] public float currentArea;
    [HideInInspector] public float currentProjectileSpeed;
    [HideInInspector] public float currentDuration;
    [HideInInspector] public float currentAmount; 
    [HideInInspector] public float currentCooldown;
    [HideInInspector] public float currentPierce;

    public float CurrentDamage
    {
        get { return currentDamage; }
        set
        {
            // Check if value has changed
            if (currentDamage != value)
            {
                currentDamage = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Attack.text = "Attack: " + currentDamage;
                }
            }
        }
    }
    #endregion

    #region Current Passive Stats
    [HideInInspector] public float currentLuck;
    [HideInInspector] public float currentGrowth;
    [HideInInspector] public float currentGreed;
    [HideInInspector] public float currentCurse;
    [HideInInspector] public float currentRevival; 
    [HideInInspector] public float currentReroll;
    [HideInInspector] public float currentSkip;
    [HideInInspector] public float currentBanish;
    #endregion

    #region Invincibility Frames
    [Header("Invincibility Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;
    #endregion

    #region Experience / Levels
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
    #endregion

    #region Inventory
    InventoryManager inventory;
    public int weaponIndex;
    public int passiveItemIndex;
    #endregion

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

        #region Getting Data
        CurrentMaxHealth = characterData.MaxHealth;
        CurrentHealth = characterData.MaxHealth;
        CurrentRecovery = characterData.Recovery;
        CurrentArmour = characterData.Armour;
        CurrentMoveSpeed = characterData.MoveSpeed;
        CurrentMagnet = characterData.Magnet;

        CurrentDamage = weaponData.Damage;
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
        #endregion

        healthBar.InitializeHealthBar(CurrentMaxHealth); // Initialize the health bar

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

        // Set the current stats display
        GameManager.instance.Health.text = "Health: " + Mathf.RoundToInt(currentHealth) + " / " + CurrentMaxHealth; // Round off to nearest int
        GameManager.instance.Attack.text = "Attack: " + currentDamage;
        GameManager.instance.Defence.text = "Defence: " + currentArmour;
        GameManager.instance.Recovery.text = "Recovery: " + currentRecovery;
        GameManager.instance.Speed.text = "Speed: " + currentMoveSpeed;
        GameManager.instance.Magnet.text = "Magnet: " + currentMagnet;

        GameManager.instance.AssignCharacterUI(characterData);
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

        Recover(); // Call Recover in Update to enable passive recovery
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
            CurrentMaxHealth++;
            Heal(CurrentMaxHealth); // fully heal
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible) // If player is not invincible, take dmg and start i-frame
        {
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            CurrentHealth -= dmg;
            if (CurrentHealth <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        if (!GameManager.instance.isGameOver)
        {
            GameManager.instance.AssignLevelReached(level);
            GameManager.instance.AssignInventory(inventory.weaponUISlots, inventory.passiveItemUISlots);
            GameManager.instance.GameOver();
        }
    }

    public void Heal(float heal) // Active healing
    {
        // Only heal if player health not max
        if (CurrentHealth < CurrentMaxHealth)
        {
            if (CurrentHealth <= 0)
            {
                return; // Does not heal if player is dead
            }

            CurrentHealth += heal;
        }
    }

    void Recover() // Passive recovery
    {
        if (CurrentHealth < CurrentMaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
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

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    public CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    [Header("UI")]
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] Animator playerAnimator;
    [SerializeField] HealthBar healthBar;
    [SerializeField] EXPBar expBar;

    [Header("Visual Feedback")]
    public GameObject healingEffect; // Healing animation
    public GameObject hitEffect; // Getting Damaged animation

    #region Current Player Stats
    float health;
    public float CurrentHealth
    {
        get { return health; }

        // If we try and set the current health
        // Pause screen stats and health bar will be updated
        set
        {
            // Clamp the value to ensure it's within valid range
            // value = Mathf.Clamp(value, 0, CurrentMaxHealth);

            // Check if value has changed
            if (health != value)
            {
                health = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Health.text = string.Format("{0} / {1}", Mathf.RoundToInt(health), actualStats.maxhealth);
                }
                healthBar.SetHealth(health, MaxHealth); 
            }
        }
    }

    public float MaxHealth
    {
        get { return actualStats.maxhealth; }

        // If we try and set the current health
        // Pause screen stats and health bar will be updated
        set
        {
            // Check if value has changed
            if (actualStats.maxhealth != value)
            {
                actualStats.maxhealth = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Health.text = string.Format("{0} / {1}", Mathf.RoundToInt(health), actualStats.maxhealth);
                }
                healthBar.SetHealth(health, MaxHealth); 
            }
        }
    }

    public float CurrentMight
    {
        get { return Might; }
        set { Might = value; }
    }
    public float Might
    {
        get { return actualStats.might; }
        set
        {
            // Check if value has changed
            if (actualStats.might != value)
            {
                actualStats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Recovery.text = actualStats.might.ToString();
                }
            }
        }
    }

    public float CurrentRecovery
    {
        get { return Recovery; }
        set { Recovery = value; }
    }
    public float Recovery
    {
        get { return actualStats.recovery; }
        set
        {
            // Check if value has changed
            if (actualStats.recovery != value)
            {
                actualStats.recovery = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Recovery.text = actualStats.recovery.ToString();
                }
            }
        }
    }

    public float CurrentArmour
    {
        get { return Armour; }
        set { Armour = value; }
    }
    public float Armour
    {
        get { return actualStats.armour; }
        set
        {
            // Check if value has changed
            if (actualStats.armour != value)
            {
                actualStats.armour = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Defence.text = actualStats.armour.ToString();
                }
            }
        }
    }

    public float CurrentMoveSpeed
    {
        get { return MoveSpeed; }
        set { MoveSpeed = value; }
    }
    public float MoveSpeed
    {
        get { return actualStats.moveSpeed; }
        set
        {
            // Check if value has changed
            if (actualStats.moveSpeed != value)
            {
                actualStats.moveSpeed = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Speed.text = actualStats.moveSpeed.ToString();
                }
            }
        }
    }

    public float CurrentMagnet
    {
        get { return Magnet; }
        set { Magnet = value; }
    }
    public float Magnet
    {
        get { return actualStats.magnet; }
        set
        {
            // Check if value has changed
            if (actualStats.magnet != value)
            {
                actualStats.magnet = value;
                if (GameManager.instance != null)
                {
                    GameManager.instance.Magnet.text = actualStats.magnet.ToString();
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
    public int experience; // Player current experience points
    public int level; // Player current level
    public int experienceCap; // Experience needed to level up

    // Class for defining level ranges and the corresponding increase in experience cap
    // [System.Serializable]
    // public class LevelRange
    // {
    //     public int startLevel;
    //     public int endLevel;
    //     public int experienceCapIncrease;
    // }
    // public List<LevelRange> levelRanges;
    #endregion

    #region Inventory
    PlayerInventory inventory;
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

        characterData = CharacterSelector.GetData();
        if (CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();

        playerSprite.sprite = characterData.Sprite;
        playerAnimator.runtimeAnimatorController = characterData.Animation;

        inventory = GetComponent<PlayerInventory>();

        baseStats = actualStats = characterData.stats;
        health = actualStats.maxhealth;

        healthBar.InitializeHealthBar(health); // Initialize the health bar
    }

    void Start()
    {
        // Spawn the starting weapon
        inventory.Add(characterData.StartingWeapon);

        // Initialize experience cap to prevent player from immediately leveling up
        experienceCap = 79;
        experience = 0;
        level = 1;
        //experienceCap = levelRanges[0].experienceCapIncrease;

        // Set the current stats display
        GameManager.instance.Health.text = string.Format("{0} / {1}", Mathf.RoundToInt(CurrentHealth), MaxHealth);
        GameManager.instance.Attack.text = CurrentMight.ToString();
        GameManager.instance.Defence.text = CurrentArmour.ToString();
        GameManager.instance.Recovery.text = CurrentRecovery.ToString();
        GameManager.instance.Speed.text = CurrentMoveSpeed.ToString();
        GameManager.instance.Magnet.text = CurrentMagnet.ToString();

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

    public void RecalculateStats()
    {
        actualStats = baseStats; // Recalculate current stats using base stats
        foreach (PlayerInventory.Slot s in inventory.passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p)
            {
                actualStats += p.GetBoosts();
            }
        }
    }

    public void GainExperience(int amount)
    {
        experience += amount;
        LevelUp(); // Call LevelUp first to handle leveling logic
        expBar.SetExp(experience, experienceCap, level); // Then update the EXP bar with the final values
    }

    // Method to calculate the experience required for the next level
    public int ExperienceCapIncrease(int currentLevel)
    {
        float nextLevelExp = Mathf.Pow(4 * (currentLevel + 1), 2.1f);
        float currentLevelExp = Mathf.Pow(4 * currentLevel, 2.1f);

        return Mathf.RoundToInt(nextLevelExp) - Mathf.RoundToInt(currentLevelExp);
    }

    void LevelUp()
    {
        while (experience >= experienceCap) // Using a loop in case the player gains multiple levels at once
        {
            level++;
            experience -= experienceCap;
            experienceCap = ExperienceCapIncrease(level);

            // int experienceCapIncrease = 0;
            // foreach (LevelRange range in levelRanges) // Updates experienceCapIncrease to the relevant level range
            // {
            //     if (level >= range.startLevel && level <= range.endLevel)
            //     {
            //         experienceCapIncrease = range.experienceCapIncrease;
            //         break;
            //     }
            // }
            // experienceCap += experienceCapIncrease;

            GameManager.instance.StartLevelUp();
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible) // If player is not invincible, take dmg and start i-frame
        {
            // Calculate incoming damage ensuring it doesn't drop below 1
            float incomingDmg = Mathf.Max(dmg - CurrentArmour, 1);
            CurrentHealth -= incomingDmg;

            // Instantiate the damage effect
            if (hitEffect != null)
            {
                GameObject damageAnimation = Instantiate(hitEffect, transform.position, Quaternion.identity, transform);
                StartCoroutine(DestroyAfterAnimation(damageAnimation)); // Remove effect after the animation
            }

            invincibilityTimer = invincibilityDuration;
            isInvincible = true;

            // Check if the player's health has dropped to or below 0
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
            // GameManager.instance.AssignCharacterUI(characterData);
            GameManager.instance.AssignLevelReached(level);
            GameManager.instance.AssignInventory(inventory.weaponSlots, inventory.passiveSlots);
            GameManager.instance.GameOver();
        }
    }

    public void Heal(float heal) // Active healing
    {
        // Only heal if player health is not max
        if (CurrentHealth < MaxHealth)
        {
            // if (CurrentHealth <= 0)
            // {
            //     return; // Does not heal if player is dead
            // }

            CurrentHealth += heal;

            // Optionally, ensure the health doesn't exceed the maximum health
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        }

        // Instantiate the healing effect
        if (healingEffect != null)
        {
            GameObject healingAnimation = Instantiate(healingEffect, transform.position, Quaternion.identity, transform);
            StartCoroutine(DestroyAfterAnimation(healingAnimation)); // Remove effect after the animation
        }
    }

    void Recover() // Passive recovery
    {
        if (CurrentHealth < MaxHealth)
        {
            CurrentHealth += CurrentRecovery * Time.deltaTime;
        }
    }

    [System.Obsolete("Old function that is kept to maintain compatibility with InventoryManager. Removing Soon")]
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
        // inventory.AddWeapon(weaponIndex, spawnedWeapon.GetComponent<WeaponController>()); // Adds weapon to its inventory slot
        weaponIndex++;
    }

    [System.Obsolete("Old function that is kept to maintain compatibility with InventoryManager. Removing Soon")]
    public void SpawnPassiveItem(GameObject passiveItem)
    {
        if (passiveItemIndex >= inventory.passiveSlots.Count - 1)
        {
            Debug.LogError("Inventory Full.");
            return;
        }

        // Spawn the starting passive item
        GameObject spawnedPassiveItem = Instantiate(passiveItem, transform.position, Quaternion.identity);
        spawnedPassiveItem.transform.SetParent(transform); // Place passive item inside player gameobject
        // inventory.AddPassiveItem(passiveItemIndex, spawnedPassiveItem.GetComponent<PassiveItem>()); // Adds passive item to its inventory slot
        passiveItemIndex++;
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }

    private IEnumerator DestroyAfterAnimation(GameObject statusEffect)
    {
        Animator statusAnimator = statusEffect.GetComponent<Animator>();
        float animationLength = 0;

        // If the status effect has an Animator, get the length of the animation
        if (statusAnimator != null)
        {
            animationLength = statusAnimator.GetCurrentAnimatorStateInfo(0).length;
        }
        else
        {
            // If there's no Animator, set a default duration (e.g., 1 second)
            animationLength = 1.0f;
        }

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationLength);

        // Destroy the status effect GameObject
        Destroy(statusEffect);
    }
}

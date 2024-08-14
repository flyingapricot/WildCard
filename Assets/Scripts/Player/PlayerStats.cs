using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats instance;
    PlayerCollector collector;
    PlayerInventory inventory;

    #region Current Player Stats
    public CharacterData characterData;
    public CharacterData.Stats baseStats;
    [SerializeField] CharacterData.Stats actualStats;

    public CharacterData.Stats Stats
    {
        get { return actualStats;  }
        set { actualStats = value; }
    }
    public CharacterData.Stats Actual
    {
        get { return actualStats; }
    }

    float health;
    public float CurrentHealth
    {
        get { return health; }
        // If we try and set the current health, the UI interface
        // on the pause screen will also be updated.
        set
        {
            // Check if the value has changed
            if (health != value)
            {
                health = value;
                // Update the Health Bar whenever the player’s health is changed
                healthBar.SetHealth(CurrentHealth, Stats.maxHealth);
            }
        }
    }
    #endregion

    #region UI
    [Header("UI")]
    [SerializeField] SpriteRenderer playerSprite;
    [SerializeField] Animator playerAnimator;
    [SerializeField] HealthBar healthBar;
    [SerializeField] EXPBar expBar;

    [Header("Visual Feedback")]
    public GameObject healAnimation;
    public GameObject healEffect; 
    public GameObject hitAnimation; 
    public GameObject hitEffect; 
    public GameObject reviveAnimation; 
    public GameObject reviveEffect; 
    //public ParticleSystem blockedEffect; // If armor completely blocks damage.

    [Header("Audio Feedback")]
    [SerializeField] private AudioClip hitAudio; // Sound effect when damaged
    [SerializeField] private AudioClip healAudio; // Sound effect when healed
    [SerializeField] private AudioClip levelAudio; // Sound effect when leveling up
    [SerializeField] private AudioClip reviveAudio; // Sound effect when player revived
    private AudioSource audioSource; // The audio source that will play all the SFX
    #endregion

    #region Invincibility Frames
    [Header("Invincibility Frames")]
    public float invincibilityDuration;
    float invincibilityTimer;
    bool isInvincible;
    #endregion

    #region Experience / Levels
    [Header("Experience / Level")]
    public int experience = 0; // Player current experience points
    public int level = 1; // Player current level
    public int experienceCap; // Experience needed to level up
    #endregion

    void Awake()
    {
        if (instance == null) { // Singleton Pattern
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroySingleton(); }

        inventory = GetComponent<PlayerInventory>();
        collector = GetComponentInChildren<PlayerCollector>();
        audioSource = GetComponent<AudioSource>();

        // Get chosen character data
        characterData = CharacterSelector.GetData();
        if (CharacterSelector.instance)
            CharacterSelector.instance.DestroySingleton();

        playerSprite.sprite = characterData.Sprite;
        playerAnimator.runtimeAnimatorController = characterData.Animation;

        // Assign the initial stats
        baseStats = actualStats = characterData.stats;
        collector.SetRadius(actualStats.magnet);
        health = actualStats.maxHealth;
    }

    void Start()
    {
        // Spawn the starting weapon
        inventory.Add(characterData.StartingWeapon);

        // Initialize the health bar
        healthBar.InitializeHealthBar(health); 

        // Initialize experience cap to prevent player from immediately leveling up
        experienceCap = SetExperienceCap(1);

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
        // Update the PlayerCollector's radius.
        collector.SetRadius(actualStats.magnet);
    }

    public void GainExperience(int amount)
    {
        experience += amount;
        LevelUp(); // Call LevelUp first to handle leveling logic
        expBar.SetExp(experience, experienceCap, level); // Then update the EXP bar with the final values
    }
    
    public int SetExperienceCap(int currentLevel) // Method to calculate the experience required for the next level
    {
        float nextLevelExp = Mathf.Pow(4 * (currentLevel + 1), 2f);
        float currentLevelExp = Mathf.Pow(4 * currentLevel, 2f);

        return Mathf.RoundToInt(nextLevelExp) - Mathf.RoundToInt(currentLevelExp);
    }

    void LevelUp()
    {
        while (experience >= experienceCap) // Using a loop in case the player gains multiple levels at once
        {
            // Play level up sound
            audioSource.clip = levelAudio;
            audioSource.Play();

            // Level up the player and reduce their experience by the experience cap
            level++;
            experience -= experienceCap;
            experienceCap = SetExperienceCap(level);
            GameManager.instance.StartLevelUp();
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!isInvincible) // If player is not invincible, take dmg and start i-frame
        {
            // Calculate incoming damage ensuring it doesn't drop below 1
            float incomingDmg = Mathf.Max(dmg - Stats.armour, 1);
            CurrentHealth -= incomingDmg;

            PlayAudio(hitAudio);
            PlayEffect(hitAnimation, hitEffect);

            // Check if the player's health has dropped to or below 0
            if (CurrentHealth <= 0)
            {
                // If have revives, revive player with 30% health
                if (Stats.revive > 0)
                {
                    PlayAudio(reviveAudio);
                    PlayEffect(reviveAnimation, reviveEffect);
                    CurrentHealth = Stats.maxHealth / 3;
                    actualStats.revive--;
                }
                else
                {
                    Kill();
                }
            }
            
            invincibilityTimer = invincibilityDuration;
            isInvincible = true;
        }
    }

    public void Kill()
    {
        if (!GameManager.instance.IsGameOver)
        {
            GameManager.instance.AssignScore(level);
            GameManager.instance.GameOver();
        }
    }

    public void Heal(float heal) // Active healing
    {
        // Only heal if player health is not max
        if (CurrentHealth < Stats.maxHealth)
        {
            CurrentHealth += heal;

            // Ensure the health doesn't exceed the maximum health
            CurrentHealth = Mathf.Min(CurrentHealth, Stats.maxHealth);
        }

        PlayAudio(healAudio);
        PlayEffect(healAnimation, healEffect);
    }

    void Recover() // Passive recovery
    {
        if (CurrentHealth < Stats.maxHealth)
        {
            CurrentHealth += Stats.recovery * Time.deltaTime;
            CurrentHealth = Mathf.Min(CurrentHealth, Stats.maxHealth);
        }
    }

    public void DestroySingleton()
    {
        instance = null;
        Destroy(gameObject);
    }

    private void PlayEffect(GameObject animation, GameObject effect)
    {
        // Instantiate the animation
        if (animation != null)
        {
            GameObject Animation = Instantiate(animation, transform.position, Quaternion.identity, transform);
            StartCoroutine(DestroyAfterAnimation(Animation)); // Remove effect after the animation
        }
        // Instantiate the effect at the calculated position
        if (effect != null)
        {
            Camera mainCamera = FindObjectOfType<Camera>();
            // Calculate the position in the middle of the screen
            Vector3 screenCenter = new(Screen.width / 2, Screen.height / 2, mainCamera.nearClipPlane);
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(screenCenter);

            GameObject Effect = Instantiate(effect, worldPosition, Quaternion.identity);
            Effect.transform.SetParent(mainCamera.transform); // Make the effect a child of the camera

            StartCoroutine(DestroyAfterAnimation(Effect)); // Remove effect after the animation
        }
    }

    public void PlayAudio(AudioClip audio)
    {
        if (audio != null)
        {
            audioSource.PlayOneShot(audio);
        }
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

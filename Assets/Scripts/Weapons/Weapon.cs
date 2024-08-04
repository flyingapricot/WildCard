using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to be attached to all Weapon Prefabs. 
/// The weapon prefab works together with the WeaponData Scriptableobjects to manage and run the behaviours of all weapons in the game.
/// </summary>

public abstract class Weapon : Item
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description; // Name / Description of the Weapon or its upgrade

        [Header("Visuals")]
        public Projectile projectilePrefab; // If attached, a projectile will spawn every time the weapon cools down.
        public Aura auraPrefab; // If attached, an aura will spawn when weapon is equipped
        public ParticleSystem hitEffect; // Effect on enemy
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan; // If 0, it will last forever
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int amount, pierce, maxInstances; // The maximum amount of active projectiles

        // Allows us to use the + operator to add 2 Stats together
        // Very important when we want to increase out weapon stats
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new()
            {
                // If following properties are not null in s2, use their values
                // Otherwise, use corresponding values from s1
                name = s2.name ?? s1.name,
                description = s2.description ?? s1.description,
                projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab,
                auraPrefab = s2.auraPrefab ?? s1.auraPrefab,
                hitEffect = s2.hitEffect == null ? s1.hitEffect : s2.hitEffect,

                spawnVariance = s2.spawnVariance,
                lifespan = s1.lifespan + s2.lifespan, // Modifies how long the attack lasts
                damage = s1.damage + s2.damage, // Modifies the damage of all attacks. Base value = 100%, Max value = 1000%
                damageVariance = s1.damageVariance + s2.damageVariance,
                area = s1.area + s2.area, // Modifies the area of all attacks. Base value = 100%, Max value = 1000%
                speed = s1.speed + s2.speed, // Modifies the movement speed of all projectiles. Base value = 100%, Max value = 500%
                cooldown = s1.cooldown + s2.cooldown, // Modifies the duration of the cooldown between attacks. Base value = 100%, Max value = 10%
                amount = s1.amount + s2.amount, // Determines the amount of projectiles. Base value = 1, Max value = 10
                pierce = s1.pierce + s2.pierce, // Modifies the amount of targets the projectile can hit before being destroyed. Base value = 1
                projectileInterval = s1.projectileInterval + s2.projectileInterval, // Modifies the duration between projectiles shot.
                knockback = s1.knockback + s2.knockback // Modifies the amount that enemies are pushed backwards whenever they are damaged by the player. Base value = 0, 120 milliseconds
            };
            return result;
        }

        // Get damage dealt
        public float GetDamage()
        {
            return damage + UnityEngine.Random.Range(0, damageVariance);
        }
    }

    public WeaponData data;
    protected PlayerMovement movement;
    protected Stats currentStats;
    protected float currentCooldown;

    // For dynamically created weapons, call initialize to set everything up
    public virtual void Initialize(WeaponData data)
    {
        base.Initialize(data);
        this.data = data;
        currentStats = data.baseStats;
        movement = GetComponentInParent<PlayerMovement>();
        currentCooldown = currentStats.cooldown;
    }

    protected virtual void Awake()
    {
        // Assign the stats early, as it will be used by other scripts later
        if (data) currentStats = data.baseStats;
    }

    protected virtual void Start()
    {
        // Don't initialize the weapon if the weapon data is not assigned.
        if (data)
        {
            Initialize(data);
        }
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f) // Once the cooldown becomes 0, attack
        {
            Attack(currentStats.amount);
        }
    }

    // Levels up the weapon by 1 and calculates the corresponding stats
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        // Prevents level up if already at max level
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        // Otherwise, add stats of the next level to our weapon
        currentStats += data.GetLevelData(++currentLevel); // Level++
        return true;
    }

    // Lets us check whether this weapon can attack at this current moment
    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }
    // Performs an attack with the weapon
    // Returns true if attack was successful
    // This doesn't do anything. We have to override this at the child class to add a behaviour.
    protected virtual bool Attack(int attackCount = 1)
    {
        if (CanAttack())
        {
            currentCooldown += currentStats.cooldown;
            return true;
        }
        return false;
    }
    // Gets the amount of damage that the weapon is supposed to deal
    // Factoring in the weapon's stats (including damage variance)
    // As well as the character's Might stat.
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * player.Stats.might;
    }

    // For retrieving the weapon's stats
    public virtual Stats GetStats() { return currentStats; }
}

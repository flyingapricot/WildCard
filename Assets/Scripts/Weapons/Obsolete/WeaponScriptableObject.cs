using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This will be replaced by the WeaponData class.")]
[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]

// Used to create Unique Weapon Scriptable Objects
// Which are referenced to Weapon Behaviours in the Weapon Prefabs

public class WeaponScriptableObject : ScriptableObject // Inheritance
{
    [SerializeField] new string name; // Name of the Weapon or its upgrade
    public string Name { get => name; private set => name = value; }

    [SerializeField] public GameObject prefab;
    public GameObject Prefab { get => prefab; private set => prefab = value; }

    [SerializeField] Sprite icon; // Editor only
    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField] string description; // Description of the Weapon or its upgrade
    public string Description { get => description; private set => description = value; }

    [SerializeField] int level; // Editor only
    public int Level { get => level; private set => level = value; }

    [SerializeField] GameObject nextLevelPrefab; // Prefab of the next level weapon
    public GameObject NextLevelPrefab { get => nextLevelPrefab; private set => nextLevelPrefab = value; }


    [Header("Weapon Stats")]

    // Modifies the damage of all attacks.
    // Base value = 100%
    // Max value = 1000%
    [SerializeField] float damage;
    public float Damage { get => damage; private set => damage = value; }

    // Modifies the area of all attacks.
    // Base value = 100%
    // Max value = 1000%
    [SerializeField] float area;
    public float Area { get => area; private set => area = value; }

    // Modifies the movement speed of all projectiles.
    // Base value = 100%
    // Max value = 500%
    [SerializeField] float projectileSpeed;
    public float ProjectileSpeed { get => projectileSpeed; private set => projectileSpeed = value; }

    // Modifies the duration of weapon effects.
    // Base value = 100%
    // Max value = 500%
    [SerializeField] float duration;
    public float Duration { get => duration; private set => duration = value; }

    // Determines the amount of extra projectiles weapons have.
    // Base value = 0
    // Max value = 10
    [SerializeField] float amount;
    public float Amount { get => amount; private set => amount = value; }

    // Modifies the duration of the cooldown between attacks.
    // Base value = 100%
    // Max value = 10%
    [SerializeField] float cooldown; // Needs to be > destroyAfterSeconds
    public float Cooldown { get => cooldown; private set => cooldown = value; }

    // Modifies the amount of targets the projectile can hit before being destroyed.
    // Base value = 0
    [SerializeField] int pierce;
    public int Pierce { get => pierce; private set => pierce = value; }

    // Modifies the amount that enemies are pushed backwards whenever they are damaged by the player.
    // Base value = 0, 120 milliseconds
    [SerializeField] int knockback;
    public int Knockback { get => knockback; private set => knockback = value; }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This will be replaced by the CharacterData class.")]
[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/Character")]

// Used to create Unique Character Scriptable Objects
// Which are referenced to Character Stats in the Character Prefabs

public class CharacterScriptableObject : ScriptableObject 
{
    [SerializeField] Sprite characterName;
    public Sprite CharacterName { get => characterName; private set => characterName = value; }

    [SerializeField] Sprite characterSprite;
    public Sprite CharacterSprite { get => characterSprite; private set => characterSprite = value; }

    [SerializeField] RuntimeAnimatorController characterAnimation;
    public RuntimeAnimatorController CharacterAnimation { get => characterAnimation; private set => characterAnimation = value; }

    [SerializeField] GameObject startingWeapon;
    public GameObject StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }

    [Header("Character Stats")]

    // Determines the maximum amount of HP for the character.
    // Base value = 100 HP
    [SerializeField] float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }

    // Determines how much HP is generated for the character per second.
    // Base value = 0 HP/sec
    [SerializeField] float recovery;
    public float Recovery { get => recovery; private set => recovery = value; }

    // Determines the amount of reduced incoming damage.
    // Base value = 0 Armour
    // Max value = 50 Armour
    [SerializeField] float armour;
    public float Armour { get => armour; private set => armour = value; }

    // Modifies the movement speed of the character.	
    // Base value = 100%
    [SerializeField] float moveSpeed;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }  

    // Determines the radius inside which Experience Gems and Pickups are collected.
    // Base value = 30
    [SerializeField] float magnet;
    public float Magnet { get => magnet; private set => magnet = value; }

    [Header("Passive Stats")]

    // Modifies the chances of certain things, such as the drop chances of most Pickups and the chances of Treasure Chests being of higher quality.
    // Base value = 100%
    [SerializeField] float luck;
    public float Luck { get => luck; private set => luck = value; }

    // Modifies the amount of experience gained from collecting Experience Gems.
    // Base value = 100%
    [SerializeField] float growth;
    public float Growth { get => growth; private set => growth = value; }

    // Modifies the amount of gold gained from Pickups and Treasure Chests.
    // Base value = 100%
    [SerializeField] float greed;
    public float Greed { get => greed; private set => greed = value; }

    // Modifies the enemies speed, health, quantity and frequency.	
    // Base value = 100%
    [SerializeField] float curse;
    public float Curse { get => curse; private set => curse = value; }

    // Determines the amount of Extra Lives the player has.
    // Base value = 0
    [SerializeField] float revival;
    public float Revival { get => revival; private set => revival = value; }

    // Determines how many times the player can reroll level-up rewards.
    // Base value = 0
    [SerializeField] float reroll;
    public float Reroll { get => reroll; private set => reroll = value; }

    // Determines how many times the player can skip level-up rewards.
    // Base value = 0
    [SerializeField] float skip;
    public float Skip { get => skip; private set => skip = value; }

    // Determines how many times the player can banish level-up rewards.
    // Base value = 0
    [SerializeField] float banish;
    public float Banish { get => banish; private set => banish = value; }
}

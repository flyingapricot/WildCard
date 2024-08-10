using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "WildCard/Character Data")]
public class CharacterData : ScriptableObject
{
    [SerializeField] new Sprite name; // We showcase the name as a sprite
    public Sprite Name { get => name; private set => name = value; }

    [SerializeField] Sprite sprite;
    public Sprite Sprite { get => sprite; private set => sprite = value; }

    [SerializeField] RuntimeAnimatorController animation;
    public RuntimeAnimatorController Animation { get => animation; private set => animation = value; }

    [SerializeField] WeaponData startingWeapon;
    public WeaponData StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery, armour;
        [Range(-1, 10)] public float moveSpeed, might, area;
        [Range(-1, 5)] public float projSpeed, duration;
        [Range(-1, 10)] public int amount;
        [Range(-1, 1)] public float cooldown;
        [Min(-1)] public float luck, growth, greed, curse;
        public float magnet;
        public int revive; 
        // Reroll, Skip, Banish

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth; // Determines the maximum amount of HP (1000 HP)
            s1.might += s2.might; // Modifies the damage of all attacks (100% ~ 1000%)
            s1.armour += s2.armour; // Determines the amount of reduced incoming damage (0 ~ 50)
            s1.moveSpeed += s2.moveSpeed; // Modifies the movement speed of the character (100%)
            s1.recovery += s2.recovery; // Determines how much HP is generated per second (0 HP/sec)
            s1.magnet += s2.magnet; // Determines the radius inside which Pickups are collected (30)
            s1.area += s2.area; // Modifies the area of all attacks (100% ~ 1000%)
            s1.amount += s2.amount; // Determines the amount of extra projectiles weapons have (0 ~ 10)
            s1.duration += s2.duration; // Modifies the duration of weapon effects (100% ~ 500%)
            s1.cooldown += s2.cooldown; // Modifies the duration of the cooldown between attacks (100% ~ 10%)
            s1.projSpeed += s2.projSpeed; // Modifies the movement speed of all projectiles (100% ~ 500%)
            s1.luck += s2.luck; // Modifies the chances of Pickups and Events (100%)
            s1.greed += s2.greed; // Modifies the amount of Souls gained (100%)
            s1.curse += s2.curse; // Modifies the enemies speed, health, quantity and frequency (100%)
            s1.growth += s2.growth; // Modifies the amount of experience gained (100%)
            s1.revive += s2.revive; // Determines the amount of Extra Lives (0)
            return s1;
        }
    }

    public Stats stats = new()
    {
        maxHealth = 100, 
        might = 1,
        armour = 0, 
        moveSpeed = 1,
        recovery = 0,
        magnet = 1, 
        area = 1, 
        amount = 0,
        duration = 1, 
        cooldown = 1,
        projSpeed = 1, 
        luck = 1, 
        greed = 1, 
        curse = 1,
        growth = 1, 
        revive = 0, 
    };
}

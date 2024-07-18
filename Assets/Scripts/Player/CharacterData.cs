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
        public float maxhealth, might, armour, recovery, moveSpeed, magnet;

        public Stats(float maxHealth = 1000, float might = 1f, float armour = 0, float recovery = 0, float moveSpeed = 1f, float magnet = 30f)
        {
            this.maxhealth = maxHealth; // Determines the maximum amount of HP for the character. Base value = 1000 HP
            this.might = might; // Modifies the damage of all attacks. Base value = 100%, Max value = 1000%
            this.armour = armour; // Determines the amount of reduced incoming damage. Base value = 0, Max value = 50
            this.recovery = recovery; // Determines how much HP is generated for the character per second. Base value = 0 HP/sec
            this.moveSpeed = moveSpeed; // Modifies the movement speed of the character. Base value = 100%
            this.magnet = magnet; // Determines the radius inside which Experience Gems and Pickups are collected. Base value = 30
        }

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxhealth += s2.maxhealth;
            s1.might += s2.might;
            s1.armour += s2.armour;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.magnet += s2.magnet;
            return s1;
        }
    }
    public Stats stats = new(1000);
}

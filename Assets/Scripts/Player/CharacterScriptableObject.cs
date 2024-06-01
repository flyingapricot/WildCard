using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/Character")]

// Used to create Unique Character Scriptable Objects
// Which are referenced to Character Stats in the Character Prefabs

public class CharacterScriptableObject : ScriptableObject 
{
    [Header("Character Stats")]
    [SerializeField] GameObject startingWeapon;
    public GameObject StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }
    [SerializeField] float moveSpeed;
    public float MoveSpeed { get => moveSpeed; private set => moveSpeed = value; }  
    [SerializeField] float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }
    [SerializeField] float recovery;
    public float Recovery { get => recovery; private set => recovery = value; }
    [SerializeField] float might;
    public float Might { get => might; private set => might = value; }
    [SerializeField] float projectileSpeed;
    public float ProjectileSpeed { get => projectileSpeed; private set => projectileSpeed = value; }
}

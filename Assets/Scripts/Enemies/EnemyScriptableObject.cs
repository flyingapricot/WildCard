using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "ScriptableObjects/Enemy")]

// Used to create Unique Enemy Scriptable Objects
// Which are referenced to Enemy Stats in the Enemy Prefabs

public class EnemyScriptableObject : ScriptableObject // Inheritance
{
    [Header("Enemy Stats")]
    [SerializeField] float damage;
    public float Damage { get => damage; private set => damage = value; }
    [SerializeField] float speed;
    public float Speed { get => speed; private set => speed = value; }  
    [SerializeField] float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }
}

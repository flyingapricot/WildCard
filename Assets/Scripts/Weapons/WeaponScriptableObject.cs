using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]

// Used to create Unique Weapon Scriptable Objects
// Which are referenced to Weapon Behaviours in the Weapon Prefabs

public class WeaponScriptableObject : ScriptableObject // Inheritance
{
    public GameObject prefab;

    [Header("Weapon Stats")]
    [SerializeField] float damage;
    public float Damage { get => damage; private set => damage = value; }
    [SerializeField] float speed;
    public float Speed { get => speed; private set => speed = value; }
    [SerializeField] float cooldownDuration; // Needs to be > destroyAfterSeconds
    public float CooldownDuration { get => cooldownDuration; private set => cooldownDuration = value; }
    [SerializeField] int pierce;
    public int Pierce { get => pierce; private set => pierce = value; }
}

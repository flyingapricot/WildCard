using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base script for all Weapon controllers
// Not Placed anywhere

public class WeaponController : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    float currentCooldown;
    protected PlayerMovement pm;

    // Protected so that its only accessible to child class
    // virtual so that child class can override (cannot be private)
    protected virtual void Start()
    {
        pm = FindObjectOfType<PlayerMovement>();
        currentCooldown = weaponData.Cooldown; // Initialize to prevent weapon from firing immediately
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0f)
        {
            Attack();
        }
    }

    protected virtual void Attack()
    {
        currentCooldown = weaponData.Cooldown;
    }
}

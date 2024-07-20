using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GameObject that is spawned as an effect of a weapon firing: Projectiles, Auras, Pulses
/// </summary>

public abstract class WeaponEffect : MonoBehaviour
{
    [HideInInspector] public PlayerStats player;
    [HideInInspector] public Weapon weapon;

    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}

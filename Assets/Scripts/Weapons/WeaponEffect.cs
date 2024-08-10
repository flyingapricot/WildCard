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

    // Makes it possible to access owner using capital letter as well.
    // This maintains consistency between naming conventions across
    // different classes.
    public PlayerStats Player { get { return player; } }

    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}

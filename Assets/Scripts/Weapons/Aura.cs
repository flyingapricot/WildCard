using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An aura is a damage-over-time effect that applies to a specific area in timed intervals. 
/// It is used to give functionality to Area of Effect melee weapons.
/// </summary>

public class Aura : WeaponEffect
{
    Dictionary<IDamageable, float> affectedTargets = new();
    List<IDamageable> targetsToUnaffect = new();

    void Update()
    {
        Weapon.Stats stats = weapon.GetStats();
        Dictionary<IDamageable, float> affectedTargsCopy = new(affectedTargets);

        // Loop through every target affected by the aura
        // Reduce the cooldown of the aura for the target
        // Once cooldown reaches 0, deal dmg to target
        foreach (KeyValuePair<IDamageable, float> pair in affectedTargsCopy)
        {
            // if (pair.Key == null)
            // {
            //     // Remove null targets
            //     affectedTargets.Remove(pair.Key);
            //     targetsToUnaffect.Remove(pair.Key);
            //     continue;
            // }

            affectedTargets[pair.Key] -= Time.deltaTime;
            if (pair.Value <= 0)
            {
                if (targetsToUnaffect.Contains(pair.Key))
                {
                    // If the target is marked for removal, remove it
                    affectedTargets.Remove(pair.Key);
                    targetsToUnaffect.Remove(pair.Key);
                }
                else
                {
                    // Reset the cooldown and deal damage
                    affectedTargets[pair.Key] = stats.cooldown;
                    if (pair.Key is EnemyStats)
                    {
                        pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                    }
                    else if (pair.Key is BreakableProps)
                    {
                        pair.Key.TakeDamage();
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            // If the target is not yet affected by this aura
            // Add it to our list of affected targets
            if (!affectedTargets.ContainsKey(damageable))
            {
                // Always starts with an interval of 0
                // So that it will get damaged in the next Update() tick
                affectedTargets.Add(damageable, 0);
            }
            else
            {
                if (targetsToUnaffect.Contains(damageable))
                {
                    targetsToUnaffect.Remove(damageable);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            // Do not directly remove the target upon leaving
            // Since we still have to track their cooldowns
            if (!affectedTargets.ContainsKey(damageable))
            {
                targetsToUnaffect.Add(damageable);
            }
        }
    }
}

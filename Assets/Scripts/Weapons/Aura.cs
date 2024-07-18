using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An aura is a damage-over-time effect that applies to a specific area in timed intervals. 
/// It is used to give functionality to Area of Effect melee weapons.
/// </summary>

public class Aura : WeaponEffect
{
    Dictionary<EnemyStats, float> affectedTargets = new();
    List<EnemyStats> targetsToUnaffect = new();

    void Update()
    {
        Dictionary<EnemyStats, float> affectedTargsCopy = new(affectedTargets);

        // Loop through every target affected by the aura
        // Reduce the cooldown of the aura for the target
        // Once cooldown reaches 0, deal dmg to target
        foreach (KeyValuePair<EnemyStats, float> pair in affectedTargsCopy)
        {
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
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.TryGetComponent(out EnemyStats es))
        {
            // If the target is not yet affected by this aura
            // Add it to our list of affected targets
            if (!affectedTargets.ContainsKey(es))
            {
                // Always starts with an interval of 0
                // So that it will get damaged in the next Update() tick
                affectedTargets.Add(es, 0);
            }
            else
            {
                if (targetsToUnaffect.Contains(es))
                {
                    targetsToUnaffect.Remove(es);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if (other.TryGetComponent(out EnemyStats es))
        {
            // Do not directly remove the target upon leaving
            // Since we still have to track their cooldowns
            if (!affectedTargets.ContainsKey(es))
            {
                targetsToUnaffect.Add(es);
            }
        }
    }
}

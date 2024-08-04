using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon // Inheritance
{
    protected float currentAttackInterval; // Time between each projectile spawned
    protected int currentAttackCount; // Number of times this attack will happen

    protected override void Update()
    {
        base.Update(); // Attack when cooldown <= 0

        // If weapon is not finished with its attack, wait until interval <= 0 to continue attack
        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true; // Continue attacking if not done shooting all projectiles
        return base.CanAttack(); // Continue attacking if cooldown <= 0
    }

    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, leave a warning message
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            currentCooldown = data.baseStats.cooldown;
            return false; // To check if attack is successful or not
        }

        // Can we attack?
        if (!CanAttack()) return false;

        // If yes, calculate the angle and offset of our spawned projectile
        float spawnAngle = GetSpawnAngle();

        // Spawn a copy of the projectile
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            player.transform.position + (Vector3)GetSpawnOffset(spawnAngle), // Spawn point = player position + randomized offset
            Quaternion.Euler(0, 0, spawnAngle) 
        );
        
        prefab.weapon = this;
        prefab.player = player;

        // Reset the cooldown only if this attack was triggered by cooldown
        if (currentCooldown <= 0)
            currentCooldown += currentStats.cooldown;

        attackCount--;

        // Do we perform another attack?
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }

    // Gets which direction projectile should face when spawned
    protected virtual float GetSpawnAngle()
    {
        return Mathf.Atan2(movement.lastVerticalVector, movement.lastHorizontalVector) * Mathf.Rad2Deg;
    }

    // Generates a random point in a rect to spawn the projectile 
    // Rotates the facing of the point by spawnAngle
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2( 
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}

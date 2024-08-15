using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : ProjectileWeapon
{
    int currentSpawnCount; // How many times the sword has been attacking in this iteration
    float currentSpawnYOffset; // If there are more than 2 swords, we will start offsetting it upwards

    protected override void Update()
    {
        base.Update();
        transform.position = player.transform.position;
    }

    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, leave a warning message
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning(string.Format("Projectile prefab has not been set for {0}", name));
            ActivateCooldown(true);
            return false; // To check if attack is successful or not
        }

        // If there is no projectile assigned, set the weapon on cooldown
        if (!CanAttack()) return false;

        // If this is the first time the attack has been fired
        // We reset the currentSpawnCount
        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0f;
        }

        // Otherwise calculate the angle and offset of our spawned projectile
        // If <currentSpawnCount> has an even value
        // We will flip the direction of the spawn
        float spawnDir = Mathf.Sign(movement.lastHorizontalVector) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new(spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax), currentSpawnYOffset);

        // Spawn a copy of the projectile
        Projectile prefab = Instantiate(currentStats.projectilePrefab, player.transform.position + (Vector3)spawnOffset, Quaternion.identity);
        prefab.player = player; // Set player as spawn point
        // Set the projectile as a child of the player
        prefab.transform.SetParent(transform);

        // Flip the projectile's sprite
        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(-Mathf.Abs(prefab.transform.localScale.x), prefab.transform.localScale.y, prefab.transform.localScale.z);
            //Debug.Log(spawnDir + " | " + prefab.transform.localScale);
        }

        // Assign the stats
        prefab.weapon = this;
        ActivateCooldown(true);
        attackCount--;

        // Determine where the next projectile should spawn
        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
            currentSpawnYOffset += 1;

        // Do we perform another attack?
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = currentStats.projectileInterval;
        }

        return true;
    }
}

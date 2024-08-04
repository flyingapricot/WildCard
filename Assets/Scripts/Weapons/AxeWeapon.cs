using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeWeapon : ProjectileWeapon
{
    // Gets which direction projectile should face when spawned
    protected override float GetSpawnAngle()
    {
        // int offset = currentAttackCount > 0 ? currentStats.amount - currentAttackCount : 0;
        // return 90f - Mathf.Sign(movement.lastMovementVector.x) * (5 * offset);
        return Mathf.Atan2(movement.lastVerticalVector, movement.lastHorizontalVector) * Mathf.Rad2Deg;
    }

    // Generates a random point in a rect to spawn the projectile 
    // Rotates the facing of the point by spawnAngle
    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return new Vector2( 
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}

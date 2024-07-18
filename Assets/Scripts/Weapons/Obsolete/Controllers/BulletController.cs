using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This will be replaced by the Weapon class.")]
// To be placed in Projectile Weapon GameObject

public class BulletController : WeaponController // Inheritance
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedBullet = Instantiate(weaponData.prefab);
        // Assigns position of bullet to be same as Player
        spawnedBullet.transform.position = transform.position;
        // Reference Player to set direction of bullet
        spawnedBullet.GetComponent<BulletBehaviour>().DirectionChecker(pm.lastMovementVector);
        // Organize bullets into WeaponBullet GameObject
        spawnedBullet.transform.parent = transform; 
    }
}

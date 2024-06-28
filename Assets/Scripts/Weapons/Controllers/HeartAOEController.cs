using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed in Melee Weapon GameObject

public class HeartAOEController : WeaponController // Inheritance
{
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedHeart = Instantiate(weaponData.prefab);
        // Assigns position of Heart to be same as Player
        spawnedHeart.transform.position = transform.position;
        // Organize Heart into WeaponHeartAOE GameObject
        spawnedHeart.transform.parent = transform; 
    }
}

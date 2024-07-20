using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This will be replaced by the Weapon class.")]
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

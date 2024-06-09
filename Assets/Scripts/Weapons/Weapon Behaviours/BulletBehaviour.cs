using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed in Projectile Weapon Prefabs

public class BulletBehaviour : ProjectileBehaviour // Inheritence
{
    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        transform.position += direction * currentSpeed * Time.deltaTime; // Set movement of bullet
    }
}

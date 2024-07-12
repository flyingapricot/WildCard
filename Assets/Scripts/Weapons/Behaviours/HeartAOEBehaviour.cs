using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed in Melee Weapon Prefabs

public class HeartAOEBehaviour : MeleeBehaviour // Inheritence
{
    List<GameObject> markedEnemies; // List of enemies that is already damaged

    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position, currentKnockback);

            markedEnemies.Add(col.gameObject); // Mark the Enemy so that you cannot spam damage on the same enemy
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                markedEnemies.Add(col.gameObject);
            }
        }
    }
}

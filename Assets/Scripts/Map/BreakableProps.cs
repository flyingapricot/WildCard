using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableProps : MonoBehaviour
{
    public float durability;

    public void TakeDamage(float dmg)
    {
        {
            durability -= dmg;

            if (durability <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealDrop : MonoBehaviour, ICollectible
{
    public int healValue;
    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.Heal(healValue);
        Destroy(gameObject); // Destroys once collected

        // throw new System.NotImplementedException();
    }
    
    /* void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroys once collected
        }
    }*/
}

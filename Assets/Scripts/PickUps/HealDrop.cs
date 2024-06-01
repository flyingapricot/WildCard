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
        Destroy(gameObject);

        // throw new System.NotImplementedException();
    }
}

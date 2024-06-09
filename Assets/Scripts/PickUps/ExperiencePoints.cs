using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperiencePoints : MonoBehaviour, ICollectible
{
    public int experienceValue;
    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.GainExperience(experienceValue);
        Destroy(gameObject); // Destroys once collected

        //throw new System.NotImplementedException();
    }

    /* void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Destroy(gameObject); // Destroys once collected
        }
    } */
}

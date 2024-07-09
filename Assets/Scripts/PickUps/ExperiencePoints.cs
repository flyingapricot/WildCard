using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperiencePoints : MonoBehaviour, ICollectible
{
    public bool Collected { get; set; } = false;
    public int minExperienceValue; // Minimum experience value
    public int maxExperienceValue; // Maximum experience value

    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        // Assign a random experience value within the range
        player.GainExperience(Random.Range(minExperienceValue, maxExperienceValue + 1));
        // Trigger flag to stop bobbing animation
        Collected = true;
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

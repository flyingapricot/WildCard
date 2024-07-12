using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantXP : MonoBehaviour, ICollectible
{
    public bool Collected { get; set; } = false;

    public void Collect()
    {
        PlayerStats player = FindObjectOfType<PlayerStats>();
        // Assign a random experience value within the range
        player.GainExperience(player.experienceCap);
        // Trigger flag to stop bobbing animation
        Collected = true;
        Destroy(gameObject); // Destroys once collected
    }
}

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
        Destroy(gameObject);

        //throw new System.NotImplementedException();
    }
}

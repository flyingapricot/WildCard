using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRateManager : MonoBehaviour
{
    [System.Serializable]
    public class Drops
    {
        // public string name;
        public GameObject itemPrefab; // To be destroyed when picked up by player
        public float dropRate; // Chance in % for item to drop (MAX 100)
    }
    public bool active = false;
    public bool singular = false; // Whether will drop only 1 item or multiple
    public List<Drops> dropsList;

    void OnDestroy() // When GameObject is destroyed
    {
        // Stops the enemies from dropping items when despawned
        if (!active) return;
        // Stops the enemies from dropping items when destroyed while exiting play mode
        if (!gameObject.scene.isLoaded) return;

        // New list for unique GameObjects to track multiple drops
        List<Drops> possibleDrops = new();
        // Increase drop rate by decreasing the number pool
        float rng = Random.Range(0f, 100f - PlayerStats.instance.Actual.luck);

        foreach (Drops drop in dropsList)
        {
            if (rng <= drop.dropRate) // rng/dropRate % chance to drop item
            {
                if (!singular)
                {
                    Instantiate(drop.itemPrefab, transform.position, Quaternion.identity); // Spawn all drops
                }
                else
                {
                    possibleDrops.Add(drop); // Multiple drops may have the same chance of dropping
                }
            }
        }

        if (possibleDrops.Count > 0) 
        {
            Drops chosenDrop = possibleDrops[Random.Range(0, possibleDrops.Count)]; // Choose only 1 of the possible drops
            Instantiate(chosenDrop.itemPrefab, transform.position, Quaternion.identity); // Spawn chosen drop
        }
    }
}

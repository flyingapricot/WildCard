using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        // Checks if the Drops GameObjects have the ICollectible Interface
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            collectible.Collect(); // If yes, collect it
        }
    }
}

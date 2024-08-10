using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]

public class PlayerCollector : MonoBehaviour
{
    PlayerStats player;
    CircleCollider2D detector;
    public float pullSpeed; // Speed of item moving towards the player
    //public float collectionDistance; // Distance threshold to consider the item collected

    void Start()
    {
        player = GetComponentInParent<PlayerStats>();
    }

    public void SetRadius(float r)
    {
        if(!detector) detector = GetComponent<CircleCollider2D>();
        detector.radius = r;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Check if the other GameObject is a Pickup.
        if (col.TryGetComponent(out Pickup p))
        {
            p.Collect(player, pullSpeed);
        }
    }

    // void OnTriggerEnter2D(Collider2D col)
    // {
    //     // Checks if the Drops GameObjects have the ICollectible Interface
    //     if (col.gameObject.TryGetComponent(out ICollectible collectible))
    //     {
    //         // Stop bobbing animation if it exists
    //         if (col.gameObject.TryGetComponent(out BobbingAnimation bobbingAnimation))
    //         {
    //             bobbingAnimation.StopBobbing();
    //         }

    //         // Start the coroutine to pull and collect the item
    //         StartCoroutine(PullAndCollect(col.gameObject, collectible));
    //     }
    // }

    // IEnumerator PullAndCollect(GameObject item, ICollectible collectible)
    // {
    //     while (Vector2.Distance(transform.position, item.transform.position) > collectionDistance)
    //     {
    //         // Move the item towards the player
    //         item.transform.position = Vector2.MoveTowards(item.transform.position, transform.position, pullSpeed * Time.deltaTime);
    //         yield return null; // Wait for the next frame
    //     }

    //     // Once the item is close enough to the player, collect it
    //     collectible.Collect();
    // }
}

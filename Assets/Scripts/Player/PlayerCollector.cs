using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollector : MonoBehaviour
{
    PlayerStats stats;
    CircleCollider2D playerCollector;
    public float pullSpeed; // Speed of item moving towards the player
    public float collectionDistance; // Distance threshold to consider the item collected

    void Start()
    {
        stats = FindObjectOfType<PlayerStats>();
        playerCollector = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        playerCollector.radius = stats.CurrentMagnet; // Update collection radius
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        // Checks if the Drops GameObjects have the ICollectible Interface
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            // Start the coroutine to pull and collect the item
            StartCoroutine(PullAndCollect(col.gameObject, collectible));
        }
    }

    IEnumerator PullAndCollect(GameObject item, ICollectible collectible)
    {
        while (Vector2.Distance(transform.position, item.transform.position) > collectionDistance)
        {
            // Move the item towards the player
            item.transform.position = Vector2.MoveTowards(item.transform.position, transform.position, pullSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        // Once the item is close enough to the player, collect it
        collectible.Collect();
    }

    /* void OnTriggerEnter2D(Collider2D col)
    {
        // Checks if the Drops GameObjects have the ICollectible Interface
        if (col.gameObject.TryGetComponent(out ICollectible collectible))
        {
            // Pulls the drops towards the player (Animation)
            Rigidbody2D rb = col.gameObject.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (transform.position - col.transform.position).normalized;
            rb.AddForce(forceDirection * pullSpeed);

            collectible.Collect(); // If yes, collect it
        }
    } */
}

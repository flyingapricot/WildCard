using System.Collections;
using UnityEngine;

public class KamikazeBehaviour : EnemyMovement
{
    [Header("Explosion")]
    public GameObject explosionEffect;         
    public GameObject radiusIndicator; // Prefab for explosion radius circle
    public float explosionRadius = 3.0f; 
    public float triggerDistance = 1.5f; // Distance from the player to trigger explosion
    public float explosionDelay = 2.0f; // Time before explosion after triggering
    private bool isExploding = false;

    [Header("Cluster Spawn")]
    public bool isCluster; // Check for cluster bomb spawning
    public GameObject spawns; // Prefab of enemy to spawn when killed
    [Min(0)] public float spawnAmount = 5f, spawnRadius = 2f;

    protected override void Update()
    {
        base.Update();
        if (transform == null) { return; }
        // Check if within triggering distance
        if (!isExploding && Vector2.Distance(transform.position, player.position) <= triggerDistance)
        {
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        isExploding = true;
        // When exploding, slow down enemy
        stats.baseStats.moveSpeed /= 2;

        // Set the radius of the indicator to match the explosion radius
        radiusIndicator.transform.localScale = new Vector3(explosionRadius * 5, explosionRadius * 5, 1);
        GameObject radius = Instantiate(radiusIndicator, transform.position, Quaternion.identity, transform);

        yield return new WaitForSeconds(explosionDelay);

        // Check if the player is within the explosion radius
        CircleCollider2D radiusCollider = radius.GetComponent<CircleCollider2D>();
        if (radiusCollider.OverlapPoint(player.position))
        {
            // Deal damage to the player
            PlayerStats.instance.TakeDamage(stats.Actual.damage);
        }

        // Destroy the radius indicator
        Destroy(radius);

        // Trigger explosion
        if (explosionEffect)
        {
            explosionEffect.transform.localScale = new Vector3(explosionRadius * 5, explosionRadius * 5, 1);
            Destroy(Instantiate(explosionEffect, transform.position, Quaternion.identity), 0.1f);
        }

        if (isCluster) // Spawn cluster enemies around transform
        {
            ClusterSpawn();
        }

        stats.Kill(); // Destroy Enemy
    }

    void ClusterSpawn()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            // Generate a random angle between 0 and 360 degrees
            float angle = Random.Range(0f, 360f);
            
            // Convert the angle to radians and calculate the x and y coordinates
            float spawnX = transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * spawnRadius;
            float spawnY = transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * spawnRadius;
            
            // Create the spawn position
            Vector2 spawnPosition = new(spawnX, spawnY);
            
            // Instantiate the object at the calculated position
            Instantiate(spawns, spawnPosition, Quaternion.identity);
        }
    }
}

using System.Collections;
using UnityEngine;

public class KamikazeBehaviour : MonoBehaviour
{
    private EnemyStats stats;
    private Transform player;
    public GameObject spawns; // Prefab of enemy to spawn when killed
    public bool isCluster; // Check for cluster bomb spawning

    public GameObject explosionEffect;         
    public GameObject explosionRadiusIndicator; // Prefab for explosion radius circle
    private GameObject radiusIndicator;
    public float explosionRadius = 3.0f;      
    public float triggerDistance = 1.5f; // Distance from the player to trigger explosion
    public float explosionDelay = 2.0f; // Time before explosion after triggering
    private bool isExploding = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isExploding) return;

        // Move towards the player
        transform.position = Vector2.MoveTowards(transform.position, player.position, stats.Actual.moveSpeed * Time.deltaTime);

        // Check if within triggering distance
        if (Vector2.Distance(transform.position, player.position) <= triggerDistance)
        {
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        isExploding = true;

        // Show explosion radius
        radiusIndicator = Instantiate(explosionRadiusIndicator, transform.position, Quaternion.identity);
        radiusIndicator.transform.localScale = new Vector3(explosionRadius * 2, explosionRadius * 2, 1);

        yield return new WaitForSeconds(explosionDelay);

        // Trigger explosion
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Destroy enemy and radius indicator
        Destroy(radiusIndicator);
        Destroy(gameObject);

        // Logic to deal damage to player or other entities within explosionRadius can be added here
    }
}

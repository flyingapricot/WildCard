using System.Collections;
using UnityEngine;

public class LazerShooting : EnemyMovement
{
    [Header("Lazer Beam")]
    public GameObject lazerIndicator; // Prefab for laser indicator
    public GameObject lazerBeam; // Prefab for lazer beam
    public float indicatorDuration = 3f; // Duration the indicator is shown before the laser fires
    public float lazerDuration = 3f; // Duration of lazer beam
    public float lazerCooldown = 2f; // Time between lazer shootings
    private float lazerTimer = 2f; // Timer to track cooldown
    private bool isShooting = false;
    private Vector3 directionToPlayer;
    private Quaternion rotation;

    protected override void Update()
    {
        base.Update();

        // Countdown the delay timer
        if (lazerTimer > 0)
        {
            lazerTimer -= Time.deltaTime;
        }

        // If not currently shooting and the cooldown has passed, shoot the laser
        if (!isShooting && lazerTimer <= 0)
        {
            StartCoroutine(ShootLazer());
        }
    }

    IEnumerator ShootLazer()
    {
        isShooting = true;
        // Save the original move speed
        float speed = stats.actualStats.moveSpeed;

        // Instantiate the laser indicator
        GameObject indicator = Instantiate(lazerIndicator, transform.position, Quaternion.identity, transform);

        float indicatorElapsed = 0f;
        while (indicatorElapsed < indicatorDuration / 2)
        {
            // Continuously aim the indicator at the player
            directionToPlayer = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            rotation = Quaternion.Euler(0, 0, angle - 90f); // Adjust for laser orientation (bottom to top)

            // Rotate and position the laser based on the player's position
            indicator.transform.SetPositionAndRotation(transform.position + directionToPlayer * indicator.GetComponent<SpriteRenderer>().bounds.size.y, rotation);

            indicatorElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        // Stop and Destroy the indicator before firing the laser
        yield return new WaitForSeconds(indicatorDuration / 2);
        Destroy(indicator);

        // Stop enemy movement when shooting lazer
        stats.actualStats.moveSpeed = 0;

        // Instantiate the laser beam and adjust its position similarly
        GameObject lazer = Instantiate(lazerBeam, transform.position, Quaternion.identity, transform);
        lazer.transform.SetPositionAndRotation(transform.position + directionToPlayer * lazer.GetComponent<SpriteRenderer>().bounds.size.y, rotation);

        // Continuously check for player collision during the laser's duration
        BoxCollider2D beamCollider = lazer.GetComponent<BoxCollider2D>();
        float elapsedTime = 0f;
        while (elapsedTime < lazerDuration)
        {
            if (beamCollider.OverlapPoint(player.position))
            {
                // Deal damage to the player
                PlayerStats.instance.TakeDamage(stats.Actual.damage);
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        // Destroy the lazer beam
        Destroy(lazer);

        // Reset cooldown timer and revert enemy movement
        stats.actualStats.moveSpeed = speed;
        lazerTimer = lazerCooldown;

        isShooting = false;
    }
}
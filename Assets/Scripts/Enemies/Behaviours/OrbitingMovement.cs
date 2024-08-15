using UnityEngine;

public class OrbitingMovement : EnemyMovement
{
    [Header("Orbital")]
    public float minDistance = 2f; // Minimum distance from the player
    public float maxDistance = 5f; // Maximum allowed distance from the player
    public float orbitSpeed = 5f; // Speed of orbiting
    public float elasticity = 2f; // How quickly the enemy catches up to the orbit position
    private float currentAngle = 0f; // Current angle for orbiting

    public override void Move()
    {
        if (player == null) return;

        // Calculate the current distance from the player
        float distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);

        // Clamp the distance to stay within the min and max bounds
        float clampedDistance = Mathf.Clamp(distanceFromPlayer, minDistance, maxDistance);

        // Adjust the orbit speed based on the distance (further away = faster)
        float adjustedSpeed = orbitSpeed * (distanceFromPlayer / clampedDistance);

        // Update the current angle for clockwise rotation
        currentAngle -= adjustedSpeed * Time.deltaTime;

        // Calculate the new position using the current angle and clamped distance
        Vector2 offset = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)) * clampedDistance;
        Vector2 targetPosition = (Vector2)player.transform.position + offset;

        // Move towards the target orbit position with elasticity
        transform.position = Vector2.Lerp(transform.position, targetPosition, elasticity * Time.deltaTime);

        // Flip the sprite based on the direction
        sprite.flipX = offset.x > 0; // Flip when moving right
    }
}

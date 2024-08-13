using UnityEngine;

public class OrbitingMovement : EnemyMovement
{    
    private Vector3 orbitPosition;
    public float elasticity = 2f; // How quickly the enemy catches up to the player
    public float minDistance = 1f; // Minimum distance from the player
    public float maxDistance = 5f; // Maximum distance from the player

    public override void Move()
    {
        Vector2 moveDirection = (player.transform.position - transform.position).normalized;
        if (moveDirection.x != 0) // Flip the sprite based on the horizontal direction
        {
            sprite.flipX = moveDirection.x > 0; // Flip when moving right since default is left
        }

        // Calculate desired orbiting position based on player's movement
        float distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);
        float targetDistance = Mathf.Clamp(distanceFromPlayer, minDistance, maxDistance);

        orbitPosition = (Vector2)player.transform.position + (moveDirection * targetDistance);

        // Rotate around the player with elasticity
        transform.position = Vector2.Lerp(transform.position, orbitPosition, elasticity * Time.deltaTime);

        // Determine rotation direction: clockwise or anticlockwise
        float rotationDirection = moveDirection.x > 0 ? 1f : -1f;

        // Orbit around the player
        transform.RotateAround(player.position, Vector3.forward, rotationDirection * stats.Actual.moveSpeed * Time.deltaTime);
    }
}

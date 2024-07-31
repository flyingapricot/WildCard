using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed on Enemy Prefabs

public class DashBehaviour : MonoBehaviour
{
    EnemyStats stats;
    SpriteRenderer sprite;
    Vector2 knockbackVelocity;
    float knockbackDuration;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is >0
        if (knockbackDuration > 0) return;
        
        // Begin the knockback
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

    private void FixedUpdate()
    {
        if (PlayerStats.instance != null)
        {
            if (knockbackDuration > 0) // Currently being knockedback
            {
                transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
                knockbackDuration -= Time.deltaTime;
            }
            else // Otherwise, Move the enemy towards player
            {
                Vector3 targetPosition = PlayerStats.instance.transform.position; // Player is target destination
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, stats.currentSpeed * 2 * Time.fixedDeltaTime);

                Vector2 moveDirection = (targetPosition - transform.position).normalized;
                if (moveDirection.x != 0) // Flip the sprite based on the horizontal direction
                {
                    sprite.flipX = moveDirection.x > 0; // Flip when moving right
                }
            }
        }
    }
}

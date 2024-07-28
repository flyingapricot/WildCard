using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBehaviour : MonoBehaviour
{
    EnemyStats stats;
    SpriteRenderer sprite;
    Vector2 knockbackVelocity;
    public Rigidbody2D rb;
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
                rb.AddForce(new Vector2(PlayerStats.instance.transform.position.x - transform.position.x, 10),ForceMode2D.Force);

                Vector3 targetPosition = PlayerStats.instance.transform.position; // Player is target destination
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, stats.currentSpeed * 1 * Time.fixedDeltaTime);

                Vector2 moveDirection = (targetPosition - transform.position).normalized;
                if (moveDirection.x != 0) // Flip the sprite based on the horizontal direction
                {
                    sprite.flipX = moveDirection.x > 0; // Flip when moving right
                }
            }
        }
    }
}

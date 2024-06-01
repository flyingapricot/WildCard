using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed on Enemy Prefabs

public class EnemyMovement : MonoBehaviour
{
    EnemyStats stats;
    Rigidbody2D rgbd2D;
    SpriteRenderer sprite;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        rgbd2D = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject
    }

    private void FixedUpdate()
    {
        if (PlayerStats.instance != null)
        {
            Vector3 targetPosition = PlayerStats.instance.transform.position; // Player is target destination
            // Move the enemy towards player
            rgbd2D.position = Vector3.MoveTowards(rgbd2D.position, targetPosition, stats.currentSpeed * Time.fixedDeltaTime);

            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            // Flip the sprite based on the horizontal direction
            if (moveDirection.x != 0)
            {
                sprite.flipX = moveDirection.x > 0; // Flip when moving right
            }
        }
    }
}

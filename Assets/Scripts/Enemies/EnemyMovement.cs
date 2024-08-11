using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed on Enemy Prefabs

public class EnemyMovement : MonoBehaviour
{
    protected EnemyStats stats;
    protected Transform player;
    protected SpriteRenderer sprite;
    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;

    public enum OutOfFrameAction { none, respawnAtEdge, despawn }
    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnAtEdge;
    protected bool spawnedOutOfFrame = false;

    protected virtual void Start()
    {
        spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);
        stats = GetComponent<EnemyStats>();
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject

        // Picks a random player on the screen, instead of always picking the 1st player.
        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();
        player = allPlayers[Random.Range(0, allPlayers.Length)].transform;
    }

    protected virtual void Update()
    {
        // If we are currently being knocked back, then process the knockback.
        if(knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else // Otherwise, Move the enemy towards player
        {
            Move();
            HandleOutOfFrameAction();
        }
    }

    // If the enemy falls outside of the frame, handle it.
    protected virtual void HandleOutOfFrameAction() {
        // Handle the enemy when it is out of frame.
        if (!SpawnManager.IsWithinBoundaries(transform))
        {
            switch(outOfFrameAction)
            {
                case OutOfFrameAction.none: default:
                    break;
                case OutOfFrameAction.respawnAtEdge:
                    // If the enemy is outside the camera frame, teleport it back to the edge of the frame.
                    transform.position = SpawnManager.GeneratePosition();
                    break;
                case OutOfFrameAction.despawn:
                    // Don't destroy if it is spawned outside the frame.
                    if (!spawnedOutOfFrame)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        } else spawnedOutOfFrame = false;
    }

    public virtual void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is >0
        if (knockbackDuration > 0) return;
        
        // Begin the knockback
        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }

    public virtual void Move()
    {
        // Constantly move the enemy towards the player
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, stats.currentSpeed * Time.deltaTime);

        Vector2 moveDirection = (player.transform.position - transform.position).normalized;
        if (moveDirection.x != 0) // Flip the sprite based on the horizontal direction
        {
            sprite.flipX = moveDirection.x > 0; // Flip when moving right since default is left
        }
    }
}
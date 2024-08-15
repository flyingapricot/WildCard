using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To be placed on Enemy Prefabs

public class EnemyMovement : MonoBehaviour
{
    protected EnemyStats stats;
    protected Transform player;
    protected Rigidbody2D rb; // For checking if enemy has a rigidbody.
    protected SpriteRenderer sprite;
    public const float DEFAULT_MOVESPEED = 5f;

    protected Vector2 knockbackVelocity;
    protected float knockbackDuration;
    [System.Flags]
    public enum KnockbackVariance { duration = 1, velocity = 2 }
    public KnockbackVariance knockbackVariance = KnockbackVariance.velocity;

    public enum OutOfFrameAction { none, respawnAtEdge, despawn }
    public OutOfFrameAction outOfFrameAction = OutOfFrameAction.respawnAtEdge;
    protected bool spawnedOutOfFrame = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<EnemyStats>();
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject
        spawnedOutOfFrame = !SpawnManager.IsWithinBoundaries(transform);

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
        
        // Ignore knockback if the knockback type is set to none.
        if (knockbackVariance == 0) return;

        // Only change the factor if the multiplier is not 0 or 1.
        float pow = 1;
        bool reducesVelocity = (knockbackVariance & KnockbackVariance.velocity) > 0,
            reducesDuration = (knockbackVariance & KnockbackVariance.duration) > 0;

        if (reducesVelocity && reducesDuration) pow = 0.5f;

        // Check which knockback values to affect.
        knockbackVelocity = velocity * (reducesVelocity ? Mathf.Pow(stats.Actual.knockbackMultiplier, pow) : 1);
        knockbackDuration = duration * (reducesDuration ? Mathf.Pow(stats.Actual.knockbackMultiplier, pow) : 1);
    }

    public virtual void Move()
    {
        // If Player is missing, return to avoid accessing a destroyed object
        if (player == null) return;
    
        // If there is a rigidbody, use it to move instead of moving the position directly.
        // This optimises performance.
        if(rb)
        {
            rb.MovePosition(Vector2.MoveTowards(
                rb.position,
                player.transform.position,
                DEFAULT_MOVESPEED * stats.Actual.moveSpeed * Time.deltaTime)
            );
        } 
        else
        {
            // Constantly move the enemy towards the player
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position, 
                DEFAULT_MOVESPEED * stats.Actual.moveSpeed * Time.deltaTime
            );
        }

        Vector2 moveDirection = (player.transform.position - transform.position).normalized;
        if (moveDirection.x != 0) // Flip the sprite based on the horizontal direction
        {
            sprite.flipX = moveDirection.x > 0; // Flip when moving right since default is left
        }
    }
}
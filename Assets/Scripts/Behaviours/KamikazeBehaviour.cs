using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamikazeBehaviour : MonoBehaviour
{
    EnemyStats stats;
    SpriteRenderer sprite;
    Vector2 knockbackVelocity;
    float knockbackDuration;
    public LayerMask LayerToHit;
    public float fieldofImpact;
    public float force;
    public GameObject ExplosionEffect;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject
        ExplosionEffect.SetActive(false);
    }

    public void Knockback(Vector2 velocity, float duration)
    {
        // Ignore the knockback if the duration is >0
        if (knockbackDuration > 0) return;

        // Begin the knockback
        knockbackVelocity = velocity;
        knockbackDuration = duration;

    }

    private void explode()
    {
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, fieldofImpact, LayerToHit);
        
        foreach(Collider2D obj in objects)
        {
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = obj.transform.position - transform.position;
                rb.AddForce(direction * force);
            }

        }
        Destroy(gameObject);
        GameObject ExplosionEffectIns = Instantiate(ExplosionEffect, transform.position, Quaternion.identity);
        Destroy(ExplosionEffectIns, 10);
        //Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,fieldofImpact);
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
                transform.position = Vector2.MoveTowards(transform.position, targetPosition, stats.currentSpeed * Time.fixedDeltaTime);

                //If within the range of the player, explode and reduce player health
                if(Mathf.Abs(targetPosition.x - transform.position.x) < 1.5)
                {
                    explode();
                    PlayerStats.instance.CurrentHealth -= 5;
                    Destroy(gameObject);
                }

                Vector2 moveDirection = (targetPosition - transform.position).normalized;
                if (moveDirection.x != 0) // Flip the sprite based on the horizontal direction
                {
                    sprite.flipX = moveDirection.x > 0; // Flip when moving right
                }
            }
        }
    }
}

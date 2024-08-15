using UnityEngine;
using System.Linq;

/// <summary>
/// Component that you attach to all bullet prefabs. 
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public EnemyStats stats;
    protected Rigidbody2D rb;
    public float projSpeed = 1f;
    public float lifespan = 5f;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destroy the projectile after its lifespan expires
        if (lifespan > 0) Destroy(gameObject, lifespan);
    }

    protected virtual void FixedUpdate()
    {
        // Only drive movement ourselves if this is a kinematic
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            transform.position += projSpeed * Time.fixedDeltaTime * transform.right;
            rb.MovePosition(transform.position);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerStats player = other.GetComponent<PlayerStats>();

        if (other.CompareTag("Player")) // Check if player
        {
            // Deals the damage
            player.TakeDamage(stats.Actual.damage);
        }
    }
}

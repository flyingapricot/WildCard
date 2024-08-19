using UnityEngine;
using System.Collections;

/// <summary>
/// Component that you attach to all enemy bullet prefabs. 
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public EnemyStats stats;
    protected Rigidbody2D rb;
    public float damage = 10f;
    public float projSpeed = 1f;
    public float lifespan = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Destroy the projectile after its lifespan expires
        if (lifespan > 0) Destroy(gameObject, lifespan);
    }

    void FixedUpdate()
    {
        // Only drive movement ourselves if this is a kinematic Rigidbody2D
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            // Move the bullet in the direction it's facing
            rb.MovePosition(rb.position + projSpeed * Time.fixedDeltaTime * (Vector2)transform.right);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Alternative approach using triggers
        if (other.TryGetComponent(out PlayerStats player))
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}

using UnityEngine;
using System.Linq;

/// <summary>
/// Component that you attach to all projectile prefabs. 
/// ALl Spawned projectiles will fly in the direction they are facing and deal damage when they hit an object.
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect // Inheritance
{
    public enum DamageSource { projectile, player }; // Used to determine knockback source
    public DamageSource damageSource = DamageSource.projectile;
    public bool hasAutoAim = false;
    public float arcSpeed; // Speed at which the projectile travels in an arc
    protected Rigidbody2D rb;
    protected int pierce;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Weapon.Stats stats = weapon.GetStats();

        // Set up initial velocity if dynamic
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = arcSpeed;
            rb.velocity = transform.right * stats.speed;
        }

        // Prevent the area from being 0, as it hides the projectile
        float area = stats.area == 0 ? 1 : stats.area;
        transform.localScale = new Vector3(
            area * Mathf.Sign(transform.localScale.x),
            area * Mathf.Sign(transform.localScale.y), 1
        );

        // Set how much pierce this object has
        pierce = stats.pierce;

        // Destroy the projectile after its lifespan expires
        if (stats.lifespan > 0) Destroy(gameObject, stats.lifespan);

        // If the projectile is auto-aiming, automatically find a suitable enemy
        if (hasAutoAim) AcquireAutoAimFacing();
    }
        
    //If the projectile is homing, it will automatically find a suitable target to move towards
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle; // We need to determine where to aim

        // Find all enemies on the screen
        EnemyStats[] allTargets = FindObjectsOfType<EnemyStats>();

        // Filter out only the targets that are visible
        EnemyStats[] visibleTargets = allTargets.Where(target => target.GetComponentInChildren<Renderer>().isVisible).ToArray();

        // Select a random visible enemy (if there is at least one)
        // Otherwise, pick a random angle to shoot at
        if (visibleTargets.Length > 0)
        {
            EnemyStats selectedTarget = visibleTargets[Random.Range(0, visibleTargets.Length)];
            Vector2 difference = selectedTarget.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        // Point the projectile towards where we are aiming at
        transform.rotation = Quaternion.Euler(0, 0, aimAngle);
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        Weapon.Stats stats = weapon.GetStats();

        // Only drive movement ourselves if this is a kinematic
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            transform.position += stats.speed * Time.fixedDeltaTime * transform.right;
            rb.MovePosition(transform.position);
            // Move the projectile around an arc
            transform.Rotate(0, 0, arcSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyStats es = other.GetComponent<EnemyStats>();
        BreakableProps p = other.GetComponent<BreakableProps>();

        // Only collide with enemies or breakable props
        if (es) // Enemy
        {
            // If there is a player, and the damage source is set to player,
            // We will calculate knockback using the player instead of projectile
            Vector3 source = damageSource == DamageSource.player && player ? player.transform.position : transform.position;

            // Deals the damage
            Weapon.Stats stats = weapon.GetStats();
            es.TakeDamage(GetDamage(), source, stats.knockback);
            pierce--;

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }
        else if (p) // Prop
        {
            // Deals the damage
            p.TakeDamage(GetDamage());
            Weapon.Stats stats = weapon.GetStats();
            pierce--;

            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f);
            }
        }

        // Destroy projectile if it has run out pierce
        if (pierce <= 0) Destroy(gameObject);
    }
}

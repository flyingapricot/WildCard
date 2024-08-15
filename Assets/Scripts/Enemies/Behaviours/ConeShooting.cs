using UnityEngine;

public class ConeShooting : MonoBehaviour
{
    public GameObject fireball; // The fireball prefab to be instantiated
    public float cooldown = 0.1f; // Time between each fireball shot in seconds
    public float coneRange = 90f; // Angle range of the cone attack in degrees
    public int fireballCount = 5; // Number of fireballs to shoot in the cone
    public float attackDuration = 5f; // How long the attack lasts in seconds
    public float attackDelay = 3f; // Time between each attack in seconds

    private float timer = 0f; // Timer to keep track of when to fire
    private float attackTimer = 0f; // Timer to keep track of the attack duration
    private bool isAttacking = false; // Whether the enemy is currently attacking
    private Transform player; // Reference to the player's transform

    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
    }

    void Update()
    {
        // Update the attack delay timer if not attacking
        if (!isAttacking)
        {
            attackTimer += Time.deltaTime;

            // Start the attack if the delay is over
            if (attackTimer >= attackDelay)
            {
                isAttacking = true;
                attackTimer = 0f; // Reset the attack timer for duration tracking
            }
        }
        else
        {
            // Update the fire timer
            timer += Time.deltaTime;

            // Fire bullets if cooldown time has passed
            if (timer >= cooldown)
            {
                FireBullets();
                timer = 0f; // Reset the fire timer
            }

            // Update the attack duration timer
            attackTimer += Time.deltaTime;

            // Stop attacking if the attack duration is over
            if (attackTimer >= attackDuration)
            {
                isAttacking = false;
                attackTimer = 0f; // Reset for the next delay period
            }
        }
    }

    void FireBullets()
    {
        // Calculate the direction from the enemy to the player
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // Calculate the angle to the player
        float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        for (int i = 0; i < fireballCount; i++)
        {
            // Randomize the angle within the cone range
            float randomAngle = Random.Range(-coneRange / 2, coneRange / 2);
            float angle = angleToPlayer + randomAngle;

            // Calculate the direction for this fireball
            Vector2 fireballDirection = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Instantiate the fireball and set its velocity
            GameObject newFireball = Instantiate(fireball, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = newFireball.GetComponent<Rigidbody2D>();
            bulletRb.velocity = directionToPlayer * newFireball.GetComponent<Bullet>().projSpeed;

            // Rotate the fireball to face its direction
            newFireball.transform.right = fireballDirection;
        }
    }
}

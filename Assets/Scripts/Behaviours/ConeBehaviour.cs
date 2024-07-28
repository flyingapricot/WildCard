using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ConeBehaviour : MonoBehaviour
{
    public Transform FirePoint;
    public GameObject bulletPrefab;
    private float fireRate = 0.1f; // Seconds between each shot
    private float nextFireTime = 0f;
    Vector2 knockbackVelocity;
    float knockbackDuration;
    SpriteRenderer sprite;
    EnemyStats stats;

    private void Awake()
    {
        stats = GetComponent<EnemyStats>();
        sprite = GetComponentInChildren<SpriteRenderer>(); // Get the SpriteRenderer from the child GameObject
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextFireTime)
        {
            // Call the method that should run repeatedly while the mouse is held down
            // Update the next fire time
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, FirePoint.position, FirePoint.rotation);
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

                Vector2 moveDirection = (targetPosition - transform.position).normalized;
                if (moveDirection.x != 0) // Flip the sprite based on the horizontal direction
                {
                    //transform.Rotate(0f, 180f, 0f);
                    sprite.flipX = moveDirection.x > 0; // Flip when moving right
                    //FirePoint.r
                }
            }
        }
    }
}

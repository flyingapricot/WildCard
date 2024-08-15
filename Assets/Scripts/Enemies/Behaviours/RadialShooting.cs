using UnityEngine;

public class RadialShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // The bullet to be instantiated
    public int numberOfPoints = 8; // Number of points around the circle to shoot from
    public float fireRate = 1f; // Time between each shot in seconds
    public float rotationSpeed = 10f; // Speed at which the shooting pattern rotates

    private float currentAngle = 0f; // The current angle for the rotation
    private float fireTimer = 0f; // Timer to keep track of when to fire

    void Update()
    {
        // Update the fire timer
        fireTimer += Time.deltaTime;

        // If it's time to fire, shoot the bullets
        if (fireTimer >= fireRate)
        {
            FireBullets();
            fireTimer = 0f; // Reset the fire timer
        }

        // Slowly rotate the shooting pattern
        currentAngle += rotationSpeed * Time.deltaTime;
    }

    void FireBullets()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            // Calculate the angle for each bullet
            float angle = currentAngle + (i * 360f / numberOfPoints);
            Vector2 direction = new(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // Instantiate the bullet and set its direction
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            // Set bullet velocity based on calculated direction
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.velocity = direction * bullet.GetComponent<Bullet>().projSpeed;

            // Rotate the bullet to face its movement direction
            bullet.transform.right = direction;
        }
    }
}
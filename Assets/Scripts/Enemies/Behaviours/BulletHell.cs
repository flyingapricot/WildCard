using UnityEngine;

public class BulletHell : MonoBehaviour
{
    public Transform FirePoint1;
    public Transform FirePoint2;
    public Transform FirePoint3;
    public Transform FirePoint4;
    public GameObject bulletPrefab;
    private float fireRate = 0.1f; // Seconds between each shot
    private float nextFireTime = 0f;

    // protected override Update()
    // {
    //     base.Update();
    //     if (Time.time >= nextFireTime)
    //     {
    //         nextFireTime = Time.time + fireRate; // Update the next fire time
    //         Shoot();
    //     }
    // }

    void Shoot()
    {
        Instantiate(bulletPrefab, FirePoint1.position, FirePoint1.rotation);
        Instantiate(bulletPrefab, FirePoint2.position, FirePoint2.rotation);
        Instantiate(bulletPrefab, FirePoint3.position, FirePoint3.rotation);
        Instantiate(bulletPrefab, FirePoint4.position, FirePoint4.rotation);
    }
}

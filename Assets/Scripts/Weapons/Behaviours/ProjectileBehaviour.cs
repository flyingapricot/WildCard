using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base script of all Projectile Weapon behaviours
// Not Placed anywhere

public class ProjectileBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    protected Vector3 direction;
    public float destroyAfterSeconds;

    // Current Projectile Stats
    protected float currentDamage;
    protected float currentArea;
    protected float currentSpeed;
    protected float currentDuration;
    protected float currentAmount;
    //protected float currentCooldown;
    protected int currentPierce;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentArea = weaponData.Area;
        currentSpeed = weaponData.ProjectileSpeed;
        currentDuration = weaponData.Duration;
        currentAmount = weaponData.Amount;
        //currentCooldown = weaponData.Cooldown;
        currentPierce = weaponData.Pierce;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().currentDamage;
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage()); // Use GetCurrentDamage() since multiplier might be applied
            ReducePierce(); 
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
                ReducePierce(); 
            }
        }
    }

    void ReducePierce() // Destroy projectile once its pierce reaches 0
    {
        currentPierce--;
        if (currentPierce <= 0)
        {
            Destroy(gameObject); 
        }
    }

    public void DirectionChecker(Vector3 dir)
    {
        direction = dir;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // angle += spriteAngleOffset;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /*public void DirectionChecker(Vector3 dir)
    {
        direction = dir;
        float dirx = direction.x;
        float diry = direction.y;
        Vector3 scale = transform.localScale;
        Vector3 rotation = transform.rotation.eulerAngles;

        // Default direction is right, rotation.z = 0f
        if (dirx < 0 && diry == 0) // Left
        {
            rotation.z = 180f;
        }
        else if (dirx == 0 && diry > 0) // Up
        {
            rotation.z = 90f;
        }
        else if (dirx == 0 && diry < 0) // Down
        {
            rotation.z = -90f;
        }
        else if (dirx > 0 && diry > 0) // Top Right
        {
            rotation.z = 45f;
        }
        else if (dirx > 0 && diry < 0) // Bottom Right
        {
            rotation.z = -45f;
        }
        else if (dirx < 0 && diry < 0) // Bottom Left
        {
            rotation.z = -135f;
        }
        else if (dirx < 0 && diry > 0) // Top Left
        {
            rotation.z = 135f;
        }

        transform.localScale = scale;
        transform.rotation = Quaternion.Euler(rotation); // Vector3 cannot be converted
    }*/
}

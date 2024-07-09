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
    protected int currentKnockback;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentArea = weaponData.Area;
        currentSpeed = weaponData.ProjectileSpeed;
        currentDuration = weaponData.Duration;
        currentAmount = weaponData.Amount;
        //currentCooldown = weaponData.Cooldown;
        currentPierce = weaponData.Pierce;
        currentKnockback = weaponData.Knockback;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentDamage;
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.TakeDamage(GetCurrentDamage(), transform.position, currentKnockback); // Use GetCurrentDamage() since multiplier might be applied
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Base script of all Melee Weapon behaviours
// Not Placed anywhere

public class MeleeBehaviour : MonoBehaviour
{
    public WeaponScriptableObject weaponData;
    public float destroyAfterSeconds;

    // Current Melee Stats
    protected float currentDamage;
    protected float currentArea;
    protected float currentDuration;
    //protected float currentCooldown;

    void Awake()
    {
        currentDamage = weaponData.Damage;
        currentArea = weaponData.Area;
        currentDuration = weaponData.Duration;
        //currentCooldown = weaponData.Cooldown;
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
            enemy.TakeDamage(GetCurrentDamage()); // Use GetCurrentDamage() since multiplier might be applied
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
            }
        }
    }
}

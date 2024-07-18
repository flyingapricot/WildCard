using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] GameObject leftSwordAttack;
    [SerializeField] GameObject rightSwordAttack;
    //[SerializeField] int attackDamage = 1;
    [SerializeField] float attackInterval = 2f;
    [SerializeField] Vector2 swordAttackSize = new Vector2(4f, 2f);
    float timer;
    PlayerMovement playerMovement;

    private void Awake()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            Attack();
        }
    }

    private void Attack()
    {
        timer = attackInterval;

        if (playerMovement.lastHorizontalVector > 0) // Stores latest player direction
        {
            rightSwordAttack.SetActive(true); // Right attack
            Collider2D[] colliders = Physics2D.OverlapBoxAll(rightSwordAttack.transform.position, swordAttackSize, 0f);
            //ApplyDamage(colliders);
        }
        else
        {
            leftSwordAttack.SetActive(true); // Left attack
            Collider2D[] colliders = Physics2D.OverlapBoxAll(leftSwordAttack.transform.position, swordAttackSize, 0f);
            //ApplyDamage(colliders);
        }
    }
    /*
    private void ApplyDamage(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            Diamond d = colliders[i].GetComponent<Diamond>(); // Only diamond enemy for now
            if (d != null)
            {
                d.TakeDamage(attackDamage); // Refer to Diamond.cs
            }
        }
    } */
}

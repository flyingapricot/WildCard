using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator animator;
    PlayerMovement pm;
    SpriteRenderer sr;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float vertical;
    [HideInInspector] public bool isMoving;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        isMoving = pm.moveDir.sqrMagnitude > 0;
        horizontal = pm.lastHorizontalVector;
        vertical = pm.lastVerticalVector;

        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Movement
    [HideInInspector] public Vector2 moveDir; // (0,0)
    [HideInInspector] public float lastHorizontalVector;
    [HideInInspector] public float lastVerticalVector;
    [HideInInspector] public Vector2 lastMovedVector;

    // References
    Rigidbody2D rb;
    PlayerStats player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerStats>();
        // Give projectile initial direction (Down)
        lastMovedVector = new Vector2(0f, -1f);
    }

    void Update() // Frame rate dependent
    {
        InputManagement();
    }

    void FixedUpdate() // Frame rate independent
    {
        Move();
    }

    void InputManagement()
    {
        if (GameManager.instance.isGameOver)
        {
            return; // Prevents inputs if game over
        }

        // When Input.GetAxisRaw("Horizontal") is called, it checks the current state of the input devices associated with the "Horizontal" axis:
        // If you press 'D' or the right arrow key, or push a joystick to the right, it returns 1.
        // If you press 'A' or the left arrow key, or push a joystick to the left, it returns -1.
        // If there is no input or the joystick is in the neutral position along the horizontal axis, it returns 0.

        float moveX, moveY;
        // if (VirtualJoystick.CountActiveInstances() > 0)
        // {
        //     moveX = VirtualJoystick.GetAxisRaw("Horizontal");
        //     moveY = VirtualJoystick.GetAxisRaw("Vertical");
        // }
        // else
        // {
            moveX = Input.GetAxisRaw("Horizontal");
            moveY = Input.GetAxisRaw("Vertical");
        //}
        
        moveDir = new Vector2(moveX, moveY).normalized;

        // Update the latest movement vector based on input
        if (moveDir.x != 0 || moveDir.y != 0)
        {
            lastHorizontalVector = moveDir.x;
            lastVerticalVector = moveDir.y;
            lastMovedVector = new Vector2(lastHorizontalVector, lastVerticalVector);
        }
    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return; // Prevents movement if game over
        }
        rb.velocity = new Vector2(moveDir.x * player.Stats.moveSpeed, moveDir.y * player.Stats.moveSpeed);
    }
}

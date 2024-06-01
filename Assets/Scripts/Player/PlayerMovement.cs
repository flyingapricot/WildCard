using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    // Movement
    [HideInInspector] public Vector3 movementVector;
    [HideInInspector] public float lastHorizontalVector;
    [HideInInspector] public float lastVerticalVector;
    [HideInInspector] public Vector3 lastMovementVector;
    //[SerializeField] public float speed = 5.0f;

    // References
    Rigidbody2D player;
    PlayerStats stats;

    private void Awake()
    {
        player = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        movementVector = new Vector3(); // (0,0,0)
        // Give projectile initial direction (Down)
        lastMovementVector = new Vector3(0f, -1f, 0f);
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
        // When Input.GetAxisRaw("Horizontal") is called, it checks the current state of the input devices associated with the "Horizontal" axis:
        // If you press 'D' or the right arrow key, or push a joystick to the right, it returns 1.
        // If you press 'A' or the left arrow key, or push a joystick to the left, it returns -1.
        // If there is no input or the joystick is in the neutral position along the horizontal axis, it returns 0.

        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.y = Input.GetAxisRaw("Vertical");
        
        // Stores latest player direction
        if (movementVector.x != 0)
        {
            lastHorizontalVector = movementVector.x;
            lastMovementVector = new Vector3(lastHorizontalVector, 0f, 0f);
        }
        if (movementVector.y != 0)
        {
            lastVerticalVector = movementVector.y;
            lastMovementVector = new Vector3(0f, lastVerticalVector, 0f);
        }
        if (movementVector.x != 0 && movementVector.y != 0) // Diagonal
        {
            lastMovementVector = new Vector3(lastHorizontalVector, lastVerticalVector, 0f);
        }
    }

    void Move()
    {
        // The movement is multiplied by a speed factor and Time.deltaTime to make sure it's smooth and frame-rate independent.
        movementVector *= stats.currentMoveSpeed;
        player.velocity = movementVector;
    }
}

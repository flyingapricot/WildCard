using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    public float frequency; // Speed of movement
    public float magnitude; // Range of movement
    public Vector3 direction; // Direction of movement
    Vector3 initialPosition;
    Pickup pickup;

    void Start()
    {
        pickup = GetComponent<Pickup>();

        // Saves the starting position of the game object
        initialPosition = transform.position;
    }

    void Update()
    {
        if (pickup)
        {
            // Sine function for smooth bobbing effect
            transform.position = initialPosition + magnitude * Mathf.Sin(Time.time * frequency) * direction;
        }
    }

    // Method to stop bobbing animation
    public void StopBobbing()
    {
        enabled = false; // Disable this script
    }
}

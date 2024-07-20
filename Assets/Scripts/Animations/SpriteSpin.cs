using UnityEngine;

public class SpriteSpin : MonoBehaviour
{
    // Speed of the rotation
    public float rotationSpeed = 100f;

    // Update is called once per frame
    void Update()
    {
        // Rotate the sprite around its Z axis
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}


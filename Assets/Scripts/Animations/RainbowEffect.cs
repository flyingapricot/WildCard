using UnityEngine;

public class RainbowEffect : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float colorChangeSpeed = 1.0f; // Speed of color change

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Calculate the color based on time
        float time = Time.time * colorChangeSpeed;
        float red = Mathf.Sin(time) * 0.5f + 0.5f;
        float green = Mathf.Sin(time + Mathf.PI / 3f) * 0.5f + 0.5f;
        float blue = Mathf.Sin(time + 2 * Mathf.PI / 3f) * 0.5f + 0.5f;

        spriteRenderer.color = new Color(red, green, blue);
    }
}

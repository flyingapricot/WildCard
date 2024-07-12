using System.Collections;
using UnityEngine;

public class BreakableProps : MonoBehaviour
{
    public float durability;
    public Sprite brokenSprite; // Reference to the broken sprite
    public float fadeDuration = 1f; // Duration of the fade-out

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the GameObject");
        }
    }

    public void TakeDamage(float dmg)
    {
        durability -= dmg;

        if (durability <= 0)
        {
            if (brokenSprite == null)
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(BreakAndFade());
            }
        }
    }

    private IEnumerator BreakAndFade()
    {
        // Change to the broken sprite
        spriteRenderer.sprite = brokenSprite;

        // Fade out
        float elapsedTime = 0f;
        Color originalColor = spriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure the sprite is fully transparent at the end
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Destroy the game object
        Destroy(gameObject);
    }
}


using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeAnimation : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f; // Duration for the fade animation
    [SerializeField] private bool fadeInOnStart = true; // Object to fade in when the scene starts
    [SerializeField] private bool fadeOutOnStart = false; // Object to fade out when the scene starts

    private SpriteRenderer spriteRenderer;
    private CanvasGroup canvasGroup;
    private Image image;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        if (fadeInOnStart)
        {
            SetAlpha(0f); // Ensure starting transparent for fade in
            StartCoroutine(FadeIn());
        }

        if (fadeOutOnStart)
        {
            StartCoroutine(FadeOut());
        }
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(0f, 1f);
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(1f, 0f);
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        else if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
        else if (image != null)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / fadeDuration);
            SetAlpha(newAlpha);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(endAlpha);
    }
}

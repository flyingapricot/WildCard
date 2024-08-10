using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    // Call this method to initialize the health bar
    public void InitializeHealthBar(float max)
    {
        slider.maxValue = max;
        slider.value = max;
        fill.color = gradient.Evaluate(1f); // Full health
    }

    public void SetHealth(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
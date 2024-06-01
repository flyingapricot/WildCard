using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetExp(float current, float max)
    {
        slider.maxValue = max;
        slider.value = current;
        
        if (current < 0f)
        {
            current = 0f;
        }
        
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
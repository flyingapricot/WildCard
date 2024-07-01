using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EXPBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public TMP_Text levelText;

    public void SetExp(float current, float max, int level)
    {
        slider.maxValue = max;
        slider.value = current;
        
        if (current < 0f)
        {
            current = 0f;
        }
        
        fill.color = gradient.Evaluate(slider.normalizedValue);
        levelText.text = "LV " + level.ToString(); // Update level text
    }
}
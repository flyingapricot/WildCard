using TMPro;
using UnityEngine;

public class SoulsDisplay : MonoBehaviour
{
    public TMP_Text soulsText;

    void OnEnable()
    {
        if (SoulsManager.instance != null)
        {
            SoulsManager.instance.OnSoulsChanged += UpdateSoulsDisplay;
            UpdateSoulsDisplay(); // Initial update
        }
    }

    void OnDisable()
    {
        if (SoulsManager.instance != null)
        {
            SoulsManager.instance.OnSoulsChanged -= UpdateSoulsDisplay;
        }
    }

    void UpdateSoulsDisplay()
    {
        soulsText.text = SoulsManager.instance.soulCount.ToString();
    }
}

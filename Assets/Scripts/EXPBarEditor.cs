using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EXPBar))]
public class EXPBarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Reference to the target script
        EXPBar expBar = (EXPBar)target;

        // Add a slider in the inspector
        EditorGUILayout.LabelField("Test EXP Bar Fill", EditorStyles.boldLabel);
        expBar.slider.value = EditorGUILayout.Slider("Current EXP", expBar.slider.value, 0, expBar.slider.maxValue);

        // Manually call SetExp to update the fill
        expBar.SetExp(expBar.slider.value, expBar.slider.maxValue, expBar.slider.value == expBar.slider.maxValue ? 1 : 0);

        // Ensure the changes are visible in edit mode
        if (GUI.changed)
        {
            EditorUtility.SetDirty(expBar);
        }
    }
}

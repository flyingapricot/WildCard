using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("This will be replaced by the Passive class.")]
public class PassiveItem : MonoBehaviour
{
    protected PlayerStats player;
    public PassiveItemScriptableObject passiveItemData;

    protected virtual void ApplyModifier()
    {
        // Apply the boost value to the appropriate stat in the child classes
    }

    void Start()
    {
        player = FindObjectOfType<PlayerStats>();
        ApplyModifier();
    }
}

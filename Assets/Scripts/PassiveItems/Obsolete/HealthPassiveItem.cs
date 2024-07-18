using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("Will soon be replaced by PassiveData")]
public class HealthPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.MaxHealth *= 1 + passiveItemData.Multiplier / 100f; // Original multiplied by multiplier %
    }
}

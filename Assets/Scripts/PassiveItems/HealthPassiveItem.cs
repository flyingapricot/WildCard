using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMaxHealth *= 1 + passiveItemData.Multiplier / 100f; // Original multiplied by multiplier %
    }
}

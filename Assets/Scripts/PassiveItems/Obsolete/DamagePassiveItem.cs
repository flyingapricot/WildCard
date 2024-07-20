using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("To be replaced")]
public class DamagePassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMight *= 1 + passiveItemData.Multiplier / 100f; // Original multiplied by multiplier %
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.currentDamage *= 1 + passiveItemData.Multiplier / 100f; // Original multiplied by multiplier %
    }
}

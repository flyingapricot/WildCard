using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMoveSpeed *= 1 + passiveItemData.Multiplier / 100f; // Original multiplied by multiplier %
    }
}

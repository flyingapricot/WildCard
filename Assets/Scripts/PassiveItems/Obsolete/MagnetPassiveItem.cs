using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentMagnet += passiveItemData.Multiplier; // Original added by multiplier
    }
}

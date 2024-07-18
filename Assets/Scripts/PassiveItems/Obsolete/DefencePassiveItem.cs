using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefencePassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentArmour += passiveItemData.Multiplier; // Original added by multiplier
    }
}

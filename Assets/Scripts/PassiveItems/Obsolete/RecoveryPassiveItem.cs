using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.CurrentRecovery += passiveItemData.Multiplier; // Original added by multiplier
    }
}

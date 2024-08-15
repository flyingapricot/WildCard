using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

/// <summary>
/// A class that takes a PassiveData and is used to increment a player's stat when equipped
/// </summary>

public class Passive : Item
{
    [SerializeField] CharacterData.Stats currentBoosts;

    [System.Serializable]
    public class Modifier : LevelData
    {
        public CharacterData.Stats boosts;
    }

    // For dynamically created passives, call initialize to set everything up
    public virtual void Initialize(PassiveData data)
    {
        base.Initialize(data);
        this.data = data;
        currentBoosts = data.baseStats.boosts;
    }

    public virtual CharacterData.Stats GetBoosts()
    {
        return currentBoosts;
    }

    // Levels up the PassiveItem by 1 and calculates the corresponding stats
    public override bool DoLevelUp()
    {
        base.DoLevelUp();

        // Prevents level up if already at max level
        if (!CanLevelUp())
        {
            Debug.LogWarning(string.Format("Cannot level up {0} to level {1}, max level of {2} already reached.", name, currentLevel, data.maxLevel));
            return false;
        }

        // Otherwise, add stats of the next level to our PassiveItem
        currentBoosts += ((Modifier)data.GetLevelData(++currentLevel)).boosts; // Level++
        return true;
    }
    
}

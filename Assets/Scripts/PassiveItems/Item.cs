using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for both the Passive Items and Weapon classes
/// </summary>

public abstract class Item : MonoBehaviour
{
    public int currentLevel = 1, maxLevel = 1;

    protected PlayerStats player;

    public virtual void Initialize(ItemData data)
    {
        maxLevel = data.maxLevel;
        player = FindObjectOfType<PlayerStats>();
    }

    public virtual bool CanLevelUp()
    {
        return currentLevel <= maxLevel;
    }

    public virtual bool DoLevelUp()
    {
        // Weapon Evolution Logic
        return true;
    }

    // Effects received upon equipping the item
    public virtual void OnEquip()
    {

    }

    // Effects removed upon unequipping the item
    public virtual void OnUnequip()
    {

    }
}

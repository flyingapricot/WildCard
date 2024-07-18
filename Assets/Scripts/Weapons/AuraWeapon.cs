using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraWeapon : Weapon
{
    protected Aura currentAura;

    protected override void Update() { }

    public override void OnEquip() 
    {
        // Try to replace the aura of the weapon with a new one
        if (currentStats.auraPrefab)
        {
            if (currentAura) Destroy(currentAura);
            currentAura = Instantiate(currentStats.auraPrefab, transform);
            currentAura.weapon = this;
            currentAura.player = player;
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
    }

    public override void OnUnequip() 
    {
        if (currentAura) Destroy(currentAura);
    }

    public override bool DoLevelUp() 
    {
        if (!base.DoLevelUp()) return false;

        // If there is an aura attached to this weapon, we update the aura
        if (currentAura)
        {
                Destroy(currentAura.gameObject);
                currentAura = Instantiate(currentStats.auraPrefab, transform);
                currentAura.weapon = this;
                currentAura.player = player;
                currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
        return true;
    }
}

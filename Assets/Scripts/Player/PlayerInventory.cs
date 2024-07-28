using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if (item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Assigned {0} to player.", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty() { return item == null; }
    }

    public List<Slot> weaponSlots = new(6);
    public List<Slot> passiveSlots = new(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public Image upgradeIcon;
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new(); // List of weapon upgrade options
    public List<PassiveData> availablePassives = new(); // List of passive items upgrade options
    public List<UpgradeUI> upgradeUIOptions = new(); // List of UI for upgrade options present in the scene

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
        availableWeapons.Add(player.characterData.StartingWeapon);
    }

    // Checks if the inventory has an item of a certain type
    public bool Has(ItemData type) { return Get(type); }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }

    // Find a passive of a certain type in the inventory
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p.data == type)
                return p;
        }
        return null;
    }

    // Find a weapon of a certain type in the inventory
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w.data == type)
                return w;
        }
        return null;
    }

    // Removes a weapon of a particular type, as specified by <data>
    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        // Remove this weapon from the upgrade pool
        if (removeUpgradeAvailability) availableWeapons.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }
        return false;
    }

    // Removes a passive of a particular type, as specified by <data>
    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        // Remove this passive from the upgrade pool
        if (removeUpgradeAvailability) availablePassives.Remove(data);

        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.data == data)
            {
                passiveSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }
        return false;
    }

    // If an ItemData is passed, determine what type it is and call the respective overload
    // We also have an optional boolean to remove this item from the upgrade list
    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData) return Remove(data as PassiveData, removeUpgradeAvailability);
        else if (data is WeaponData) return Remove(data as WeaponData, removeUpgradeAvailability);
        return false;
    }

    // Find an empty slot and adds a weapon of certain type
    // Returns the slot number that the item was put in
    public int Add(WeaponData data)
    {
        int slotNum = -1;

        // Try to find an empty slot
        for (int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // If there is no empty slot, exit
        if (slotNum < 0) return slotNum;

        // Otherwise create the weapon in the slot
        // Get the type of the weapon we want to spawn
        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            // Spawn the weapon GameObject
            GameObject go = new(data.baseStats.name + "Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.Initialize(data);
            spawnedWeapon.transform.SetParent(transform); // Set the weapon to be child of the player
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.OnEquip();

            // Assign the weapon to the slot
            weaponSlots[slotNum].Assign(spawnedWeapon);

            // Close the level up UI if its open
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
                GameManager.instance.EndLevelUp();

            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}", data.name));
        }

        return -1;
    }

    // Find an empty slot and adds a passive of certain type
    // Returns the slot number that the item was put in
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        // Try to find an empty slot
        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // If there is no empty slot, exit
        if (slotNum < 0) return slotNum;

        // Otherwise create the passive in the slot
        // Get the type of the passive we want to spawn
        GameObject go = new(data.baseStats.name + "Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialize(data);
        p.transform.SetParent(transform); // Set the passive to be child of the player
        p.transform.localPosition = Vector2.zero;

        // Assign the passive to the slot
        passiveSlots[slotNum].Assign(p);

        // Close the level up UI if its open
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            GameManager.instance.EndLevelUp();

        player.RecalculateStats();
        return slotNum;
    }

    // If we don't know what item is being added, this function will determine that
    public int Add(ItemData data)
    {
        if (data is WeaponData) return Add(data as WeaponData);
        else if (data is PassiveData) return Add(data as PassiveData);
        return -1;
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            Weapon weapon = weaponSlots[slotIndex].item as Weapon;

            // Don't level up the weapon if its already at max level
            if (!weapon.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", weapon.name));
                return;
            }
        }

        // Close the level up UI if its open
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            GameManager.instance.EndLevelUp();
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveSlots.Count > slotIndex)
        {
            Passive p = passiveSlots[slotIndex].item as Passive;

            // Don't level up the passive item if its already at max level
            if (!p.DoLevelUp())
            {
                Debug.LogWarning(string.Format("Failed to level up {0}", p.name));
                return;
            }
        }

        // Close the level up UI if its open
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            GameManager.instance.EndLevelUp();

        player.RecalculateStats();
    }

    // Determines what upgrade options should appear
    void ApplyUpgradeOptions()
    {
        // Keeps track of which upgrade is listed to prevent duplicated options
        List<WeaponData> availableWeaponUpgrades = new(availableWeapons);
        List<PassiveData> availablePassiveItemUpgrades = new(availablePassives);

        // Iterate through each slot in the upgrade UI
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            // If there are no more available upgrades, abort
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                Debug.Log("No more upgrades available.");
                return;
            }
            
            // Determine whether this upgrade should be for passive items or weapons
            int upgradeType;
            if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2; // Only PassiveItem Upgrades Left
            }
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1; // Only Weapon Upgrades Left
            }
            else
            {
                upgradeType = UnityEngine.Random.Range(1, 3); // 1 is inclusive, 3 is exclusive, 2C1 between weapon and passive items
            }

            // Generates an active weapon upgrade
            #region Weapon Upgrade
            if (upgradeType == 1)
            {
                // Choose a random weapon to upgrade, then remove it so we don't get it twice
                WeaponData chosenWeaponUpgrade = availableWeaponUpgrades[UnityEngine.Random.Range(0, availableWeaponUpgrades.Count)];
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

                // Ensure that the selected weapon data is valid
                if (chosenWeaponUpgrade != null)
                {
                    // Turn on the UI slot
                    EnableUpgradeUI(upgradeOption);

                    // Loops through all our existing weapons
                    // If we find a match, we will hook an event listener to the button
                    // Which will level up the weapon when this upgrade option is clicked
                    bool isLevelUp = false; 
                    for (int i = 0; i < weaponSlots.Count; i++)
                    {
                        Weapon w = weaponSlots[i].item as Weapon;
                        if (w != null && w.data == chosenWeaponUpgrade)
                        {
                            // If the weapon is already max level, do not upgrade
                            if (chosenWeaponUpgrade.maxLevel <= w.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            // Set the Event Listener, item and level description to be that of the next level
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, i)); // Apply upgrade functionality to button
                            Weapon.Stats nextLevel = chosenWeaponUpgrade.GetLevelData(w.currentLevel + 1);
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }
                    
                    // If the code reaches here, it means we will be adding a new weapon
                    // Instead of upgrading an existing weapon
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenWeaponUpgrade)); // Apply spawn functionality to button
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.baseStats.name;
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.baseStats.description;
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.icon;
                    }
                }
            }
            #endregion

            // Generates a passive item upgrade
            #region Passive Item Upgrade
            else if (upgradeType == 2)
            {
                // Choose a random passive to upgrade, then remove it so we don't get it twice
                PassiveData chosenPassiveUpgrade = availablePassiveItemUpgrades[UnityEngine.Random.Range(0, availablePassiveItemUpgrades.Count)];
                availablePassiveItemUpgrades.Remove(chosenPassiveUpgrade);

                // Ensure that the selected passive data is valid
                if (chosenPassiveUpgrade != null)
                {
                    // Turn on the UI slot
                    EnableUpgradeUI(upgradeOption);

                    // Loops through all our existing passives
                    // If we find a match, we will hook an event listener to the button
                    // Which will level up the weapon when this upgrade option is clicked
                    bool isLevelUp = false; 
                    for (int i = 0; i < passiveSlots.Count; i++)
                    {
                        Passive p = passiveSlots[i].item as Passive;
                        if (p != null && p.data == chosenPassiveUpgrade)
                        {
                            // If the passive is already max level, do not upgrade
                            if (chosenPassiveUpgrade.maxLevel <= p.currentLevel)
                            {
                                DisableUpgradeUI(upgradeOption);
                                isLevelUp = true;
                                break;
                            }

                            // Set the Event Listener, item and level description to be that of the next level
                            upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, i)); // Apply upgrade functionality to button
                            Passive.Modifier nextLevel = chosenPassiveUpgrade.GetLevelData(p.currentLevel + 1);
                            upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                            upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                            upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                            isLevelUp = true;
                            break;
                        }
                    }
                    
                    // If the code reaches here, it means we will be adding a new passive item
                    // Instead of upgrading an existing passive item
                    if (!isLevelUp)
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenPassiveUpgrade)); // Apply spawn functionality to button
                        Passive.Modifier nextLevel = chosenPassiveUpgrade.baseStats;
                        upgradeOption.upgradeNameDisplay.text = nextLevel.name;
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeIcon.sprite = chosenPassiveUpgrade.icon;
                    }
                }
            }
            #endregion
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners(); // Removed button functionality
            DisableUpgradeUI(upgradeOption); // Removes Option UI completely
        }
    }

    public void RemoveAndApplyUpgrades() // Use this for SendMessage()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI (UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI (UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    PlayerStats player;

    #region Weapons
    [Header("Weapons")]
    public List<WeaponController> weaponSlots = new List<WeaponController>(6); // List of Weapons
    public List<Image> weaponUISlots = new List<Image>(6); // List of Weapon Icons
    public int[] weaponLevels = new int[3]; // List of the Weapons current levels
    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }
    public List<WeaponUpgrade> weaponUpgradeOptions = new List<WeaponUpgrade>(); // List of available Weapon Upgrades
    #endregion

    #region Passive Items
    [Header("Passive Items")]
    public List<PassiveItem> passiveItemSlots = new List<PassiveItem>(6); // List of Passive Items
    public List<Image> passiveItemUISlots = new List<Image>(6); // List of Passive Items Icons
    public int[] passiveItemLevels = new int[3]; // List of current Passive Items levels
    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new List<PassiveItemUpgrade>(); // List of available Passive Items upgrades
    #endregion

    #region Upgrade UI
    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }
    [Header("Upgrade UI")]
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();
    #endregion

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    public void AddWeapon(int slotIndex, WeaponController weapon)
    {
        weaponSlots[slotIndex] = weapon;
        weaponLevels[slotIndex] = weapon.weaponData.Level;
        weaponUISlots[slotIndex].enabled = true;
        weaponUISlots[slotIndex].sprite = weapon.weaponData.Icon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void AddPassiveItem(int slotIndex, PassiveItem passiveItem)
    {
        passiveItemSlots[slotIndex] = passiveItem;
        passiveItemLevels[slotIndex] = passiveItem.passiveItemData.Level;
        passiveItemUISlots[slotIndex].enabled = true;
        passiveItemUISlots[slotIndex].sprite = passiveItem.passiveItemData.Icon;

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
    }

    public void LevelUpWeapon(int slotIndex, int upgradeIndex)
    {
        if (weaponSlots.Count > slotIndex)
        {
            WeaponController weapon = weaponSlots[slotIndex];

            if (!weapon.weaponData.NextLevelPrefab) // Checks if there is a next level
            {
                Debug.LogError(weapon.name + " is at Max Level.");
                return;
            }

            GameObject upgradedWeapon = Instantiate(weapon.weaponData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedWeapon.transform.SetParent(transform); // Set weapon to be child of the player
            AddWeapon(slotIndex, upgradedWeapon.GetComponent<WeaponController>());
            Destroy(weapon.gameObject);
            weaponLevels[slotIndex] = upgradedWeapon.GetComponent<WeaponController>().weaponData.Level; // To ensure correct weapon level

            weaponUpgradeOptions[upgradeIndex].weaponData = upgradedWeapon.GetComponent<WeaponController>().weaponData;
            
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    public void LevelUpPassiveItem(int slotIndex, int upgradeIndex)
    {
        if (passiveItemSlots.Count > slotIndex)
        {
            PassiveItem passiveItem = passiveItemSlots[slotIndex];

            if (!passiveItem.passiveItemData.NextLevelPrefab) // Checks if there is a next level
            {
                Debug.LogError(passiveItem.name + " is at Max Level.");
                return;
            }

            GameObject upgradedPassiveItem = Instantiate(passiveItem.passiveItemData.NextLevelPrefab, transform.position, Quaternion.identity);
            upgradedPassiveItem.transform.SetParent(transform); // Set passive item to be child of the player
            AddPassiveItem(slotIndex, upgradedPassiveItem.GetComponent<PassiveItem>());
            Destroy(passiveItem.gameObject);
            passiveItemLevels[slotIndex] = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData.Level; // To ensure correct passive item level

            passiveItemUpgradeOptions[upgradeIndex].passiveItemData = upgradedPassiveItem.GetComponent<PassiveItem>().passiveItemData;
                        
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
            {
                GameManager.instance.EndLevelUp();
            }
        }
    }

    void ApplyUpgradeOptions()
    {
        // Keeps track of which upgrade is listed to prevent duplicated options
        List<WeaponUpgrade> availableWeaponUpgrades = new List<WeaponUpgrade>(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new List<PassiveItemUpgrade>(passiveItemUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions)
        {
            int upgradeType;
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                return; // If no more upgrades, Stop (for now)
            }
            else if (availableWeaponUpgrades.Count == 0)
            {
                upgradeType = 2; // Only PassiveItem Upgrades Left
            }
            else if (availablePassiveItemUpgrades.Count == 0)
            {
                upgradeType = 1; // Only Weapon Upgrades Left
            }
            else
            {
                upgradeType = Random.Range(1, 3); // 1 is inclusive, 3 is exclusive, 2C1 between weapon and passive items
            }

            #region Weapon Upgrade
            if (upgradeType == 1) // If weapon
            {
                WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)]; // Choose a random weapon to upgrade
                availableWeaponUpgrades.Remove(chosenWeaponUpgrade); // Removes upgrade option from being chosen again

                if (chosenWeaponUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool newWeapon = true; 
                    for (int i = 0; i < weaponSlots.Count; i++) // Iterate through every weapon slot
                    {
                        if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData) // Check if weapon already equipped
                        {
                            newWeapon = false; // If exists in inventory with same data, weapon is already equipped.
                            
                            if (!newWeapon)
                            {
                                if (!chosenWeaponUpgrade.weaponData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption); // Removes Option UI completely
                                    break; // Stop if there are no further upgrades
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex)); // Apply upgrade functionality to button
                                // Set Weapon's next level name and description
                                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                            }
                            break; // Stop iterating
                        }
                    }

                    if (newWeapon) // Weapon is not equipped yet, Spawn a new weapon
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon)); // Apply spawn functionality to button
                        // Set Weapon's initial name and description
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
                }
            }
            #endregion

            #region Passive Item Upgrade
            else if (upgradeType == 2) // If Passive Item
            {
                PassiveItemUpgrade chosenPassiveItemUpgrade = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)]; // Chosoe a random PassiveItem to upgrade
                availablePassiveItemUpgrades.Remove(chosenPassiveItemUpgrade); // Removes upgrade option from being chosen again


                if (chosenPassiveItemUpgrade != null)
                {
                    EnableUpgradeUI(upgradeOption);
                    bool newPassiveItem = true; 
                    for (int i = 0; i < passiveItemSlots.Count; i++) // Iterate through every PassiveItem slot
                    {
                        if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData) // Check if PassiveItem already equipped
                        {
                            newPassiveItem = false; // If exists in inventory with same data, PassiveItem is already equipped.
                            
                            if (!newPassiveItem)
                            {
                                if (!chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab)
                                {
                                    DisableUpgradeUI(upgradeOption); // Removes Option UI completely
                                    break; // Stop if there are no further upgrades
                                }

                                upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex)); // Apply upgrade functionality to button
                                // Set PassiveItem's next level name and description
                                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
                                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                            }
                            break; // Stop iterating
                        }
                    }

                    if (newPassiveItem) // PassiveItem is not equipped yet, Spawn a new PassiveItem
                    {
                        upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem)); // Apply spawn functionality to button
                        // Set PassiveItem's initial name and description
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name;
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description;
                    }

                    upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon;
                }
            }
            #endregion
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (var upgradeOption in upgradeUIOptions)
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

    void DisableUpgradeUI (UpgradeUI UI)
    {
        UI.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI (UpgradeUI UI)
    {
        UI.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }
}

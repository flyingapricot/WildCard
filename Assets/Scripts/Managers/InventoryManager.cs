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
    public List<WeaponController> weaponSlots = new(6); // List of Weapons
    public List<Image> weaponUISlots = new(6); // List of Weapon Icons
    public int[] weaponLevels = new int[3]; // List of the Weapons current levels
    [System.Serializable]
    public class WeaponUpgrade
    {
        public int weaponUpgradeIndex;
        public GameObject initialWeapon;
        public WeaponScriptableObject weaponData;
    }
    public List<WeaponUpgrade> weaponUpgradeOptions = new(); // List of available Weapon Upgrades
    #endregion

    #region Passive Items
    [Header("Passive Items")]
    public List<PassiveItem> passiveItemSlots = new(6); // List of Passive Items
    public List<Image> passiveItemUISlots = new(6); // List of Passive Items Icons
    public int[] passiveItemLevels = new int[3]; // List of current Passive Items levels
    [System.Serializable]
    public class PassiveItemUpgrade
    {
        public int passiveItemUpgradeIndex;
        public GameObject initialPassiveItem;
        public PassiveItemScriptableObject passiveItemData;
    }
    public List<PassiveItemUpgrade> passiveItemUpgradeOptions = new(); // List of available Passive Items upgrades
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
    public List<UpgradeUI> upgradeUIOptions = new();
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
        List<WeaponUpgrade> availableWeaponUpgrades = new(weaponUpgradeOptions);
        List<PassiveItemUpgrade> availablePassiveItemUpgrades = new(passiveItemUpgradeOptions);

        foreach (var upgradeOption in upgradeUIOptions)
        {
            // Check available upgrades
            if (availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
            {
                DisableUpgradeUI(upgradeOption);
                Debug.Log("No more upgrades available.");
                continue; // Skip to next upgrade option
            }

            bool upgradeAssigned = false;

            while (!upgradeAssigned)
            {
                // Determine upgrade type with true RNG
                int upgradeType = Random.Range(1, 3); // Randomly choose between weapon (1) and passive items (2)

                // Check if chosen type has available upgrades
                if (upgradeType == 1 && availableWeaponUpgrades.Count > 0)
                {
                    upgradeAssigned = TryAssignWeaponUpgrade(upgradeOption, ref availableWeaponUpgrades);
                }
                else if (upgradeType == 2 && availablePassiveItemUpgrades.Count > 0)
                {
                    upgradeAssigned = TryAssignPassiveItemUpgrade(upgradeOption, ref availablePassiveItemUpgrades);
                }

                // If the chosen type had no upgrades available, recheck both lists to avoid infinite loop
                if (!upgradeAssigned && availableWeaponUpgrades.Count == 0 && availablePassiveItemUpgrades.Count == 0)
                {
                    DisableUpgradeUI(upgradeOption);
                    Debug.Log("Ran out of upgrades during assignment.");
                    break;
                }
            }
        }
    }

    bool TryAssignWeaponUpgrade(UpgradeUI upgradeOption, ref List<WeaponUpgrade> availableWeaponUpgrades)
    {
        if (availableWeaponUpgrades.Count == 0)
        {
            return false;
        }

        WeaponUpgrade chosenWeaponUpgrade = availableWeaponUpgrades[Random.Range(0, availableWeaponUpgrades.Count)];
        availableWeaponUpgrades.Remove(chosenWeaponUpgrade);

        if (chosenWeaponUpgrade != null)
        {
            EnableUpgradeUI(upgradeOption);
            bool newWeapon = true;

            for (int i = 0; i < weaponSlots.Count; i++)
            {
                if (weaponSlots[i] != null && weaponSlots[i].weaponData == chosenWeaponUpgrade.weaponData)
                {
                    newWeapon = false;

                    if (!newWeapon)
                    {
                        if (!chosenWeaponUpgrade.weaponData.NextLevelPrefab)
                        {
                            DisableUpgradeUI(upgradeOption);
                            Debug.Log("No further weapon upgrades available.");
                            return false;
                        }

                        upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpWeapon(i, chosenWeaponUpgrade.weaponUpgradeIndex));
                        upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Icon;
                        upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Name;
                        upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.NextLevelPrefab.GetComponent<WeaponController>().weaponData.Description;
                    }
                    break;
                }
            }

            if (newWeapon)
            {
                upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnWeapon(chosenWeaponUpgrade.initialWeapon));
                upgradeOption.upgradeIcon.sprite = chosenWeaponUpgrade.weaponData.Icon;
                upgradeOption.upgradeNameDisplay.text = chosenWeaponUpgrade.weaponData.Name;
                upgradeOption.upgradeDescriptionDisplay.text = chosenWeaponUpgrade.weaponData.Description;
            }

            return true;
        }
        return false;
    }

    bool TryAssignPassiveItemUpgrade(UpgradeUI upgradeOption, ref List<PassiveItemUpgrade> availablePassiveItemUpgrades)
    {
        if (availablePassiveItemUpgrades.Count == 0)
        {
            return false;
        }

        PassiveItemUpgrade chosenPassiveItemUpgrade = availablePassiveItemUpgrades[Random.Range(0, availablePassiveItemUpgrades.Count)];
        availablePassiveItemUpgrades.Remove(chosenPassiveItemUpgrade);

        if (chosenPassiveItemUpgrade != null)
        {
            EnableUpgradeUI(upgradeOption);
            bool newPassiveItem = true;

            for (int i = 0; i < passiveItemSlots.Count; i++)
            {
                if (passiveItemSlots[i] != null && passiveItemSlots[i].passiveItemData == chosenPassiveItemUpgrade.passiveItemData)
                {
                    newPassiveItem = false;

                    if (!newPassiveItem)
                    {
                        if (!chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab)
                        {
                            DisableUpgradeUI(upgradeOption);
                            Debug.Log("No further passive item upgrades available.");
                            return false;
                        }

                        upgradeOption.upgradeButton.onClick.AddListener(() => LevelUpPassiveItem(i, chosenPassiveItemUpgrade.passiveItemUpgradeIndex));
                        upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Icon;
                        upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Name;
                        upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.NextLevelPrefab.GetComponent<PassiveItem>().passiveItemData.Description;
                    }
                    break;
                }
            }

            if (newPassiveItem)
            {
                upgradeOption.upgradeButton.onClick.AddListener(() => player.SpawnPassiveItem(chosenPassiveItemUpgrade.initialPassiveItem));
                upgradeOption.upgradeIcon.sprite = chosenPassiveItemUpgrade.passiveItemData.Icon;
                upgradeOption.upgradeNameDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Name;
                upgradeOption.upgradeDescriptionDisplay.text = chosenPassiveItemUpgrade.passiveItemData.Description;
            }

            return true;
        }
        return false;
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

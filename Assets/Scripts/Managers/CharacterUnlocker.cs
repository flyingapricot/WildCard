using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterUnlocker : MonoBehaviour
{
    public GameObject characterToUnlock; // Sans
    public GameObject warningMessage; // Shows up when not enough Souls
    public GameObject unlockedImage; // Shows when character is unlocked
    public Button unlockButton;
    public TMP_Text costText;
    public int unlockCost;

    void Start()
    {
        costText.text = unlockCost.ToString();
        unlockButton.onClick.AddListener(UnlockCharacter);
        unlockedImage.SetActive(false);
        warningMessage.SetActive(false); // Hide warning text initially
        CheckCharacterStatus();
    }

    void CheckCharacterStatus()
    {
        // Check if the character has been unlocked previously
        if (PlayerPrefs.GetInt(characterToUnlock.name + "_Unlocked", 0) == 1)
        {
            characterToUnlock.SetActive(true);
            unlockedImage.SetActive(true);
            unlockButton.gameObject.SetActive(false); // Hide unlock button if character is already unlocked
        }
        else
        {
            characterToUnlock.SetActive(false);
        }
    }

    void UnlockCharacter()
    {
        if (SoulsManager.instance.SpendSouls(unlockCost))
        {
            characterToUnlock.SetActive(true);
            PlayerPrefs.SetInt(characterToUnlock.name + "_Unlocked", 1);
            unlockButton.gameObject.SetActive(false);
            unlockedImage.SetActive(true);
            warningMessage.SetActive(false);
        }
        else
        {
            StartCoroutine(ShowWarningMessage());
        }
    }

    IEnumerator ShowWarningMessage()
    {
        warningMessage.SetActive(true);
        yield return new WaitForSeconds(2);
        warningMessage.SetActive(false);
    }

    public void ResetCharacter()
    {
        PlayerPrefs.SetInt(characterToUnlock.name + "_Unlocked", 0);
        characterToUnlock.SetActive(false);
        unlockedImage.SetActive(false);
        unlockButton.gameObject.SetActive(true);
    }
}

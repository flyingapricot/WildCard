using UnityEngine;
using TMPro;
using PlayFab.ClientModels;

public class MainMenu : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject characterScreen;
    public GameObject tutorialScreen;
    public GameObject settingsScreen;
    public GameObject creditsScreen;
    public GameObject shopScreen;
    public TMP_Text killCount;
    public TMP_Text scoreCount;
    public TMP_Text loginStatus;

    public void updateText(GetAccountInfoResult result)
    {
        if (result != null && result.AccountInfo != null)
        {
            // Access the email address
            if (result.AccountInfo.PrivateInfo != null)
            {
                string[] parts = result.AccountInfo.PrivateInfo.Email.Split('@');
                if(parts.Length >= 1)
                {
                    loginStatus.GetComponent<TMP_Text>().text = "Logged in as: " + parts[0];
                    loginStatus.GetComponent<TMP_Text>().color = Color.green;
                }
            }
            else
            {
                Debug.Log("No email address found for this player.");
            }
        }
        else
        {
            Debug.Log("Account info is null.");
        }

    }

    public void Update()
    {
        //Before playing, check if user is logged in
        if (AccountManager.Instance.isPlayerLoggedIn() == true && string.Equals(loginStatus.GetComponent<TMP_Text>().text, "Log in First!"))
        {
            AccountManager.Instance.GetAccountInfo(updateText);
        }
    }


    void Start()
    {
        MenuSelect();
    }

    public void MenuSelect()
    {
        // Update highscore and total kills
        killCount.text = PlayerPrefs.GetInt("totalKills", 0).ToString();
        scoreCount.text = PlayerPrefs.GetInt("highscore", 0).ToString();

        // Activate title screen only
        titleScreen.SetActive(true);
        characterScreen.SetActive(false);
        tutorialScreen.SetActive(false);
        settingsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        shopScreen.SetActive(false);
    }

    public void CharacterSelect()
    {
        //Before playing, check if user is logged in
        if (AccountManager.Instance.isPlayerLoggedIn() == true)
        {
            titleScreen.SetActive(false);
            characterScreen.SetActive(true);
        }
    }

    public void TutorialSelect()
    {
        titleScreen.SetActive(false);
        tutorialScreen.SetActive(true);
    }

    public void ShopSelect()
    {
        titleScreen.SetActive(false);
        shopScreen.SetActive(true);
    }

    public void ToggleSettings()
    {
        settingsScreen.SetActive(!settingsScreen.activeSelf);
    }

    public void ToggleCredits()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
    }

    public void LevelSelect(string levelName)
    {
        SceneController.instance.LoadGameplay(levelName);
    }

    public void LoginSelect()
    {
        SceneController.instance.LoadLogin();
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
}

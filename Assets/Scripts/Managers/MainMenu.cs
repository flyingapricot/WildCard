using System.Collections;
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
    public GameObject leaderboardScreen;
    public GameObject loginStatus;
    public TMP_Text killCount;
    public TMP_Text scoreCount;

    void Start()
    {
        MenuSelect();
    }

    public void MenuSelect()
    {
        // Update highscore and total kills
        killCount.text = PlayerPrefs.GetInt("totalKills", 0).ToString();
        scoreCount.text = PlayerPrefs.GetInt("highscore", 0).ToString();

        // Update Login Status and show
        if (AccountManager.Instance.isPlayerLoggedIn() == true)
        {
            AccountManager.Instance.GetAccountInfo(LoginUpdate);
        }

        // Activate title screen only
        titleScreen.SetActive(true);
        characterScreen.SetActive(false);
        tutorialScreen.SetActive(false);
        settingsScreen.SetActive(false);
        creditsScreen.SetActive(false);
        shopScreen.SetActive(false);
        leaderboardScreen.SetActive(false);
    }

    public void CharacterSelect()
    {
        titleScreen.SetActive(false);
        characterScreen.SetActive(true);
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

    public void LevelSelect(string levelName)
    {
        SceneController.instance.LoadGameplay(levelName);
    }


    public void ToggleSettings()
    {
        settingsScreen.SetActive(!settingsScreen.activeSelf);
    }

    public void ToggleCredits()
    {
        creditsScreen.SetActive(!creditsScreen.activeSelf);
    }
    public void LoginSelect()
    {
        SceneController.instance.LoadLogin();
    }
    public void ViewLeaderboard()
    {
        // Before showing leaderboard, check if user is logged in
        if (AccountManager.Instance.isPlayerLoggedIn() == true)
        {
            titleScreen.SetActive(false);
            leaderboardScreen.SetActive(true);
        }
        else
        {
            if (!loginStatus.activeInHierarchy)
            {
                loginStatus.GetComponent<TMP_Text>().text = "Log in First!";
                StartCoroutine(ShowMessage(loginStatus));
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    public void LoginUpdate(GetAccountInfoResult result)
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
                    StartCoroutine(ShowMessage(loginStatus));
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

    IEnumerator ShowMessage(GameObject message)
    {
        message.SetActive(true);
        yield return new WaitForSeconds(5);
        message.SetActive(false);
    }
}

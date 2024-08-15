using UnityEngine;
using TMPro;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject characterScreen;
    public GameObject tutorialScreen;
    public GameObject settingsScreen;
    public GameObject creditsScreen;
    public GameObject shopScreen;

    void Start()
    {
        SoulsManager.instance.LoadSouls();
        MenuSelect();
    }

    public void MenuSelect()
    {
        titleScreen.SetActive(true);
        characterScreen.SetActive(false);
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

    public void SettingsSelect()
    {
        settingsScreen.SetActive(true);
    }

    public void CreditsSelect()
    {
        creditsScreen.SetActive(true);
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

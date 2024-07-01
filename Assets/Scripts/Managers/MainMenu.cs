using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject titleScreen;
    public GameObject characterScreen;

    void Start()
    {
        titleScreen.SetActive(true);
        characterScreen.SetActive(false);
    }

    public void CharacterSelect()
    {
        titleScreen.SetActive(false);
        characterScreen.SetActive(true);
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

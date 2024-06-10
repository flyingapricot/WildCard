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

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }

    /*
    public GameObject menu;
    public GameObject loadingInterface;
    public Image loadingProgressBar;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();
    
    public void StartGame()
    {
        HideMenu();
        ShowLoadingScreen();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Gameplay"));
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Level01", LoadSceneMode.Additive));
        StartCoroutine(LoadingScreen());
    }

    public void HideMenu()
    {
        menu.SetActive(false);
    }
    public void ShowLoadingScreen()
    {
        loadingInterface.SetActive(true);
    }

    IEnumerator LoadingScreen()
    {
        float totalProgress = 0;
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                totalProgress += scenesToLoad[i].progress;
                loadingProgressBar.fillAmount = totalProgress / scenesToLoad.Count;
                yield return null;
            }
        }
    } */
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour // Cannot be called SceneManager
{
    public string menuSceneName = "MainMenu"; 
    public string gameplaySceneName = "Gameplay"; 
    public string tutorialSceneName = "Tutorial"; 
    public string levelSceneName = "DarkForest"; 

    void Start()
    {
        LoadMainMenu();
    }

    bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    void LoadMainMenu()
    {
        if (!IsSceneLoaded(menuSceneName))
        {
            SceneManager.LoadScene(menuSceneName, LoadSceneMode.Single); // Single will unload all other scenes
        }
    }

    public void LoadGameplay(string sceneName)
    {
        if (!IsSceneLoaded(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Single); 
        }

        if (!IsSceneLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive); // Additive will load on top of current scene
        }
    }

    /*
        if (IsSceneLoaded(gameplaySceneName))
        {
            SceneManager.UnloadSceneAsync(gameplaySceneName);
        }

        gameplayBGM.loop = true;
        gameplayBGM.Play();

        if (gameplayBGM.isPlaying)
        {
            gameplayBGM.Stop();
        }
    */
}

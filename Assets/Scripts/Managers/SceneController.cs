using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour // Cannot be called SceneManager
{
    public static SceneController instance { get; private set; } // Singleton instance

    public string menuSceneName = "MainMenu"; 
    public string gameplaySceneName = "Gameplay"; 
    public string tutorialSceneName = "Tutorial"; 
    public string levelSceneName = "DarkForest"; 
    public string loginSceneName = "MongoDB"; 

    void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        if (PlayerStats.instance != null)
        {
            PlayerStats.instance.DestroySingleton();
        }
        
        StartCoroutine(SwitchScene(menuSceneName));
    }

    public void LoadGameplay(string levelName)
    {
        StartCoroutine(SwitchScene(gameplaySceneName, levelName));
    }

    public void LoadTutorial()
    {
        StartCoroutine(SwitchScene(tutorialSceneName));
    }

    public void LoadLevel()
    {
        StartCoroutine(SwitchScene(levelSceneName));
    }

    public void LoadLogin()
    {
        if (CharacterSelector.instance != null)
        {
            CharacterSelector.instance.DestroySingleton();
        }

        StartCoroutine(SwitchScene(loginSceneName));
    }

    IEnumerator SwitchScene(string targetScene, string additionalScene = null)
    {
        // Load the target scene
        if (!IsSceneLoaded(targetScene))
        {
            yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        }

        // Optionally load an additional scene
        if (!string.IsNullOrEmpty(additionalScene) && !IsSceneLoaded(additionalScene))
        {
            yield return SceneManager.LoadSceneAsync(additionalScene, LoadSceneMode.Additive);
        }

        // Wait a frame to ensure scenes are loaded before unloading
        // yield return null;

        // Unload all other scenes
        UnloadAllScenesExcept(targetScene, additionalScene);

        // Set the target scene as active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));

        // Resume time
        Time.timeScale = 1;
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

    void UnloadAllScenesExcept(string sceneToKeep, string additionalSceneToKeep = null)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != sceneToKeep && scene.name != additionalSceneToKeep)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
}
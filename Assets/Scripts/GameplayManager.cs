using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public string gameplaySceneName = "Gameplay"; // Name of your gameplay scene
    public AudioSource gameplayBGM;

    void Start()
    {
        LoadGameplayScene();
    }

    void LoadGameplayScene()
    {
        if (!IsSceneLoaded(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName, LoadSceneMode.Additive);
            gameplayBGM.loop = true;
            gameplayBGM.Play();
        }
    }

    void UnloadGameplayScene()
    {
        if (IsSceneLoaded(gameplaySceneName))
        {
            SceneManager.UnloadSceneAsync(gameplaySceneName);
            gameplayBGM.Stop();
        }
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
}
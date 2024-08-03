using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance

    // Defines the different states of the game
    public enum GameState
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState; // Stores the current state of the game
    public GameState previousState; // Stores the state of the game before it was paused
    public GameObject playerObject; 
    public bool isGameOver = false;
    public bool choosingUpgrade = false;

    [Header("BGM")]
    [SerializeField] private AudioSource audioSource; // The audio source that will play the BGM
    [SerializeField] private AudioClip gameplayBGM; // The BGM for the gameplay
    [SerializeField] private AudioClip gameOverBGM; // The BGM for the game over screen

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;

    [Header("Current Stat Displays")]
    public TMP_Text Health;
    public TMP_Text Attack;
    public TMP_Text Defence;
    public TMP_Text Recovery;
    public TMP_Text Speed;
    public TMP_Text Magnet;

    [Header("Results Screen Stats")]
    public Image chosenCharacterSprite;
    public Image chosenCharacterName;
    public TMP_Text levelReached;
    public TMP_Text timeSurvived;
    public List<Image> weaponsUI = new List<Image>(6);
    public List<Image> passiveItemsUI = new List<Image>(6);

    [Header("Stopwatch")]
    public float timeLimit; // Time when reapers spawn
    float stopwatchTime; // Time elapsed (in seconds)
    public TMP_Text stopwatchDisplay;
    public float GetElapsedTime() { return stopwatchTime; } // Gives us the time since the level has started.

    [Header("Damage Text")]
    public Canvas damageTextCanvas;
    public Camera referenceCamera;
    public Vector3 initialOffset = new(0, 2f, 0); // You can adjust the initial vertical offset here
    private Transform damageTextParent; // Place to keep the texts


    void Awake()
    {
        // Singleton pattern implementation
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Find the "DamageTexts" GameObject
        damageTextParent = damageTextCanvas.transform.Find("DamageTexts");

        DisableScreens();
    }

    void Update()
    {        
        // Define the behaviour for each state

        switch (currentState) 
        {
            case GameState.Gameplay:
                SwitchBGM(gameplayBGM);
                CheckForPauseAndResume();
                UpdateStopwatch(); // Stopwatch only runs in this state
                break;

            case GameState.Paused:
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                if (!isGameOver)
                {
                    Time.timeScale = 0f; // Stop the game
                    isGameOver = true;
                    SwitchBGM(gameOverBGM);
                    Debug.Log("GAME OVER.");
                    DisplayResults();
                }
                break;

            case GameState.LevelUp:
                if (!choosingUpgrade)
                {
                    Time.timeScale = 0f; // Stop the game
                    choosingUpgrade = true;
                    Debug.Log("Leveled Up!");
                    levelUpScreen.SetActive(true);
                }
                break;

            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
        levelUpScreen.SetActive(false);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void SwitchBGM(AudioClip newBGM)
    {
        if (newBGM == null)
        {
            audioSource.Stop();
        }
        else
        {
            if (audioSource.clip != newBGM)
            {
                audioSource.clip = newBGM;
                audioSource.Play();
            }
        }
    }

    #region Pause / Resume
    public void PauseGame()
    {
        if (currentState != GameState.Paused)
        {
            previousState = currentState;
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Pauses the game
            pauseScreen.SetActive(true);
            Debug.Log("Game is Paused.");
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // Resumes the game
            pauseScreen.SetActive(false);
            Debug.Log("Game is Resumed.");
        }
    }

    // Ensure can only pause and resume in certain states
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == GameState.Paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    #endregion

    #region Game Over
    public void ReturnMenu()
    {
        SceneController.instance.LoadMainMenu();
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        timeSurvived.text = stopwatchDisplay.text;
        resultsScreen.SetActive(true);
    }

    public void AssignCharacterUI(CharacterData characterData)
    {
        chosenCharacterName.sprite = characterData.Name;
        chosenCharacterSprite.sprite = characterData.Sprite;
    }

    public void AssignLevelReached(int levelData)
    {
        levelReached.text = levelData.ToString();
    }

    public void AssignInventory(List<PlayerInventory.Slot> weaponsData, List<PlayerInventory.Slot> passiveItemsData)
    {
        // Check that both lists have the same length
        if (weaponsData.Count != weaponsUI.Count || passiveItemsData.Count != passiveItemsUI.Count)
        {
            Debug.Log("Inventory data list have different lengths.");
            return;
        }

        // Assigns weapons data to weapons UI
        for (int i = 0; i < weaponsUI.Count; i++)
        {
            // Check the sprite of the corresponding element in weapons data is not null
            if (weaponsData[i].image.sprite)
            {
                // Enables the corresponding element in weapons UI and set its sprite
                weaponsUI[i].enabled = true;
                weaponsUI[i].sprite = weaponsData[i].image.sprite;
                Debug.Log("Weapon equipped.");
            }
            else
            {
                // If sprite null, disable the corresponding element in weapons UI
                weaponsUI[i].enabled = false;
            }
        }

        // Assigns passive items data to weapons UI
        for (int i = 0; i < passiveItemsUI.Count; i++)
        {
            // Check the sprite of the corresponding element in passive items data is not null
            if (passiveItemsData[i].image.sprite)
            {
                // Enables the corresponding element in passive items UI and set its sprite
                passiveItemsUI[i].enabled = true;
                passiveItemsUI[i].sprite = passiveItemsData[i].image.sprite;
                Debug.Log("Item equipped.");
            }
            else
            {
                // If sprite null, disable the corresponding element in passive items UI
                passiveItemsUI[i].enabled = false;
            }
        }
    }
    #endregion

    #region Level Up
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        playerObject.SendMessage("RemoveAndApplyUpgrades"); // Execute function in InventoryManager
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false;
        Time.timeScale = 1f; // Resume Game
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
    }
    #endregion

    #region Stopwatch
    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;
        UpdateStopwatchDisplay();
        if (stopwatchTime >= timeLimit)
        {
            PlayerStats.instance.Kill();
        }
    }

    void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60);
        int seconds = Mathf.FloorToInt(stopwatchTime % 60);
        // Display the elapsed time in minutes:seconds
        stopwatchDisplay.text = string.Format("{0:00} : {1:00}", minutes, seconds);
    }
    #endregion

    #region Damage Text
    public static void GenerateDamageText(string text, Transform target, float duration = 0.2f, float speed = 1f, Color? color = null)
    {
        // If no canvas, no damage text
        if (!instance.damageTextCanvas) return;
        // Find relevant camera to convert world position to screen position
        if (!instance.referenceCamera) instance.referenceCamera = Camera.main;

        instance.StartCoroutine(instance.DamageTextCoroutine(text, target, duration, speed, color ?? Color.white));
    }

    private IEnumerator DamageTextCoroutine(string text, Transform target, float duration, float speed, Color color)
    {
        // Start generating the damage text
        GameObject textObj = ObjectPool.instance.GetPooledObject();
        // Parent the generated text object to the "DamageTexts" folder
        textObj.transform.SetParent(instance.damageTextParent.transform, false);

        TextMeshProUGUI tmPro = textObj.GetComponent<TextMeshProUGUI>();
        RectTransform rect = textObj.GetComponent<RectTransform>();

        tmPro.text = text;
        tmPro.color = color;
        // tmPro.alpha = 1f;

        // Initial position offset above the enemy
        Vector3 startPosition = referenceCamera.WorldToScreenPoint(target.position + initialOffset);
        rect.position = startPosition;

        textObj.SetActive(true);

        float t = 0;
        Vector3 offset = new(0, speed * Time.deltaTime);

        while (t < duration)
        {
            // Wait for a frame and update time
            yield return null;
            t += Time.deltaTime;

            // Fade out the text
            // tmPro.alpha = Mathf.Lerp(1, 0, t / duration);

            // Move the text upwards, check if target is still valid
            if (target != null)
            {
                rect.position = referenceCamera.WorldToScreenPoint(target.position + initialOffset) + offset * (t / duration);
            }
            else
            {
                rect.position += offset * (t / duration);
            }
        }

        textObj.SetActive(false);
    }
    #endregion
}

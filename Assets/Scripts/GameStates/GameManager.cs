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
        GameOver
    }

    // Stores the current state of the game
    public GameState currentState;
    public GameState previousState;


    [Header("BGM")]
    [SerializeField] private AudioSource audioSource; // The audio source that will play the BGM
    [SerializeField] private AudioClip gameplayBGM; // The BGM for the gameplay
    [SerializeField] private AudioClip gameOverBGM; // The BGM for the game over screen


    [Header("UI")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;

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

    public bool isGameOver = false;

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
                break;

            case GameState.Paused:
                CheckForPauseAndResume();
                break;

            case GameState.GameOver:
                if (!isGameOver)
                {
                    SwitchBGM(gameOverBGM);
                    isGameOver = true;
                    Time.timeScale = 0f; // Stop the game
                    Debug.Log("GAME OVER.");
                    DisplayResults();
                }
                break;

            default:
                Debug.LogWarning("STATE DOES NOT EXIST");
                break;
        }
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

    public void ReturnMenu()
    {
        SceneController.instance.LoadMainMenu();
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
        resultsScreen.SetActive(false);
    }

    public void GameOver()
    {
        ChangeState(GameState.GameOver);
    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignCharacterUI(CharacterScriptableObject characterData)
    {
        chosenCharacterSprite.sprite = characterData.CharacterSprite;
        chosenCharacterName.sprite = characterData.CharacterName;
    }

    public void AssignLevelReached(int levelData)
    {
        levelReached.text = levelData.ToString();
    }

    public void AssignInventory(List<Image> weaponsData, List<Image> passiveItemsData)
    {
        if (weaponsData.Count != weaponsUI.Count || passiveItemsData.Count != passiveItemsUI.Count)
        {
            Debug.Log("Inventory data list have different lengths.");
            return;
        }

        // Assigns weapons data to weapons UI
        for (int i = 0; i < weaponsUI.Count; i++)
        {
            // Check the sprite of the corresponding element in weapons data is not null
            if (weaponsData[i].sprite)
            {
                // Enables the corresponding element in weapons UI and set its sprite
                weaponsUI[i].enabled = true;
                weaponsUI[i].sprite = weaponsData[i].sprite;
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
            if (passiveItemsData[i].sprite)
            {
                // Enables the corresponding element in passive items UI and set its sprite
                passiveItemsUI[i].enabled = true;
                passiveItemsUI[i].sprite = passiveItemsData[i].sprite;
            }
            else
            {
                // If sprite null, disable the corresponding element in passive items UI
                passiveItemsUI[i].enabled = false;
            }
        }
    }
}

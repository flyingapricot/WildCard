using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton instance
    PlayerStats[] players; // Tracks all players.

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
    public TMP_Text rerollText; // Text showing number of rerolls left
    [HideInInspector] public int highscore; // Score determined from enemies defeated, time survived and level reached
    [HideInInspector] public int totalDefeated; // Total number of enemies player has defeated in current gameplay
    [HideInInspector] public int basicDefeated; 
    [HideInInspector] public int eliteDefeated; 
    [HideInInspector] public int bossDefeated; 
    int stackedLevelUps = 0; // If we try to StartLevelUp() multiple times.
    int rerolls = 3; // Number of times player can reroll in a playthrough.

    // Getters for parity with older scripts.
    public bool IsGameOver { get { return currentState == GameState.Paused; } }
    public bool ChoosingUpgrade { get { return currentState == GameState.LevelUp; } }

    #region Curse
    // Sums up the curse stat of all players and returns the value.
    public static float GetCumulativeCurse()
    {
        if (!instance) return 1;

        float totalCurse = 0;
        foreach(PlayerStats p in instance.players)
        {
            totalCurse += p.Actual.curse;
        }
        return Mathf.Max(1, totalCurse);
    }

    // Sum up the levels of all players and returns the value.
    public static int GetCumulativeLevels()
    {
        if (!instance) return 1;

        int totalLevel = 0;
        foreach (PlayerStats p in instance.players)
        {
            totalLevel += p.level;
        }
        return Mathf.Max(1, totalLevel);
    }
    #endregion

    #region Headers
    [Header("BGM")]
    [SerializeField] private AudioClip gameplayBGM; // The BGM for the gameplay
    [SerializeField] private AudioClip gameOverBGM; // The BGM for the game over screen
    private AudioSource audioSource; // The audio source that will play the BGM

    [Header("Screens")]
    public GameObject pauseScreen;
    public GameObject resultsScreen;
    public GameObject levelUpScreen;
    public GameObject settingsScreen;

    [Header("Results Screen Stats")]
    public Image chosenCharacterSprite;
    public Image chosenCharacterName;
    public TMP_Text timeSurvived;
    public TMP_Text levelReached;
    public TMP_Text scoreCount;
    public TMP_Text killCount;

    [Header("Stopwatch")]
    public float timeLimit; // Kills player instantly
    float stopwatchTime; // Time elapsed (in seconds)
    public TMP_Text stopwatchDisplay;
    public float GetElapsedTime() { return stopwatchTime; } // Gives us the time since the level has started.

    [Header("Damage Text")]
    public Canvas damageTextCanvas;
    public Camera referenceCamera;
    public Vector3 initialOffset = new(0, 2f, 0); // You can adjust the initial vertical offset here
    private Transform damageTextParent; // Place to keep the texts
    // public float textFontSize = 20;
    // public TMP_FontAsset textFont;
    #endregion

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

        // Load the saved kill count and highscore
        totalDefeated = PlayerPrefs.GetInt("totalKills", 0);
        highscore = PlayerPrefs.GetInt("highscore", 0);

        // Find the "DamageTexts" GameObject
        damageTextParent = damageTextCanvas.transform.Find("DamageTexts");
        players = FindObjectsOfType<PlayerStats>();
        audioSource = GetComponent<AudioSource>();
        DisableScreens();
    }

    void Update()
    {      
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

            case GameState.LevelUp:
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
        settingsScreen.SetActive(false);
    }

    // Define the method to change the state of the game
    public void ChangeState(GameState newState)
    {
        previousState = currentState;
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
            ChangeState(GameState.Paused);
            Time.timeScale = 0f; // Pauses the game
            pauseScreen.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeState(previousState);
            Time.timeScale = 1f; // Resumes the game
            pauseScreen.SetActive(false);
        }
    }

    // Define the method to check for pause and resume input
    void CheckForPauseAndResume()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
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

    public void ToggleSettings()
    {
        settingsScreen.SetActive(!settingsScreen.activeSelf);
    }
    #endregion

    #region Game Over
    public void ReturnMenu()
    {
        SceneController.instance.LoadMainMenu();
    }

    public void GameOver()
    {
        timeSurvived.text = stopwatchDisplay.text;
        ChangeState(GameState.GameOver);
        Time.timeScale = 0f; // Stop the game
        SwitchBGM(gameOverBGM);
        DisplayResults();
        UpdateTotalDefeated();
        //Send score to leaderboard
        AccountManager.Instance.SendLeaderboardHigh();
        AccountManager.Instance.SendLeaderboardKills();

    }

    void DisplayResults()
    {
        resultsScreen.SetActive(true);
    }

    public void AssignCharacterUI(CharacterData characterData)
    {
        chosenCharacterName.sprite = characterData.Name;
        chosenCharacterSprite.sprite = characterData.Sprite;
    }

    public void AssignScore(int level)
    {
        // Assign Level Reached
        levelReached.text = level.ToString();

        // Assign current gameplay score
        int baseScore = basicDefeated * 75 + eliteDefeated * 428 + bossDefeated * 2251 + (int)stopwatchTime / 60 * 126 + (int)stopwatchTime % 60 * 10 + level * 1219;
        scoreCount.text = baseScore.ToString();

        // If the score is the highest, save the highscore
        if (baseScore > highscore) { PlayerPrefs.SetInt("highscore", baseScore); }
    }

    public void UpdateTotalDefeated()
    {
        // Calculate total amount of enemies killed this gameplay
        int currentDefeated = basicDefeated + eliteDefeated + bossDefeated;
        killCount.text = currentDefeated.ToString();

        // Save the updated kill count
        totalDefeated += currentDefeated;
        PlayerPrefs.SetInt("totalKills", totalDefeated);
    }

    #endregion

    #region Level Up
    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp);
        if(levelUpScreen.activeSelf) stackedLevelUps++;
        else
        {
            levelUpScreen.SetActive(true);
            Time.timeScale = 0f; // Pause the game for now
            // Execute function in InventoryManager
            foreach(PlayerStats p in players) { p.SendMessage("RemoveAndApplyUpgrades"); } 
        }
    }

    public void EndLevelUp()
    {
        Time.timeScale = 1f; // Resume Game
        levelUpScreen.SetActive(false);
        ChangeState(GameState.Gameplay);
        if(stackedLevelUps > 0)
        {
            stackedLevelUps--;
            StartLevelUp();
        }
    }

    public void Reroll()
    {
        if (rerolls > 0)
        {
            rerolls--;
            rerollText.text = rerolls.ToString() + "/3";
            EndLevelUp();
            StartLevelUp();
        }
    }
    #endregion

    #region Stopwatch
    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime;
        UpdateStopwatchDisplay();
        if (stopwatchTime >= timeLimit)
        {
            foreach(PlayerStats p in players)
                p.Kill();
        }
    }

    void UpdateStopwatchDisplay()
    {
        // Calculate the number of minutes and seconds that have elapsed
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

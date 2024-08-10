using UnityEngine;
using TMPro;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    private readonly int[] volumeLevels = { 0, 20, 40, 60, 80, 100 };

    // References to TMP_Text components for displaying volume percentages
    private TMP_Text[] options;
    public TMP_Text masterVolumeText;
    public TMP_Text bgmVolumeText;
    public TMP_Text sfxVolumeText;
    public TMP_Text fullscreenText;

    private int selectedOptionIndex = 0;
    private int masterVolumeIndex = 5; // Default to 100%
    private int bgmVolumeIndex = 5; // Default to 100%
    private int sfxVolumeIndex = 5; // Default to 100%
    public GameObject controlsPanel;
    private bool isFullscreen = true;

    void Start()
    {
        // Initialize UI Text components with default values
        options = new TMP_Text[] { masterVolumeText, bgmVolumeText, sfxVolumeText };
        UpdateVolumeText();
        UpdateFullscreenText();
        HighlightSelectedOption();
    }

    void Update()
    {
        // Navigate through settings
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedOptionIndex = (selectedOptionIndex - 1 + options.Length) % options.Length;
            HighlightSelectedOption();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedOptionIndex = (selectedOptionIndex + 1) % options.Length;
            HighlightSelectedOption();
        }

        // Adjust volume, view controls or toggle fullscreen based on selected option
        if (selectedOptionIndex < 3)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                AdjustVolume(selectedOptionIndex, -1);
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                AdjustVolume(selectedOptionIndex, 1);
            }
        }
    }

    void AdjustVolume(int optionIndex, int change)
    {
        switch (optionIndex)
        {
            case 0:
                masterVolumeIndex = Mathf.Clamp(masterVolumeIndex + change, 0, volumeLevels.Length - 1);
                break;
            case 1:
                bgmVolumeIndex = Mathf.Clamp(bgmVolumeIndex + change, 0, volumeLevels.Length - 1);
                break;
            case 2:
                sfxVolumeIndex = Mathf.Clamp(sfxVolumeIndex + change, 0, volumeLevels.Length - 1);
                break;
        }
        UpdateVolumeText();
        ApplyVolumeSettings();
    }

    public void ToggleControls()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
    }

    public void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        Screen.fullScreen = isFullscreen;
        UpdateFullscreenText();
    }

    void UpdateVolumeText()
    {
        masterVolumeText.text = volumeLevels[masterVolumeIndex] + "%";
        bgmVolumeText.text = volumeLevels[bgmVolumeIndex] + "%";
        sfxVolumeText.text = volumeLevels[sfxVolumeIndex] + "%";
    }

    void UpdateFullscreenText() { fullscreenText.text = isFullscreen ? "On" : "Off"; }

    void HighlightSelectedOption()
    {
        // Clear existing highlights
        foreach (var option in options)
        {
            option.color = Color.white; // Change to your default color
        }

        // Highlight the selected option
        options[selectedOptionIndex].color = Color.yellow; // Change to your highlight color
    }

    void ApplyVolumeSettings()
    {
        // Apply master volume
        AudioListener.volume = volumeLevels[masterVolumeIndex] / 100f;

        // Apply BGM and SFX volume
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(volumeLevels[bgmVolumeIndex] / 100f) * 20);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volumeLevels[sfxVolumeIndex] / 100f) * 20);
    }
}

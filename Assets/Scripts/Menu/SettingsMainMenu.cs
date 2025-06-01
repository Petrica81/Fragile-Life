using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsMainMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    public string volumeParameter = "MasterVolume";

    [Header("Display Settings")]
    public Toggle fullscreenToggle;
    public TextMeshProUGUI fullscreenStatusText;

    void OnEnable()
    {
        // Initialize volume controls
        if (volumeSlider != null && audioMixer != null)
        {
            // Remove previous listeners to avoid duplicates
            volumeSlider.onValueChanged.RemoveAllListeners();

            // Add new listener
            volumeSlider.onValueChanged.AddListener(SetVolume);

            // Load saved volume
            LoadVolume();
        }
        else
        {
            Debug.LogError("Volume Slider or Audio Mixer reference is missing!");
        }
    
    InitializeFullscreenControls();
    }


    private void InitializeFullscreenControls()
    {
        if (fullscreenToggle == null) return;

        fullscreenToggle.onValueChanged.RemoveAllListeners();
        fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);

        LoadFullscreenPreference();
        UpdateFullscreenText();
    }

    private void SetVolume(float volume)
    {

        // Convert linear 0-1 slider value to logarithmic dB scale (-80 to 0)
        float dBValue = Mathf.Lerp(-80f, 0f, Mathf.Pow(volume, 0.25f));
        audioMixer.SetFloat(volumeParameter, dBValue);

        // Save volume preference
        PlayerPrefs.SetFloat("SavedVolume", volume);
        PlayerPrefs.Save(); // Explicitly save


    }

    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("SavedVolume", 0.75f); // Default 75%
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume); // Apply the loaded volume
    }


    private void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenMode", isFullscreen ? 1 : 0);
        UpdateFullscreenText();
    }

    private void UpdateFullscreenText()
    {
        if (fullscreenStatusText != null)
        {
            fullscreenStatusText.text = Screen.fullScreen ? "Fullscreen: ON" : "Fullscreen: OFF";
        }
    }

    private void LoadFullscreenPreference()
    {
        if (fullscreenToggle == null) return;

        bool fullscreen = PlayerPrefs.GetInt("FullscreenMode", Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = fullscreen;
        Screen.fullScreen = fullscreen;
    }
}
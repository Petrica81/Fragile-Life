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

    [Header("Music")]
    public AudioSource backgroundMusic; // Assign in Inspector

    public AudioSource GetBackgroundMusic()
    {
        return backgroundMusic;
    }
    void OnEnable()
    {
        InitializeVolumeControls();
        InitializeFullscreenControls();
    }

    private void InitializeVolumeControls()
    {
        if (volumeSlider != null && audioMixer != null)
        {
            volumeSlider.onValueChanged.RemoveAllListeners();
            volumeSlider.onValueChanged.AddListener(SetVolume);
            LoadVolume();
        }
    }

    private void InitializeFullscreenControls()
    {
        if (fullscreenToggle == null || fullscreenStatusText == null)
        {
            Debug.LogError("Fullscreen UI references missing!");
            return;
        }

        // Load saved preference or use current screen state
        bool savedFullscreen = PlayerPrefs.GetInt("FullscreenMode", Screen.fullScreen ? 1 : 0) == 1;

        // Initialize UI elements
        fullscreenToggle.SetIsOnWithoutNotify(savedFullscreen);
        UpdateFullscreenText(savedFullscreen);

        // Add listener after initialization
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

        Debug.Log($"Initialized fullscreen: {savedFullscreen}");
    }

    private void OnFullscreenToggleChanged(bool newValue)
    {
        // Apply the new fullscreen state
        SetFullscreen(newValue);
    }

    private void SetFullscreen(bool fullscreen)
    {
        // Apply to screen
        Screen.fullScreen = fullscreen;

        // Update UI
        fullscreenToggle.SetIsOnWithoutNotify(fullscreen);
        UpdateFullscreenText(fullscreen);

        // Save preference
        PlayerPrefs.SetInt("FullscreenMode", fullscreen ? 1 : 0);

        Debug.Log($"Fullscreen set to: {fullscreen}");
    }

    private void UpdateFullscreenText(bool currentState)
    {
        fullscreenStatusText.text = currentState ? "Fullscreen: ON" : "Fullscreen: OFF";
    }

    public void SetVolume(float volume)
    {
        float dBValue = Mathf.Lerp(-80f, 0f, Mathf.Pow(volume, 0.25f));
        audioMixer.SetFloat(volumeParameter, dBValue);
        PlayerPrefs.SetFloat("SavedVolume", volume);

        // Update music manager volume too
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(volume);
        }
    }

    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("SavedVolume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }
}
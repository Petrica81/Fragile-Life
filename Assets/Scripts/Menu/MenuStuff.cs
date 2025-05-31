using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;
using TMPro;

public class MenuStuff : MonoBehaviour
{
    public Image ImageScreenSettings;
    public Button toggleButton;
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    public TextMeshProUGUI TextVolume;
    public TextMeshProUGUI TextFullscreen;
    public Toggle fullscreenToggle;

    public string volumeParameter = "MusicVolume"; // Match mixer exposed parameter
    public float fadeDuration = 0.3f;

    private Color fullVisibleColor = new Color(137f / 255f, 185f / 255f, 179f / 255f, 27f / 255f);
    private bool isTransitioning = false;
    private Coroutine currentFadeRoutine;

    private void Start()
    {
        LoadFullscreenPreference();

        // Initialize UI elements
        ImageScreenSettings.color = new Color(fullVisibleColor.r, fullVisibleColor.g, fullVisibleColor.b, 0);
        ImageScreenSettings.gameObject.SetActive(true);
        volumeSlider.gameObject.SetActive(false);
        TextVolume.gameObject.SetActive(false);
        TextFullscreen.gameObject.SetActive(false);

        // Set up button listener
        toggleButton.onClick.AddListener(ToggleImageVisibility);

        // Set up volume slider
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Load saved volume
        LoadVolume();

        // Initialize fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
            fullscreenToggle.gameObject.SetActive(false); // Start hidden
        }
    }

    private void ToggleImageVisibility()
    {
        if (isTransitioning) return;

        bool shouldFadeIn = ImageScreenSettings.color.a < fullVisibleColor.a / 2f;

        if (currentFadeRoutine != null)
        {
            StopCoroutine(currentFadeRoutine);
        }

        currentFadeRoutine = StartCoroutine(shouldFadeIn ? FadeIn() : FadeOut());
    }

    private IEnumerator FadeIn()
    {
        isTransitioning = true;
        float elapsed = 0f;
        Color startColor = ImageScreenSettings.color;

        // Show UI elements immediately
        volumeSlider.gameObject.SetActive(true);
        TextVolume.gameObject.SetActive(true);
        TextFullscreen.gameObject.SetActive(true);
        if (fullscreenToggle != null) fullscreenToggle.gameObject.SetActive(true);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            ImageScreenSettings.color = Color.Lerp(startColor, fullVisibleColor, t);
            yield return null;
        }

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        isTransitioning = true;
        float elapsed = 0f;
        Color startColor = ImageScreenSettings.color;
        Color targetColor = new Color(fullVisibleColor.r, fullVisibleColor.g, fullVisibleColor.b, 0);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            ImageScreenSettings.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        // Hide UI elements after fade completes
        volumeSlider.gameObject.SetActive(false);
        TextVolume.gameObject.SetActive(false);
        TextFullscreen.gameObject.SetActive(false);
        isTransitioning = false;
        if (fullscreenToggle != null) fullscreenToggle.gameObject.SetActive(false);
    }

    private void SetVolume(float volume)
    {
        // Convert linear slider value to logarithmic dB scale
        audioMixer.SetFloat(volumeParameter, Mathf.Log10(volume) * 20);

        // Save volume preference
        PlayerPrefs.SetFloat("SavedVolume", volume);
    }

    private void LoadVolume()
    {
        if (PlayerPrefs.HasKey("SavedVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("SavedVolume");
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
        else
        {
            // Default volume (100%)
            volumeSlider.value = 1f;
            SetVolume(1f);
        }
    }

    private void ToggleFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("FullscreenMode", isFullscreen ? 1 : 0);

        // Optional: Update text to reflect current state
        if (TextFullscreen != null)
        {
            TextFullscreen.text = isFullscreen ? "Fullscreen: ON" : "Fullscreen: OFF";
        }
    }

    private void LoadFullscreenPreference()
    {
        if (PlayerPrefs.HasKey("FullscreenMode"))
        {
            bool fullscreen = PlayerPrefs.GetInt("FullscreenMode") == 1;
            Screen.fullScreen = fullscreen;
            if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;
        }
        else
        {
            // Default to current screen mode
            if (fullscreenToggle != null) fullscreenToggle.isOn = Screen.fullScreen;
        }
    }

    private void OnDestroy()
    {
        toggleButton.onClick.RemoveListener(ToggleImageVisibility);
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(ToggleFullscreen);
        }
    }
}
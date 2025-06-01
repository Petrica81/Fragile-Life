using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio;
using TMPro;

public class MenuStuff : MonoBehaviour
{
    //Settings Panel
    public Image ImageScreenSettings;
    public Button toggleButton;
    public Slider volumeSlider;
    public AudioMixer audioMixer;
    public TextMeshProUGUI TextVolume;
    public TextMeshProUGUI TextFullscreen;
    public Toggle fullscreenToggle;
    public TextMeshProUGUI TextSettings;
    public Image SettingsTitleBackground;


    public string volumeParameter = "MusicVolume"; // Match mixer exposed parameter
    public float fadeDuration = 0.3f;

    private bool isTransitioning = false;
    private Coroutine currentFadeRoutine;

    // Settings UI Elements colors
    private float originalVolumeTextAlpha;
    private float originalFullscreenTextAlpha;
    private Color originalToggleNormalColor;
    private Color originalToggleHighlightedColor;
    private Color originalImageColor;
    private float originalToggleTextAlpha;
    private float originalSettingsTextAlpha;
    private Color originalSettingsTitleBgColor;


    private void Start()
    {
        LoadFullscreenPreference();

        // Initialize UI elements
        ImageScreenSettings.gameObject.SetActive(true);
        volumeSlider.gameObject.SetActive(false);
        TextVolume.gameObject.SetActive(false);
        TextFullscreen.gameObject.SetActive(false);
        TextSettings.gameObject.SetActive(false);
        SettingsTitleBackground.gameObject.SetActive(false);

        // Set up button listener
        toggleButton.onClick.AddListener(ToggleImageVisibility);

        // Set up volume slider
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Load saved volume
        LoadVolume();

        // Store original alpha values
        originalVolumeTextAlpha = TextVolume.color.a;
        originalFullscreenTextAlpha = TextFullscreen.color.a;
        originalSettingsTextAlpha = TextSettings.color.a;
        originalSettingsTitleBgColor = SettingsTitleBackground.color;
        originalImageColor = ImageScreenSettings.color;

        // set alpha value to 0
        ImageScreenSettings.color = new Color(
            originalImageColor.r, 
            originalImageColor.g, 
            originalImageColor.b, 
            0);

        SettingsTitleBackground.color = new Color(
            originalSettingsTitleBgColor.r,
            originalSettingsTitleBgColor.g,
            originalSettingsTitleBgColor.b,
            0);

        // Initialize fullscreen toggle
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(ToggleFullscreen);
            fullscreenToggle.gameObject.SetActive(false); // Start hidden

            originalToggleNormalColor = fullscreenToggle.colors.normalColor;
            originalToggleHighlightedColor = fullscreenToggle.colors.highlightedColor;

            TextMeshProUGUI toggleText = fullscreenToggle.GetComponentInChildren<TextMeshProUGUI>();
            if (toggleText != null)
            {
                originalToggleTextAlpha = toggleText.color.a;
            }
        }
    }

    private void ToggleImageVisibility()
    {
        if (isTransitioning) return;

        bool shouldFadeIn = ImageScreenSettings.color.a < originalImageColor.a / 2f;

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

        // Activate all elements before fading
        volumeSlider.gameObject.SetActive(true);
        TextVolume.gameObject.SetActive(true);
        TextFullscreen.gameObject.SetActive(true);
        if (fullscreenToggle != null) fullscreenToggle.gameObject.SetActive(true);
        TextSettings.gameObject.SetActive(true);
        SettingsTitleBackground.gameObject.SetActive(true);

        // Get initial alpha states
        Color volumeTextColor = TextVolume.color;
        Color fullscreenTextColor = TextFullscreen.color;
        Color settingsTextColor = TextSettings.color;
        Color settingsTitleBgColor = SettingsTitleBackground.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Fade main panel to its original color
            ImageScreenSettings.color = Color.Lerp(
                startColor,
                originalImageColor,
                t
            );

            // Fade title background
            SettingsTitleBackground.color = new Color(
                settingsTitleBgColor.r,
                settingsTitleBgColor.g,
                settingsTitleBgColor.b,
                Mathf.Lerp(0, originalSettingsTitleBgColor.a, t)
            );

            // Fade text elements to their original alpha
            TextVolume.color = new Color(volumeTextColor.r, volumeTextColor.g, volumeTextColor.b, Mathf.Lerp(0, originalVolumeTextAlpha, t));
            TextFullscreen.color = new Color(fullscreenTextColor.r, fullscreenTextColor.g, fullscreenTextColor.b, Mathf.Lerp(0, originalFullscreenTextAlpha, t));
            TextSettings.color = new Color(settingsTextColor.r, settingsTextColor.g, settingsTextColor.b, Mathf.Lerp(0, originalSettingsTextAlpha, t));

            // Fade toggle (if exists)
            if (fullscreenToggle != null)
            {
                var colors = fullscreenToggle.colors;
                colors.normalColor = new Color(originalToggleNormalColor.r, originalToggleNormalColor.g, originalToggleNormalColor.b, Mathf.Lerp(0, originalToggleNormalColor.a, t));
                colors.highlightedColor = new Color(originalToggleHighlightedColor.r, originalToggleHighlightedColor.g, originalToggleHighlightedColor.b, Mathf.Lerp(0, originalToggleHighlightedColor.a, t));
                fullscreenToggle.colors = colors;

                TextMeshProUGUI toggleText = fullscreenToggle.GetComponentInChildren<TextMeshProUGUI>();
                if (toggleText != null)
                {
                    toggleText.color = new Color(toggleText.color.r, toggleText.color.g, toggleText.color.b, Mathf.Lerp(0, originalToggleTextAlpha, t));
                }
            }

            yield return null;
        }

        isTransitioning = false;
    }

    private IEnumerator FadeOut()
    {
        isTransitioning = true;
        float elapsed = 0f;
        Color startColor = ImageScreenSettings.color;
        Color targetColor = new Color(originalImageColor.r, originalImageColor.g, originalImageColor.b, 0);

        // Get current alpha states (which should be at their original values)
        Color volumeTextColor = TextVolume.color;
        Color fullscreenTextColor = TextFullscreen.color;
        Color settingsTextColor = TextSettings.color;
        Color settingsTitleBgColor = SettingsTitleBackground.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1 - Mathf.Clamp01(elapsed / fadeDuration); // Inverse lerp

            // Fade title background
            SettingsTitleBackground.color = new Color(
                settingsTitleBgColor.r,
                settingsTitleBgColor.g,
                settingsTitleBgColor.b,
                Mathf.Lerp(0, originalSettingsTitleBgColor.a, t)
            );

            // Fade main panel from current color to transparent (keeping RGB)
            ImageScreenSettings.color = Color.Lerp(
                startColor,
                targetColor,
                1 - t
            );

            // Fade text elements from their original alpha to 0
            TextVolume.color = new Color(volumeTextColor.r, volumeTextColor.g, volumeTextColor.b, Mathf.Lerp(0, originalVolumeTextAlpha, t));
            TextFullscreen.color = new Color(fullscreenTextColor.r, fullscreenTextColor.g, fullscreenTextColor.b, Mathf.Lerp(0, originalFullscreenTextAlpha, t));
            TextSettings.color = new Color(settingsTextColor.r, settingsTextColor.g, settingsTextColor.b, Mathf.Lerp(0, originalSettingsTextAlpha, t));

            // Fade toggle (if exists)
            if (fullscreenToggle != null)
            {
                var colors = fullscreenToggle.colors;
                colors.normalColor = new Color(originalToggleNormalColor.r, originalToggleNormalColor.g, originalToggleNormalColor.b, Mathf.Lerp(0, originalToggleNormalColor.a, t));
                colors.highlightedColor = new Color(originalToggleHighlightedColor.r, originalToggleHighlightedColor.g, originalToggleHighlightedColor.b, Mathf.Lerp(0, originalToggleHighlightedColor.a, t));
                fullscreenToggle.colors = colors;

                TextMeshProUGUI toggleText = fullscreenToggle.GetComponentInChildren<TextMeshProUGUI>();
                if (toggleText != null)
                {
                    toggleText.color = new Color(toggleText.color.r, toggleText.color.g, toggleText.color.b, Mathf.Lerp(0, originalToggleTextAlpha, t));
                }
            }

            yield return null;
        }

        // Deactivate after fading
        SettingsTitleBackground.gameObject.SetActive(false);
        volumeSlider.gameObject.SetActive(false);
        TextVolume.gameObject.SetActive(false);
        TextFullscreen.gameObject.SetActive(false);
        TextSettings.gameObject.SetActive(false);
        if (fullscreenToggle != null) fullscreenToggle.gameObject.SetActive(false);

        isTransitioning = false;
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
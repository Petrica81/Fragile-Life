using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Audio; // Required for audio control

public class MenuStuff : MonoBehaviour
{
    public Image ImageScreenSettings;
    public Button toggleButton;
    public Slider volumeSlider; // Assign in Inspector
    public AudioMixer audioMixer; // Assign your main audio mixer
    public string volumeParameter = "MusicVolume"; // Match mixer exposed parameter
    public float fadeDuration = 0.3f;

    private Color fullVisibleColor = new Color(137f / 255f, 185f / 255f, 179f / 255f, 27f / 255f);
    private bool isTransitioning = false;
    private Coroutine currentFadeRoutine;

    private void Start()
    {
        // Initialize UI elements
        ImageScreenSettings.color = new Color(fullVisibleColor.r, fullVisibleColor.g, fullVisibleColor.b, 0);
        ImageScreenSettings.gameObject.SetActive(true);
        volumeSlider.gameObject.SetActive(false);

        // Set up button listener
        toggleButton.onClick.AddListener(ToggleImageVisibility);

        // Set up volume slider
        volumeSlider.onValueChanged.AddListener(SetVolume);

        // Load saved volume
        LoadVolume();
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

        // Show slider immediately
        volumeSlider.gameObject.SetActive(true);

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

        // Hide slider after fade completes
        volumeSlider.gameObject.SetActive(false);
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

    private void OnDestroy()
    {
        toggleButton.onClick.RemoveListener(ToggleImageVisibility);
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
    }
}
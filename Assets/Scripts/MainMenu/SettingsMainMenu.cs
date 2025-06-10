using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;

    [Header("Options Controls")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Button fullscreenButton;
    [SerializeField] private TMP_Text fullscreenText;

    private bool isPaused = false;
    private float previousTimeScale;

    void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        // Initialize menus
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);

        // Setup volume control
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("SavedVolume", 0.75f);
            volumeSlider.onValueChanged.AddListener(SetVolume);
            SetVolume(volumeSlider.value);
        }

        // Setup fullscreen control
        if (fullscreenButton != null && fullscreenText != null)
        {
            bool savedFullscreen = PlayerPrefs.GetInt("FullscreenMode", Screen.fullScreen ? 1 : 0) == 1;
            UpdateFullscreenText(savedFullscreen);
            fullscreenButton.onClick.AddListener(ToggleFullscreen);

            Debug.Log($"PauseManager: Initialized fullscreen - Current: {Screen.fullScreen}, Saved: {savedFullscreen}");
        }
        else
        {
            Debug.LogError("PauseManager: Fullscreen UI references missing!");
        }
    }

    public void ToggleFullscreen()
    {
        bool newFullscreenState = !Screen.fullScreen;

        // Apply the new state
        Screen.fullScreen = newFullscreenState;

        // Update UI and save preference
        UpdateFullscreenText(newFullscreenState);
        PlayerPrefs.SetInt("FullscreenMode", newFullscreenState ? 1 : 0);

        Debug.Log($"PauseManager: Fullscreen toggled to {newFullscreenState}");

        // Sometimes the change doesn't apply immediately - add a fallback check
        StartCoroutine(VerifyFullscreenChange(newFullscreenState));
    }

    private IEnumerator VerifyFullscreenChange(bool expectedState)
    {
        yield return new WaitForSecondsRealtime(0.5f);

        if (Screen.fullScreen != expectedState)
        {
            Debug.LogWarning($"PauseManager: Fullscreen mismatch! Expected: {expectedState}, Actual: {Screen.fullScreen}");
            // Try applying again
            Screen.fullScreen = expectedState;
            UpdateFullscreenText(expectedState);
        }
    }

    private void UpdateFullscreenText(bool currentState)
    {
        fullscreenText.text = currentState ? "Fullscreen Mode: ON" : "Fullscreen Mode: OFF";
    }

    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("SavedVolume", volume);
    }

    // ... (keep all other methods like TogglePause, OpenOptions, etc. the same)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (optionsMenu.activeSelf)
            {
                CloseOptions();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pauseMenu.SetActive(isPaused);
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        if (!isPaused)
        {
            optionsMenu.SetActive(false);
        }
    }

    public void OpenOptions()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        TogglePause();
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
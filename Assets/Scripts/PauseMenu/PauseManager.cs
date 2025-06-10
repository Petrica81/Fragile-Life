using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for UI components

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;

    [Header("Options Controls")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private bool isPaused = false;
    private float previousTimeScale; // Store previous time scale

    void Start()
    {
        // Initialize menus
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);

        // Setup options controls
        volumeSlider.value = AudioListener.volume;
        fullscreenToggle.isOn = Screen.fullScreen;

        // Add listeners
        volumeSlider.onValueChanged.AddListener(SetVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

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

        if (isPaused)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = previousTimeScale;
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(false);
        }

        UpdateCursorState();
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
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    private void UpdateCursorState()
    {
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    // Options functions
    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
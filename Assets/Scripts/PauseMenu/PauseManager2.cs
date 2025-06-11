using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq; // Needed for FirstOrDefault() ?
using UnityEngine.EventSystems;

public class PauseManager2 : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;

    [Header("Options Controls")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip buttonHoverSound;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioSource uiAudioSource;

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

        // Setup button sounds
        SetupButtonSounds();
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

    void SetupButtonSounds()
    {
        // Get all buttons in the scene
        Button[] buttons = FindObjectsOfType<Button>(true); // Include inactive buttons

        foreach (Button button in buttons)
        {
            // Add hover sound
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ??
                                   button.gameObject.AddComponent<EventTrigger>();

            // Pointer Enter event for hover sound
            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnterEntry.callback.AddListener((data) => { PlayHoverSound(); });
            trigger.triggers.Add(pointerEnterEntry);

            // Click sound is handled by the button's onClick event
            button.onClick.AddListener(PlayClickSound);
        }
    }

    public void PlayClickSound()
    {
        if (buttonClickSound != null && uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PlayHoverSound()
    {
        if (buttonHoverSound != null && uiAudioSource != null)
        {
            uiAudioSource.PlayOneShot(buttonHoverSound);
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
        // Save to PlayerPrefs
        PlayerPrefs.SetFloat("SavedVolume", volume);

        // Apply directly to audio system
        AudioListener.volume = volume;

        // Try to affect the audio mixer
        AudioMixer mixer = Resources.FindObjectsOfTypeAll<AudioMixer>().FirstOrDefault();
        if (mixer != null)
        {
            float dBValue = Mathf.Lerp(-80f, 0f, Mathf.Pow(volume, 0.25f));
            mixer.SetFloat("MasterVolume", dBValue);
        }

        // If we find SettingsMainMenu, update its slider to match
        SettingsMainMenu settings = FindObjectOfType<SettingsMainMenu>();
        if (settings != null && settings.volumeSlider != null)
        {
            settings.volumeSlider.SetValueWithoutNotify(volume);
        }
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

    }
}

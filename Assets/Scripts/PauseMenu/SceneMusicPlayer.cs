using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    public AudioSource backgroundMusic;
    public string volumeParameter = "MasterVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize audio settings
            backgroundMusic = GetComponent<AudioSource>();
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float volume)
    {
        float dBValue = Mathf.Lerp(-80f, 0f, Mathf.Pow(volume, 0.25f));
        audioMixer.SetFloat(volumeParameter, dBValue);
        PlayerPrefs.SetFloat("SavedVolume", volume);
    }

    private void LoadVolume()
    {
        float savedVolume = PlayerPrefs.GetFloat("SavedVolume", 0.75f);
        SetVolume(savedVolume);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (backgroundMusic.clip != clip)
        {
            backgroundMusic.clip = clip;
            backgroundMusic.Play();
        }
    }
}
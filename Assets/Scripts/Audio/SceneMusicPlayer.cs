using UnityEngine;

public class SceneMusicPlayer : MonoBehaviour
{
    public AudioClip sceneMusic;

    void Start()
    {
        // Find the existing music player from main menu
        SettingsMainMenu settings = FindObjectOfType<SettingsMainMenu>();

        if (settings != null && settings.GetBackgroundMusic() != null)
        {
            AudioSource musicPlayer = settings.GetBackgroundMusic();

            // Change music if needed
            if (musicPlayer.clip != sceneMusic)
            {
                musicPlayer.clip = sceneMusic;
                musicPlayer.Play();
            }
        }
        else
        {
            Debug.LogWarning("No background music player found");
        }
    }
}
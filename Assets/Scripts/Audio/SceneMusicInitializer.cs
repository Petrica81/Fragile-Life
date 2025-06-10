using UnityEngine;

public class SceneMusicInitializer : MonoBehaviour
{
    [SerializeField] private AudioClip sceneMusic;

    void Start()
    {
        if (MusicManager.Instance == null)
        {
            Debug.LogError("MusicManager instance not found!");
            return;
        }

        if (sceneMusic == null)
        {
            Debug.LogWarning("No scene music assigned!");
            return;
        }

        MusicManager.Instance.PlayMusic(sceneMusic);
    }
}
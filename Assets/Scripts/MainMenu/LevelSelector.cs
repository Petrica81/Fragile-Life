using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Image[] levelSlots; // Assign your 3 level slot images in inspector
    [SerializeField] private GameObject[] selectionBorders; // Assign the 3 border GameObjects
    [SerializeField] private AudioClip navigationSound;

    private int currentSelectedIndex = 0;

    private void Start()
    {
        // Initialize selection
        UpdateSelectionVisual();
    }

    private void Update()
    {
        HandleKeyboardNavigation();


        // Scene Index Validation
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            LoadSelectedLevel();
        }
    }

    private void HandleKeyboardNavigation()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1); // Move left
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1); // Move right
        }
    }

    private void LoadSelectedLevel()
    {
        int sceneIndex = GetSelectedLevel();
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Loading level: " + sceneIndex);
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError("Scene index out of range!");
        }
    }

    private void MoveSelection(int direction)
    {
        // Calculate new index with wrap-around
        currentSelectedIndex = (currentSelectedIndex + direction + levelSlots.Length) % levelSlots.Length;

        UpdateSelectionVisual();
        if (navigationSound != null)
        {
            AudioSource.PlayClipAtPoint(navigationSound, Camera.main.transform.position);
        }
    }

    private void UpdateSelectionVisual()
    {
        // Hide all borders first
        foreach (var border in selectionBorders)
        {
            border.SetActive(false);
        }

        // Show only the current selected border
        if (currentSelectedIndex >= 0 && currentSelectedIndex < selectionBorders.Length)
        {
            selectionBorders[currentSelectedIndex].SetActive(true);
        }
    }

    // Call this when player confirms selection (e.g., with Enter key)
    public int GetSelectedLevel()
    {
        return currentSelectedIndex + 1; // Returns 1, 2, or 3
    }
}
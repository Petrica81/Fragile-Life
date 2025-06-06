using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; //IPointerClickHandler

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private Image[] levelSlots;
    [SerializeField] private GameObject[] selectionBorders;
    [SerializeField] private AudioClip navigationSound;

    private int currentSelectedIndex = 0;

    private void Start()
    {
        // Initialize selection
        UpdateSelectionVisual();

        // Add button click handlers
        for (int i = 0; i < levelSlots.Length; i++)
        {
            int index = i; // Important: Capture current index for closure
            Button button = levelSlots[i].GetComponent<Button>();
            button.onClick.AddListener(() => SelectLevel(index));
        }
    }

    // New method for button clicks
    public void SelectLevel(int index)
    {
        currentSelectedIndex = index;
        UpdateSelectionVisual();
        PlayNavigationSound();
    }

    private void Update()
    {
        HandleKeyboardNavigation();

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
        int sceneIndex = currentSelectedIndex + 1;
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

        PlayNavigationSound();
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

    private void PlayNavigationSound()
    {
        if (navigationSound != null)
        {
            AudioSource.PlayClipAtPoint(navigationSound, Camera.main.transform.position);
        }
    }
}
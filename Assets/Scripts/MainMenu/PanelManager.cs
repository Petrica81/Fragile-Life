using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] waterSounds;

    [System.Serializable]
    public class Panel
    {
        public GameObject panelObject;
        public CanvasGroup canvasGroup;
        public Button triggerButton;
        public float fadeDuration = 0.3f;
        [HideInInspector] public bool isOpen;
    }

    [Header("Panels")]
    [SerializeField] private Panel[] panels;
    [SerializeField] private float panelSwitchDelay = 0.1f;

    private Panel currentOpenPanel;
    private Coroutine currentAnimation;

    private void Start()
    {
        // Initialize all panels
        foreach (var panel in panels)
        {
            panel.panelObject.SetActive(false);
            panel.canvasGroup.alpha = 0;
            panel.canvasGroup.interactable = false;
            panel.canvasGroup.blocksRaycasts = false;
            panel.isOpen = false;

            // Setup button click
            panel.triggerButton.onClick.AddListener(() => TogglePanel(panel));
        }
    }

    public void TogglePanel(Panel panel)
    {
        // Play sound immediately when any button is pressed
        PlayRandomWaterSound();

        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }

        // If clicking the currently open panel, close it
        if (panel.isOpen)
        {
            currentAnimation = StartCoroutine(ClosePanel(panel));
            currentOpenPanel = null;
            return;
        }

        // If another panel is open, close it first
        if (currentOpenPanel != null)
        {
            StartCoroutine(SwitchPanels(currentOpenPanel, panel));
        }
        else
        {
            currentAnimation = StartCoroutine(OpenPanel(panel));
        }
    }

    private IEnumerator OpenPanel(Panel panel)
    {
        // Disable all buttons during transition
        SetAllButtonsInteractable(false);

        panel.panelObject.SetActive(true);
        panel.isOpen = true;
        currentOpenPanel = panel;

        float elapsed = 0f;
        while (elapsed < panel.fadeDuration)
        {
            panel.canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / panel.fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panel.canvasGroup.alpha = 1f;
        panel.canvasGroup.interactable = true;
        panel.canvasGroup.blocksRaycasts = true;

        // Re-enable buttons
        SetAllButtonsInteractable(true);
    }

    public IEnumerator ClosePanel(Panel panel)
    {
        SetAllButtonsInteractable(false);

        panel.canvasGroup.interactable = false;
        panel.canvasGroup.blocksRaycasts = false;
        panel.isOpen = false;

        float elapsed = 0f;
        float startAlpha = panel.canvasGroup.alpha;

        while (elapsed < panel.fadeDuration)
        {
            panel.canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / panel.fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        panel.canvasGroup.alpha = 0f;
        panel.panelObject.SetActive(false);
        currentOpenPanel = null;

        SetAllButtonsInteractable(true);
    }

    private IEnumerator SwitchPanels(Panel panelToClose, Panel panelToOpen)
    {
        yield return StartCoroutine(ClosePanel(panelToClose));
        yield return new WaitForSeconds(panelSwitchDelay);
        yield return StartCoroutine(OpenPanel(panelToOpen));
    }

    private void SetAllButtonsInteractable(bool state)
    {
        foreach (var panel in panels)
        {
            panel.triggerButton.interactable = state;
        }
    }

    public void ClosePanelByIndex(int panelIndex)
    {
        if (panelIndex >= 0 && panelIndex < panels.Length)
        {
            // Play sound for non-button triggered closes
            PlayRandomWaterSound();

            if (currentAnimation != null)
            {
                StopCoroutine(currentAnimation);
            }
            currentAnimation = StartCoroutine(ClosePanel(panels[panelIndex]));
            currentOpenPanel = null;
        }
        else
        {
            Debug.LogError($"Invalid panel index: {panelIndex}");
        }
    }

    public void CancelQuit()
    {
        ClosePanelByIndex(2);
    }

    public void ConfirmQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void PlayRandomWaterSound()
    {
        if (waterSounds != null && waterSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, waterSounds.Length);
            AudioSource.PlayClipAtPoint(waterSounds[randomIndex], Camera.main.transform.position);
        }
    }
}
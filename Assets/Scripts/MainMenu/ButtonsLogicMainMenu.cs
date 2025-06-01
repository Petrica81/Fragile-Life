using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonsLogicMainMenu : MonoBehaviour
{
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;
    PanelManager panelManager;

    private void Start()
    {
        
    }


    private void ConfirmQuit()
    {
        #if UNITY_EDITOR // Checks if the code is running in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // private void CancelQuit() -> am mutat in PanelManager functia asta
    // {
    //     panelManager.ClosePanel(confirmationPanel);
    // }
}

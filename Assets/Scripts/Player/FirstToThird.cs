using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class FirstToThird : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _text;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController cameraController = Camera.main.GetComponentInChildren<CameraController>();
            if 
                (
                cameraController != null && 
                cameraController.isFirstUnlockFrame == false && 
                cameraController.currentMode == CameraController.CameraMode.FirstPerson
                )
            {
                cameraController.isFirstUnlockFrame = true;
                cameraController.currentMode = CameraController.CameraMode.ThirdPerson;
                cameraController.viewLocked = false;
                StartCoroutine(ShowText());
            }
            else
            {
                Debug.LogWarning("CameraController not found on player!");
            }
        }
    }

    private IEnumerator ShowText()
    {
        _text.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        _text.gameObject.SetActive(false);
        enabled = false; // Disable this script after showing the text
    }
}

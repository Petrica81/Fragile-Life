using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FirstToThird : MonoBehaviour
{
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
                this.enabled = false; // Disable this script after switching to third person
            }
            else
            {
                Debug.LogWarning("CameraController not found on player!");
            }
        }
    }
}

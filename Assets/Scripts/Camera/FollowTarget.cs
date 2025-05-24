using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;

    [SerializeField] private float _rotationalSpeed = 30f;
    [SerializeField] private float _topClamp = 70f;
    [SerializeField] private float _bottomClamp = -40f;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    private void LateUpdate()
    {
        CameraLogic();
    }

    private void CameraLogic()
    {
        float mouseX = GetMouseInput("Mouse X");
        float mouseY = GetMouseInput("Mouse Y");

        _cinemachineTargetPitch = UpdateRotation(_cinemachineTargetPitch, mouseY, _bottomClamp, _topClamp, true);
        _cinemachineTargetYaw = UpdateRotation(_cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotations(_cinemachineTargetPitch, _cinemachineTargetYaw);
    }

    private void ApplyRotations(float pitch, float yaw)
    {
        _followTarget.rotation = Quaternion.Euler(pitch, yaw, _followTarget.eulerAngles.z);
    }
    private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis)
    {
        currentRotation += isXAxis ? -input : input;
        return Mathf.Clamp(currentRotation, min, max);
    }
    private float GetMouseInput(string axis)
    {
        return Input.GetAxis(axis) * _rotationalSpeed * Time.deltaTime;
    }
}

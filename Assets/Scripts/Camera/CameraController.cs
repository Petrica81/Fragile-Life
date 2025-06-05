using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode { FirstPerson, ThirdPerson, Auto }

    [Header("Camera Settings")]
    public CameraMode currentMode = CameraMode.ThirdPerson;
    public bool viewLocked = false;
    public Transform player;
    public float switchSmoothness = 5f;
    public float returnSpeed = 2f; // Viteza de revenire după obstacol

    [Header("First Person Settings")]
    public Vector3 firstPersonOffset = new Vector3(0, 1.7f, 0.2f);

    [Header("Third Person Settings")]
    public Vector3 thirdPersonOffset = new Vector3(0, 2f, -3f);
    public float minDistance = 1f;
    public float maxDistance = 5f;
    public float scrollSensitivity = 1f;
    public float rotationSpeed = 3f;
    public LayerMask collisionMask;

    private float currentDistance;
    private Vector3 desiredPosition;
    private Camera mainCamera;
    private float xRotation;
    private float yRotation;
    private bool isFirstUnlockFrame = false;
    private Vector3 targetThirdPersonOffset; // Offset-ul țintă (fără obstacole)
    private Vector3 currentThirdPersonOffset; // Offset-ul actual (cu ajustări pentru obstacole)
    private bool isAvoidingObstacle = false;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        currentDistance = maxDistance;
        xRotation = player.eulerAngles.y;
        yRotation = 15f;
        targetThirdPersonOffset = thirdPersonOffset;
        currentThirdPersonOffset = thirdPersonOffset;

        if (player == null)
        {
            Debug.LogError("Player transform not assigned to CameraController!");
            enabled = false;
        }
    }

    void Update()
    {
        HandleInput();
    }

    void LateUpdate()
    {
        if (viewLocked && isFirstUnlockFrame)
        {
            isFirstUnlockFrame = false;
            return;
        }

        switch (currentMode)
        {
            case CameraMode.FirstPerson:
                UpdateFirstPerson();
                break;
            case CameraMode.ThirdPerson:
                UpdateThirdPerson();
                break;
            case CameraMode.Auto:
                UpdateAutoMode();
                break;
        }
    }

    void HandleInput()
    {
        // Rotire cameră (doar în Third Person sau Auto)
        if ((currentMode == CameraMode.ThirdPerson || currentMode == CameraMode.Auto) && !viewLocked)
        {
            if (Input.GetMouseButton(1)) // Right-click hold to rotate
            {
                xRotation += Input.GetAxis("Mouse X") * rotationSpeed;
                yRotation -= Input.GetAxis("Mouse Y") * rotationSpeed;
                yRotation = Mathf.Clamp(yRotation, -20f, 80f);
            }
        }

        // Scroll pentru distanță
        if ((currentMode == CameraMode.ThirdPerson || currentMode == CameraMode.Auto) && !viewLocked)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            currentDistance = Mathf.Clamp(currentDistance - scroll * scrollSensitivity, minDistance, maxDistance);

            if (currentDistance <= minDistance + 0.1f && currentMode == CameraMode.Auto)
            {
                SetCameraMode(CameraMode.FirstPerson);
            }
            else if (currentDistance > minDistance + 0.1f && currentMode == CameraMode.Auto)
            {
                SetCameraMode(CameraMode.ThirdPerson);
            }
        }

        // Comenzi de schimbare mod (ignoră dacă e viewLocked)
        if (Input.GetKeyDown(KeyCode.F) && !viewLocked)
        {
            ToggleCameraMode();
        }

        // Lock/unlock view
        if (Input.GetKeyDown(KeyCode.L))
        {
            viewLocked = !viewLocked;
            isFirstUnlockFrame = true;
            Debug.Log("View " + (viewLocked ? "locked" : "unlocked"));
        }
    }

    void UpdateFirstPerson()
    {
        transform.position = player.TransformPoint(firstPersonOffset);
        transform.rotation = player.rotation;
    }

    void UpdateThirdPerson()
    {
        // Calculează offset-ul țintă (fără obstacole)
        targetThirdPersonOffset = new Vector3(0, thirdPersonOffset.y, -currentDistance);

        // Verifică coliziuni
        RaycastHit hit;
        Vector3 rayDirection = (player.position + targetThirdPersonOffset) - player.position;
        if (Physics.Raycast(player.position, rayDirection.normalized, out hit, rayDirection.magnitude, collisionMask))
        {
            // Ajustează offset-ul pentru a evita obstacolul
            currentThirdPersonOffset = hit.point - player.position + hit.normal * 0.2f;
            isAvoidingObstacle = true;
        }
        else if (isAvoidingObstacle)
        {
            // Revine treptat la offset-ul inițial
            currentThirdPersonOffset = Vector3.Lerp(currentThirdPersonOffset, targetThirdPersonOffset, Time.deltaTime * returnSpeed);
            if (Vector3.Distance(currentThirdPersonOffset, targetThirdPersonOffset) < 0.1f)
            {
                currentThirdPersonOffset = targetThirdPersonOffset;
                isAvoidingObstacle = false;
            }
        }
        else
        {
            currentThirdPersonOffset = targetThirdPersonOffset;
        }

        // Aplică rotația camerei
        Quaternion rotation = Quaternion.Euler(yRotation, xRotation, 0);
        desiredPosition = player.position + rotation * currentThirdPersonOffset;

        // Mișcă camera
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * switchSmoothness);
        transform.LookAt(player.position + Vector3.up * 1.5f);
    }

    void UpdateAutoMode()
    {
        UpdateThirdPerson();
    }

    public void ToggleCameraMode()
    {
        if (viewLocked) return;

        if (currentMode == CameraMode.FirstPerson)
        {
            SetCameraMode(CameraMode.ThirdPerson);
            xRotation = player.eulerAngles.y;
            yRotation = 15f;
        }
        else
        {
            SetCameraMode(CameraMode.FirstPerson);
        }
    }

    public void SetCameraMode(CameraMode newMode)
    {
        if (viewLocked) return;
        currentMode = newMode;
    }

    // Funcții pentru UI
    public void ToggleViewLock() => viewLocked = !viewLocked;
    public void SetFirstPerson() => SetCameraMode(CameraMode.FirstPerson);
    public void SetThirdPerson()
    {
        SetCameraMode(CameraMode.ThirdPerson);
        xRotation = player.eulerAngles.y;
        yRotation = 15f;
    }
    public void SetAutoMode() => SetCameraMode(CameraMode.Auto);
}
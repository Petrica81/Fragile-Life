using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraMode { FirstPerson, ThirdPerson, Auto }

    [Header("Camera Settings")]
    public CameraMode currentMode = CameraMode.ThirdPerson;
    public bool viewLocked = false;
    public Transform player;
    public float switchSmoothness = 5f;
    public float returnSpeed = 2f;
    public float autoRotationSpeed = 5f; // Viteza de rotire automată când player-ul se mișcă

    [Header("First Person Settings")]
    public Vector3 firstPersonOffset = new Vector3(0, 1.7f, 0.2f);

    [Header("Third Person Settings")]
    public Vector3 thirdPersonOffset = new Vector3(0, 2f, -3f);
    public float minDistance = 1f;
    public float maxDistance = 5f;
    public float scrollSensitivity = 1f;
    public float manualRotationSpeed = 3f; // Viteza de rotire manuală (click-dreapta)
    public LayerMask collisionMask;

    private float currentDistance;
    private Vector3 desiredPosition;
    private Camera mainCamera;
    private float xRotation;
    private float yRotation;
    public bool isFirstUnlockFrame = false;
    private Vector3 targetThirdPersonOffset;
    private Vector3 currentThirdPersonOffset;
    private bool isAvoidingObstacle = false;
    private Vector3 lastPlayerPosition;
    private bool isPlayerMoving = false;
    private float movementThreshold = 0.1f; // Distanța minimă pentru a detecta mișcarea

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        currentDistance = maxDistance;
        xRotation = player.eulerAngles.y;
        yRotation = 15f;
        targetThirdPersonOffset = thirdPersonOffset;
        currentThirdPersonOffset = thirdPersonOffset;
        lastPlayerPosition = player.position;

        if (player == null)
        {
            Debug.LogError("Player transform not assigned to CameraController!");
            enabled = false;
        }
    }

    void Update()
    {
        HandleInput();
        CheckPlayerMovement();
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

    void CheckPlayerMovement()
    {
        // Verifică dacă player-ul s-a mișcat semnificativ
        float distanceMoved = Vector3.Distance(lastPlayerPosition, player.position);
        isPlayerMoving = distanceMoved > movementThreshold;
        lastPlayerPosition = player.position;
    }

    void HandleInput()
    {
        // Rotire manuală (doar dacă player-ul NU se mișcă sau se ține click-dreapta)
        if ((currentMode == CameraMode.ThirdPerson || currentMode == CameraMode.Auto) && !viewLocked)
        {
            if (Input.GetMouseButton(1)) // Dacă se ține click-dreapta, rotire manuală are prioritate
            {
                xRotation += Input.GetAxis("Mouse X") * manualRotationSpeed;
                yRotation -= Input.GetAxis("Mouse Y") * manualRotationSpeed;
                yRotation = Mathf.Clamp(yRotation, -20f, 80f);
            }
            else if (isPlayerMoving) // Dacă player-ul se mișcă, rotire automată
            {
                // Aliniază camera cu direcția player-ului (doar pe axa Y)
                float targetRotation = player.eulerAngles.y;
                xRotation = Mathf.LerpAngle(xRotation, targetRotation, autoRotationSpeed * Time.deltaTime);
            }
        }

        // Zoom cu scroll wheel
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

        // Toggle camera mode (ignoră dacă viewLocked)
        if (Input.GetKeyDown(KeyCode.F) && !viewLocked)
        {
            ToggleCameraMode();
        }

        // Toggle view lock
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
            currentThirdPersonOffset = hit.point - player.position + hit.normal * 0.2f;
            isAvoidingObstacle = true;
        }
        else if (isAvoidingObstacle)
        {
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
            xRotation = player.eulerAngles.y; // Resetează la rotirea player-ului
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
        xRotation = player.eulerAngles.y; // Resetează la rotirea player-ului
        yRotation = 15f;
    }
    public void SetAutoMode() => SetCameraMode(CameraMode.Auto);
}
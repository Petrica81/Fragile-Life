using UnityEngine;

public class InsectPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float sprintSpeed = 6f; // New sprint speed
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float staminaMax = 100f; // Maximum stamina
    [SerializeField] private float staminaDrainRate = 20f; // Stamina drain per second while sprinting
    [SerializeField] private float staminaRegenRate = 15f; // Stamina regen per second when not sprinting

    [SerializeField] private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private float currentStamina;
    private bool isSprinting = false;

    private void Awake()
    {
        // Automatically get components if not assigned
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
        {
            cameraTransform = GetComponentInChildren<Camera>()?.transform;
            if (cameraTransform == null)
                Debug.LogError("No camera found in children!");
        }

        // Lock cursor for first-person control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentStamina = staminaMax; // Initialize stamina
    }

    private void Start()
    {
        Debug.Log($"Controller: {controller != null}");
        Debug.Log($"Camera Transform: {cameraTransform != null}");
        Debug.Log($"Camera Component: {cameraTransform?.GetComponent<Camera>() != null}");
        cameraTransform.gameObject.SetActive(false);
        cameraTransform.gameObject.SetActive(true); // Forces refresh

        GameObject overlayQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        overlayQuad.name = "InsectVisionOverlay";
        overlayQuad.transform.SetParent(cameraTransform);
        overlayQuad.transform.localPosition = new Vector3(0, 0, 0.3f); // 30cm in front of camera
        overlayQuad.transform.localRotation = Quaternion.identity;
        Destroy(overlayQuad.GetComponent<Collider>()); // Remove unnecessary collider
    }

    void Update()
    {
        // Safety check
        if (controller == null || cameraTransform == null)
            return;

        HandleMovement();
        HandleLook();
        HandleSprint();
        UpdateStamina();
    }

    private void HandleMovement()
    {
        if (!controller.isGrounded)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else
        {
            Vector3 moveInput = new Vector3(
                Input.GetAxis("Horizontal"),
                0,
                Input.GetAxis("Vertical")
            );

            Vector3 moveDirection = transform.TransformDirection(moveInput);

            // Use sprint speed if sprinting and has stamina, otherwise use normal speed
            float currentSpeed = isSprinting && currentStamina > 0 ? sprintSpeed : moveSpeed;
            velocity = moveDirection * currentSpeed;

            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
            }
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleLook()
    {
        // Horizontal rotation (turning the whole insect)
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        transform.Rotate(0, mouseX, 0);

        // Vertical rotation (just the camera - insect can look up/down)
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleSprint()
    {
        // Check if player is trying to sprint (holding Left Shift)
        bool wantsToSprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Can only sprint if has stamina and is moving forward
        bool canSprint = currentStamina > 0 && Input.GetAxis("Vertical") > 0;

        isSprinting = wantsToSprint && canSprint;
    }

    void UpdateStamina()
    {
        if (isSprinting)
        {
            // Drain stamina while sprinting
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(currentStamina, 0); // Clamp to minimum 0
        }
        else
        {
            // Regenerate stamina when not sprinting
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, staminaMax); // Clamp to maximum
        }
    }

    // Optional: Add a method to get current stamina for UI display
    public float GetStaminaNormalized()
    {
        return currentStamina / staminaMax;
    }
}
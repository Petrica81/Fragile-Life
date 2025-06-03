using UnityEngine;

public class InsectPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;

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
    }

    private void Start()
    {
        Debug.Log($"Controller: {controller != null}");
        Debug.Log($"Camera Transform: {cameraTransform != null}");
        Debug.Log($"Camera Component: {cameraTransform?.GetComponent<Camera>() != null}");
        cameraTransform.gameObject.SetActive(false);
        cameraTransform.gameObject.SetActive(true); // Forces refresh
    }

    void Update()
    {
        // Safety check
        if (controller == null || cameraTransform == null)
            return;

        HandleMovement();
        HandleLook();
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
            velocity = moveDirection * moveSpeed;

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
}
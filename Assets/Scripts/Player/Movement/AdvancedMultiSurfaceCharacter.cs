using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AdvancedMultiSurfaceCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float _moveSpeed = 7f;
    public float _sprintMultiplier = 1.5f;
    public float _rotationSpeed = 15f;

    [Header("Surface Detection")]
    public float _surfaceCheckDistance = 0.6f;
    public LayerMask _walkableLayers;

    private Rigidbody _rb;
    private Vector3 _moveInput;
    private bool _isSprinting;
    private bool _grounded;

    private Vector3 _targetLocation;
    private Quaternion _targetRotation;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.freezeRotation = true;

        _targetLocation = transform.position;
        _targetRotation = transform.rotation;
        StartCoroutine(Trace());
    }

    void Update()
    {
        HandleInput();
        VerifyGroundedState();

    }

    void FixedUpdate()
    {
        Move();

        if (!_grounded)
            ApplyGravity();
    }

    void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        _moveInput = new Vector3(horizontal, 0, vertical).normalized;
        _isSprinting = Input.GetKey(KeyCode.LeftShift);
    }

    private void VerifyGroundedState()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, -transform.up, out hit, _surfaceCheckDistance, _walkableLayers);
        if(hit.collider)
            _grounded = true;
        else
            _grounded = false;
    }
    private void ApplyGravity()
    {
        transform.position += Physics.gravity * Time.deltaTime * _rb.mass;
    }

    private IEnumerator Trace()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            RaycastHit hit_f1 = TraceMovement(5, -5, 0);
            if (hit_f1.collider)
            {
                RaycastHit hit_f2 = TraceMovement(6, -5, 0);
                if (hit_f2.collider)
                {
                    RaycastHit hit_r = TraceMovement(5, -5, 1);
                    if (hit_r.collider)
                    {
                        _targetLocation = hit_f1.transform.position;

                        var forward = (hit_f2.transform.position - hit_f1.transform.position).normalized;
                        var right = (hit_r.transform.position - hit_f1.transform.position).normalized;
                        var up = Vector3.Cross(forward, right).normalized; 

                        _targetRotation = Quaternion.LookRotation(forward, up);
                    }
                }
            }
        }
    }

    private RaycastHit TraceMovement(float length, float height, float right)
    {   
        Vector3 raycastLocation = transform.position + GetMovementDirection() * length + transform.up * height + Quaternion.Euler(0, 90, 0) * GetMovementDirection() * right;
        RaycastHit hit;
        Physics.Linecast( transform.position, raycastLocation, out hit, _walkableLayers);
        Debug.DrawLine(transform.position, raycastLocation, Color.red);

        return hit;
    }

    Vector3 GetMovementDirection()
    {
        Vector3 forwardDirection = transform.forward * _moveInput.z;
        Vector3 rightDirection = transform.right * _moveInput.x;
        Vector3 movementDirection = (forwardDirection + rightDirection).normalized;

        return movementDirection;
    }

    private void Move()
    {
       if (Mathf.Abs(_moveInput.x) + Mathf.Abs(_moveInput.z) > 0 && Vector3.Distance(_targetLocation, transform.position) > 0.1)
       {
            Vector3 newPosition = Vector3.MoveTowards(
                _rb.position,
                _targetLocation,
                _moveSpeed * Time.fixedDeltaTime
            );
            _rb.MovePosition(newPosition);
            _rb.MoveRotation(Quaternion.Slerp(_rb.rotation, _targetRotation, _rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
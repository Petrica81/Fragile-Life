using UnityEngine;

public class BodyMovement : MonoBehaviour
{
    [SerializeField] private float _forwardSpeed;
    [SerializeField] private float _turnSpeed;
    [SerializeField] private float _correctionSpeed;
    [SerializeField] private float _heightCorrectionSpeed;
    [SerializeField] private float _castDepth;
    [SerializeField] private float _groundCastDepth;
    [SerializeField] private float _castHeightAboveBody;
    [SerializeField] private float _castPositionsAdvance;
    [SerializeField] private float _desiredGroundClearance;
    [SerializeField] private float _castSphereRadius;
    [SerializeField] private Transform _castPositionsTransform;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody _rigidbody;
    private Vector3 _forwardPoint;
    private Vector3 _averageNormal;
    private Vector3 _averagePosition;
    private Vector2 _input;
    private int _castPosition = -1;
    private RaycastHit[] _hits;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _input = new Vector2(horizontal, vertical);
        _castPositionsTransform.position = transform.position + (vertical * _castPositionsAdvance * transform.forward);

        OrientBody();
    }

    private void FixedUpdate()
    {
        _rigidbody.transform.Rotate(0, _input.x * _turnSpeed * Time.fixedDeltaTime, 0);
    }

    private void OrientBody()
    {
        RaycastHit hit = GetOrientationUp();

        if (hit.collider)
        {
            _forwardPoint = hit.point + (hit.normal * _desiredGroundClearance);
            transform.position = Vector3.MoveTowards(transform.position, _forwardPoint, _forwardSpeed);
            Quaternion desiredRotation = Quaternion.LookRotation(Vector3.Cross(transform.right, hit.normal), hit.normal);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, _turnSpeed * Time.deltaTime);
        }
        else
        {
            ApplyGravity();
        }
    }

    private RaycastHit GetOrientationUp()
    {
        Vector3 castOffset = (transform.up * _castHeightAboveBody) + transform.forward * _input.y * _forwardSpeed * Time.deltaTime;

        RaycastHit hit;
        Physics.SphereCast(
            transform.position + castOffset,
            _castSphereRadius,
            -transform.up,
            out hit,
            _castHeightAboveBody + _groundCastDepth,
            _groundLayer
            );
        Debug.DrawRay(transform.position + castOffset, -transform.up * (_castHeightAboveBody + _groundCastDepth), Color.magenta, 0.5f);
        return hit;
    }

    private void ApplyGravity()
    {
        transform.position += Physics.gravity * Time.deltaTime * _rigidbody.mass;
    }
}

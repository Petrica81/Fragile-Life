using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BodyMovement6Legs : MonoBehaviour
{
    [SerializeField][Tooltip("Viteza de miscare in fata.")]
    private float _forwardSpeed;
    [SerializeField][Tooltip("Viteza de rotatie pe axa Y.")]
    private float _turnSpeed;
    [SerializeField][Tooltip("Inaltimea in plus adaugata fata de picioare.")]
    private float _heightOffset;
    [SerializeField][Tooltip("Picioarele in ordine.\n[din fata in spate]\n[de la stanga la dreapta]")]
    private List<Transform> _legs;
    [SerializeField][Tooltip("Suprafata pe care se poate deplasa.")] 
    private LayerMask _walkableLayer;

    [SerializeField] private float _groundCastDepth;
    [SerializeField] private float _castHeightAboveBody;
    [SerializeField] private float _castPositionsAdvance;
    [SerializeField] private float _desiredGroundClearance;
    [SerializeField] private float _castSphereRadius;

    private Rigidbody _rigidbody;
    private Vector3 _averageNormal;
    private Vector3 _averagePosition;
    private Vector2 _input;
    private bool _grounded = true;
    private float _baseAngle;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        Vector3 directionToLeg = (_legs[4].position - _legs[0].position).normalized;
        _baseAngle = Vector3.SignedAngle(transform.right, directionToLeg, transform.up);

    }
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _input = new Vector2(horizontal, vertical);

        AverageValues();
        OrientBody();

        if (!_grounded)
            ApplyGravity();
    }

    private void FixedUpdate()
    {
        Vector3 frontDirection = transform.position + transform.forward * _input.y;
        frontDirection.y = _averagePosition.y;
        transform.position = Vector3.MoveTowards(transform.position, frontDirection, _forwardSpeed * Time.fixedDeltaTime);

        float angle = 0;
        if (_legs[4].GetComponent<Foot>()._currentPhase == StepPhases.RESTING &&
            _legs[0].GetComponent<Foot>()._currentPhase == StepPhases.RESTING)
        {
            Vector3 directionToLeg = (_legs[4].position - _legs[0].position).normalized;
            angle = Vector3.SignedAngle(transform.right, directionToLeg, transform.up) - _baseAngle;
        }
        transform.Rotate(0, _input.x * _turnSpeed * Time.fixedDeltaTime,0);
    }
    private void ApplyGravity()
    {
        transform.position += Physics.gravity * Time.deltaTime * _rigidbody.mass;
    }
    private void AverageValues()
    {
        _averagePosition = Vector3.zero;
        int numberOfLegsOnGround = 0;

        for (int i = 0; i < 6; i++)
        {
            var leg = _legs[i].GetComponent<Foot>()._footPlacementTarget.GetComponent<PlacementRaycast>().hit;
          
            if (leg.collider)
            {
                _averageNormal += leg.normal;
                _averagePosition += leg.point;
                numberOfLegsOnGround++;
            }
        }

        if (numberOfLegsOnGround > 0)
        {
            _averageNormal /= numberOfLegsOnGround;
            _averageNormal.Normalize();

            _averagePosition /= numberOfLegsOnGround;
            _averagePosition.y += _heightOffset;
        }

        if (numberOfLegsOnGround <= 2)
            _grounded = false;
        else
            _grounded = true;
    }

    private void OrientBody()
    {
        RaycastHit hit = GetOrientationUp();

        if (hit.collider)
        {
            var _forwardPoint = hit.point + (hit.normal * _desiredGroundClearance);
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
            _walkableLayer
            );

        return hit;
    }
}



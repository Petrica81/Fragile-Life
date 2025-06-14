﻿using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

public class BodyMovement6Legs : MonoBehaviour
{
    [SerializeField][Tooltip("Viteza de miscare in fata normala.")]
    private float _baseSpeed;
    [HideInInspector]
    public float _speed;
    [SerializeField][Tooltip("Viteza de miscare in fata cand alearga.")]
    private float _sprintSpeed;
    [SerializeField][Tooltip("Viteza de rotatie pe axa Y.")]
    private float _turnSpeed;
    //[Tooltip("Viteza de rotatie pe axa X.")]
    private float _flipSpeed;
    [SerializeField][Tooltip("Inaltimea in plus adaugata fata de picioare.")]
    private float _heightOffset;
    [SerializeField][Tooltip("Picioarele in ordine.\n[din fata in spate]\n[de la stanga la dreapta]")]
    private List<Transform> _legs;
    [SerializeField][Tooltip("Suprafata pe care se poate deplasa.")] 
    private LayerMask _walkableLayer;

    [SerializeField] private float _castPositionsAdvance;
    [SerializeField] private float _desiredGroundClearance;
    [SerializeField] private float _castSphereRadius;

    private Vector3 _externalForce;
    private Rigidbody _rigidbody;
    public Vector2 _input;
    private bool _grounded = true;
    private Vector3 _targetLocation;
    private Quaternion _targetRotation;
    // temporar points
    private Vector3 _above;
    private Vector3 _below;
    private Vector3 _forward;
    private Vector3 _lastHit = Vector3.zero;
    private Quaternion _startRotation;
    private Vector3 _lastPosition;
    [HideInInspector]
    public bool _moved = false;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _targetLocation = transform.position;
        _startRotation = transform.rotation;
        _lastPosition = transform.position;
        _speed = _baseSpeed;
        _externalForce = Vector3.zero;
    }
    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        _above = transform.position + (transform.forward * 0.1f) + (transform.up * _heightOffset * 0.5f);
        _below = transform.position + (transform.forward * 0.1f) - (transform.up * _heightOffset * 1.1f);
        _forward = transform.position + (transform.forward * _castPositionsAdvance);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _speed = _sprintSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _speed = _baseSpeed;
        }

        if (!_grounded)
        {
            _targetRotation = _startRotation;
        }
        else
        { 
            _targetRotation = transform.rotation; 
        }
        _flipSpeed = _speed * 0.75f;
        _input = new Vector2(horizontal, vertical);

        RaycastHit hit_fu = CheckBetween(_above, _forward);
        RaycastHit hit_fb = CheckBetween(_forward, _below);
        if (hit_fu.collider && _input.y > 0.1f && Vector3.Distance(hit_fu.point, _lastHit) > 0.2f)
        {
            _targetLocation = hit_fu.point + hit_fu.normal * _heightOffset;

            _targetRotation = Quaternion.FromToRotation(transform.up, hit_fu.normal) * transform.rotation;
        }
        else if (hit_fb.collider && _input.y > 0.1f && Vector3.Distance(hit_fb.point, _lastHit) > 0.2f)
        {
            _targetLocation = hit_fb.point + hit_fb.normal * _heightOffset;

            _targetRotation = Quaternion.FromToRotation(transform.up, hit_fb.normal) * transform.rotation;
        }

        _targetLocation += _externalForce;

        if (IsDirectionBlocked(_targetLocation - transform.position) || IsDirectionBlocked(_targetLocation - transform.position - Vector3.right * 0.3f) || IsDirectionBlocked(_targetLocation - transform.position + Vector3.right * 0.3f))
        {
            _targetLocation = transform.position;
            _targetRotation = transform.rotation;
        }
        
        if (_lastPosition != transform.position || _input.x != 0)
        {
            _moved = true;
        }
        else
        {
            _moved = false;
        }

        _lastPosition = transform.position;
        VerifyOnTerrain();

    }

    private void FixedUpdate()
    {

        if (!_grounded)
            ApplyGravity();
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetLocation, _speed * Time.fixedDeltaTime);

            _externalForce = Vector3.zero;

            transform.Rotate(0, _input.x * _turnSpeed * Time.fixedDeltaTime, 0);

        }
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetRotation, _flipSpeed * Time.fixedDeltaTime);
    }

    private RaycastHit CheckBetween(Vector3 pointA, Vector3 pointB)
    {
        RaycastHit hit;

        Physics.SphereCast(
            pointA,
            _castSphereRadius,
            (pointB - pointA).normalized,
            out hit,
            Vector3.Distance(pointA, pointB),
            _walkableLayer
        );

        Debug.DrawRay(pointA, (pointB - pointA).normalized * Vector3.Distance(pointA, pointB), hit.collider ? Color.green : Color.red);
        
        return hit;
    }

    private void ApplyGravity()
    {
        transform.position += Physics.gravity * Time.fixedDeltaTime * _rigidbody.mass / 2;
    }

    private void VerifyOnTerrain()
    {
        RaycastHit hit = GetPointBelow();

        if (hit.collider)
        {
            _grounded = true;
        }
        else
        {
            _grounded = false;
        }
    }

    private RaycastHit GetPointBelow()
    {
        RaycastHit hit;
        Physics.SphereCast(
            transform.position,
            _castSphereRadius,
            -transform.up,
            out hit,
            _heightOffset,
            _walkableLayer
            );

        Debug.DrawRay(
            transform.position ,
            -transform.up * _heightOffset,
            hit.collider ? Color.green : Color.red
        );
        return hit;
    }

    public void ApplyExternalForce(Vector3 force)
    {
        _externalForce += force;
    }
    bool IsDirectionBlocked(Vector3 direction)
    {
        RaycastHit hit;
        Physics.SphereCast(transform.position, 1, direction.normalized, out hit, 2, ~_walkableLayer & ~(1 << gameObject.layer), QueryTriggerInteraction.Ignore);
        Debug.DrawRay(transform.position, direction.normalized * 2, hit.collider ? Color.green : Color.red);
        if (hit.collider)
        {
            return true;
        }
        return false;
    }
}



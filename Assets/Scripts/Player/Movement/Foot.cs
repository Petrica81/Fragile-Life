// Modified version with improvements
using UnityEngine;
using UnityEngine.Events;

public enum StepPhases
{
    RESTING,
    MOVING_TO_TARGET,
    MOVING_TO_LIFT
}

public class Foot : MonoBehaviour
{
    [Header("Settings")]
    public Transform _footPlacementTarget;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private float _stepSize = 0.5f;
    [SerializeField] private float _stepSpeed = 5f;
    [SerializeField] private float _stepHeight = 0.3f;
    [SerializeField] private float _minDistanceTolerance = 0.01f;
    [SerializeField] private Foot _opposingFoot;

    [Header("Debug")]
    [SerializeField] private bool _debugDraw = true;

    private PlacementRaycast _placementRaycast;
    private Vector3 _targetPosition;
    private BodyMovement6Legs _bodyMovement;

    public StepPhases _currentPhase = StepPhases.MOVING_TO_TARGET;
    public UnityEvent<bool> OnPlantedChange;

    private void Start()
    {
        _placementRaycast = _footPlacementTarget.GetComponent<PlacementRaycast>();
        _targetPosition = _footPlacementTarget.position;
        _bodyMovement = _bodyTransform.GetComponent<BodyMovement6Legs>();
    }

    private void Update()
    {
        UpdateStepLogic();

        if (_bodyMovement._moved)
            Move();

        if (_debugDraw)
        {
            Debug.DrawLine(transform.position, _targetPosition, Color.red);
            Debug.DrawLine(_targetPosition, _targetPosition + Vector3.up * 0.1f, Color.green);
        }
    }

    private void UpdateStepLogic()
    {
        float distanceToTarget = Vector3.Distance(transform.position, _footPlacementTarget.position);
        bool opposingFootReady = _opposingFoot == null || _opposingFoot._currentPhase == StepPhases.RESTING;

        // Initiate step if needed
        if (distanceToTarget > _stepSize && opposingFootReady && _currentPhase == StepPhases.RESTING)
        {
            _targetPosition = GetLiftPosition();
            _currentPhase = StepPhases.MOVING_TO_LIFT;
            OnPlantedChange?.Invoke(false);
        }

        // Check if reached lift position
        if (_currentPhase == StepPhases.MOVING_TO_LIFT &&
            Vector3.Distance(transform.position, _targetPosition) < _minDistanceTolerance)
        {
            _targetPosition = _placementRaycast.GetHitPoint();
            _currentPhase = StepPhases.MOVING_TO_TARGET;
        }

        // Check if reached target position
        if (_currentPhase == StepPhases.MOVING_TO_TARGET &&
            Vector3.Distance(transform.position, _targetPosition) < _minDistanceTolerance)
        {
            _currentPhase = StepPhases.RESTING;
            OnPlantedChange?.Invoke(true);
        }
    }

    private void Move()
    {
        if (_currentPhase != StepPhases.RESTING)
        {
            // Use smoother movement with potential acceleration
            float step = _bodyMovement._speed * _stepSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, step);
        }
    }

    private Vector3 GetLiftPosition()
    {
        Vector3 direction = (_footPlacementTarget.position - transform.position).normalized;
        float halfDistance = Vector3.Distance(transform.position, _footPlacementTarget.position) * 0.5f;
        Vector3 midPoint = transform.position + (direction * halfDistance);
        return midPoint + (_bodyTransform.up * _stepHeight);
    }
}
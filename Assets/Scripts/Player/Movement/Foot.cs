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
    public Transform _footPlacementTarget;
    [SerializeField] private Transform _bodyTransform;
    [SerializeField] private float _stepSize;
    [SerializeField] private float _stepSpeed;
    [SerializeField] private float _stepHeight;
    [SerializeField] private float _minDistanceTolerance;
    [SerializeField] private Foot _opposingFoot;

    private PlacementRaycast _placementRaycast;
    private Vector3 _targetPosition = Vector3.zero;

    public StepPhases _currentPhase = StepPhases.MOVING_TO_TARGET;
    public UnityEvent<bool> OnPlantedChange;
    private BodyMovement6Legs _bodyMovement;

    // Start is called before the first frame update
    void Start()
    {
        _placementRaycast = _footPlacementTarget.GetComponent<PlacementRaycast>();
        _targetPosition = _footPlacementTarget.position;
        _bodyMovement = _bodyTransform.GetComponent<BodyMovement6Legs>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, _footPlacementTarget.position) > _stepSize && _opposingFoot._currentPhase == StepPhases.RESTING)
        {
            _targetPosition = GetLiftPosition();
            _currentPhase = StepPhases.MOVING_TO_LIFT;
            OnPlantedChange?.Invoke(false);
        }
        if (Vector3.Distance(transform.position, _targetPosition) < _minDistanceTolerance && _currentPhase == StepPhases.MOVING_TO_LIFT)
        {
            _targetPosition = _placementRaycast.GetHitPoint();
            _currentPhase = StepPhases.MOVING_TO_TARGET;
        }
        if (Vector3.Distance(transform.position, _targetPosition) < _minDistanceTolerance && _currentPhase == StepPhases.MOVING_TO_TARGET)
        {
            _currentPhase = StepPhases.RESTING;
            OnPlantedChange?.Invoke(true);
        }
        if (_bodyMovement._input != Vector2.zero)
            Move();
    }

    private void Move()
    {
        if (_currentPhase != StepPhases.RESTING)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _stepSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetLiftPosition()
    {
        Vector3 midPointDistance = (_footPlacementTarget.position - transform.position) / 2;
        Vector3 midPoint = transform.position + midPointDistance;
        Vector3 liftPoint = midPoint + (_bodyTransform.up * _stepHeight);
        return liftPoint;
    }
}

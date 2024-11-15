using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class BodyPlacement : MonoBehaviour
{
    //[SerializeField] private List<Transform> _legTargets;
    //[SerializeField][Tooltip("Inaltimea fata de media picioarelor")] 
    //private float _heightOffset = 2.0f;
    //[SerializeField] private float _forwardSpeed = 5.0f;
    //[SerializeField] private float _turnSpeed = 100.0f;
    //[SerializeField] private float _gravityForce = 9.8f;

    //private BodyMovement _bodyMovement;
    //private Vector3 _forwardPoint;
    //private Vector3 _averagePosition;
    //private Vector3 _averageNormal;

    //private void Start()
    //{
    //    _bodyMovement = GetComponent<BodyMovement>();
    //}
    //// Update is called once per frame
    //void Update()
    //{
    //    CalculateNormals();
    //    if (_bodyMovement.grounded)
    //    {
    //        OrientBodyHeight();
    //        OrientBodyRotation();
    //    }
    //}

    //private void CalculateNormals()
    //{
    //    _averagePosition = Vector3.zero;
    //    _averageNormal = Vector3.zero;
    //    int numberOfHits = 0;
    //    foreach (Transform t in _legTargets)
    //    {
    //        if (Physics.Raycast(t.position, Vector3.down, out RaycastHit hit, 1))
    //        {
    //            _averagePosition += hit.point;
    //            _averageNormal += hit.normal;
    //            numberOfHits++;
    //        }
    //    }
    //    _averagePosition /= _legTargets.Count;
    //    _averageNormal /= _legTargets.Count;
    //    if (numberOfHits <= 2)
    //        _bodyMovement.grounded = false;
    //    else
    //        _bodyMovement.grounded = true;

    //    _forwardPoint = _averagePosition + (_averageNormal * _heightOffset);
    //}
    //private void OrientBodyHeight()
    //{
    //    transform.position = Vector3.MoveTowards(transform.position, _forwardPoint, _forwardSpeed);
    //}

    //private void OrientBodyRotation()
    //{
    //    Quaternion desiredRotation = Quaternion.LookRotation(Vector3.Cross(transform.forward, _averageNormal), _averageNormal);
    //    transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, _turnSpeed * Time.deltaTime);
    //}
}

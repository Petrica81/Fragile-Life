using UnityEngine;

public class PlacementRaycast : MonoBehaviour
{
    [SerializeField] private float _castDistance;
    [SerializeField] private float _castHeight;
    [SerializeField] private LayerMask _walkableLayer;

    private int _numberOfHits = 1;
    private Vector3 _hitPos = Vector3.zero;
    public RaycastHit hit;
    public Vector3 GetHitPoint()
    {
        Physics.SphereCast(
            transform.position + (transform.up * _castHeight),
            1f,
            -transform.up,
            out hit,
            _castDistance + _castHeight,
            _walkableLayer
            );
        Debug.DrawRay(transform.position + (transform.up * _castHeight), -transform.up * (_castDistance + _castHeight), Color.yellow, 0.5f);
        if (hit.collider != null)
        {
            _hitPos = hit.point;
        }
        else
        { 
            _hitPos = transform.position;// + (transform.up * _castHeight / 2);
        }
        return _hitPos;
    }
}

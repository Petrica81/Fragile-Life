using UnityEngine;

public class PlacementRaycast : MonoBehaviour
{
    [SerializeField] private float _castDistance;
    [SerializeField] private float _castHeight;
    [SerializeField] private LayerMask _walkableLayer;

    private int _numberOfHits = 1;
    private Vector3 _hitPos = Vector3.zero;

    public Vector3 GetHitPoint()
    {
        RaycastHit hit;
        Physics.SphereCast(
            transform.position + (transform.up * _castHeight),
            1f,
            -transform.up,
            out hit,
            _castDistance + _castHeight,
            _walkableLayer
            );
        if (hit.collider != null)
        {
            _hitPos = hit.point;
        }
        else
        {
            _hitPos = transform.position + (transform.up * _castHeight);
        }

        return _hitPos;
    }
}

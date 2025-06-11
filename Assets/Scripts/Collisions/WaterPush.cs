using UnityEngine;
using UnityEngine.Rendering.HighDefinition;


public class WaterPush : MonoBehaviour
{
    [SerializeField] private float pushForce = 10f; 
    private WaterSurface _waterSurface;

    private void Start()
    {
        _waterSurface = GetComponentInParent<WaterSurface>();
    }
    private void OnTriggerStay(Collider other)
    {
        BodyMovement6Legs bodyMovement;
        other.TryGetComponent<BodyMovement6Legs>(out bodyMovement);

        if (bodyMovement != null)
        {
            Vector3 push = this.transform.forward * _waterSurface.largeCurrentSpeedValue * pushForce * Time.deltaTime;

            bodyMovement.ApplyExternalForce(push);
        }
    }
}
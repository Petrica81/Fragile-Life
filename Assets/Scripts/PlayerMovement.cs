using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    //private Rigidbody rb;

    private void Start()
    {
       
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.forward * speed * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * speed * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * speed * Time.deltaTime;
        
        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * speed * Time.deltaTime;
        
    }
}

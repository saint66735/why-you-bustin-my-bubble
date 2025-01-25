using UnityEngine;
using UnityEngine.InputSystem.Controls;

enum State 
{
    Nothing,
    Crank,
    Wheel,
    Lever
}

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 10.0f;
    
    public Camera playerCamera;
    public ShipControl ship;
    
    private State state = State.Nothing;
    
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * speed, ForceMode.Acceleration);
        rb.AddTorque(Vector3.up * Input.GetAxis("Horizontal") * rotationSpeed, ForceMode.Acceleration);

        Controls();
    }

    void Controls()
    {
        if (Input.GetAxis("Fire1") > 0)
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(ray, out RaycastHit hit, 999f))
            { 
                //Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
                if (hit.collider.gameObject.CompareTag("Crank")) state = State.Crank;
                if (hit.collider.gameObject.CompareTag("Wheel")) state = State.Wheel;
            }
            else
            { 
                //Debug.DrawRay(ray.origin, ray.direction * 1000, Color.white); 
            }
            
            if (state == State.Crank)
            {
                ship.RotateCrank(Input.GetAxis("Mouse X"));
            }
            if (state == State.Wheel)
            {
                ship.RotateWheel(ray);
            }
        }
        else
        {
            state = State.Nothing;
        }
        
    }
}

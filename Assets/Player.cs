using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

enum State 
{
    Nothing,
    Crank,
    Wheel,
    Lever,
    Gun
}


public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 10.0f;
    public float drag = 0.01f;
    
    public Camera playerCamera;
    public ShipControl ship;
    
    private State state = State.Nothing;
    private Anchor targetedAnchor;
    private Vector3 previousShipVelocity = Vector3.zero;
    private Vector3 previousShipAngularVelocity = Vector3.zero;
    
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        
        rb.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * speed, ForceMode.Acceleration);
        rb.AddRelativeTorque(Vector3.up * Input.GetAxis("Horizontal") * rotationSpeed, ForceMode.Acceleration);
        //rb.AddRelativeForce(Vector3.down * 0.1f, ForceMode.Acceleration);
        
        //try to match ship speed
        //rb.linearVelocity += (ship.rb.linearVelocity - previousShipVelocity) + (ship.rb.linearVelocity - rb.linearVelocity) * drag;
        
        //rb.linearVelocity += (ship.rb.GetPointVelocity(transform.position) - previousShipVelocity) + (ship.rb.linearVelocity- rb.linearVelocity) * drag;
        //rb.angularVelocity += (ship.rb.angularVelocity - previousShipAngularVelocity) + (ship.rb.angularVelocity - rb.angularVelocity) * drag;
        
        //keep upright with ship
        //KeepUpright();
        
        Controls();
        
        previousShipAngularVelocity = ship.rb.angularVelocity;
        previousShipVelocity = ship.rb.GetPointVelocity(transform.position);//ship.rb.linearVelocity;
        
        
        
        rb.linearVelocity = ship.rb.GetPointVelocity(transform.position) + (previousShipVelocity - rb.linearVelocity) * drag;
        rb.angularVelocity = ship.rb.angularVelocity + (previousShipAngularVelocity - rb.angularVelocity) * drag;
    }
    
    void KeepUpright()
    {
        Quaternion deltaQuat = Quaternion.FromToRotation(transform.up, ship.gameObject.transform.up);

        Vector3 axis;
        float angle;
        deltaQuat.ToAngleAxis(out angle, out axis);
        float keepUprightFactor = 0.8f;
        rb.AddTorque(-rb.angularVelocity * keepUprightFactor, ForceMode.Acceleration);

        float adjustFactor = keepUprightFactor; // this value requires tuning
        rb.AddTorque(axis.normalized * angle * adjustFactor, ForceMode.Acceleration);
    }

    void Controls()
    {
        if (Input.GetAxis("Fire1") > 0)
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            { 
                //Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow);
                if (hit.collider.gameObject.CompareTag("Crank")) state = State.Crank;
                if (hit.collider.gameObject.CompareTag("Wheel")) state = State.Wheel;
                if (hit.collider.gameObject.CompareTag("Lever"))
                {
                    targetedAnchor = hit.collider.gameObject.GetComponentInParent<Anchor>();
                    state = State.Lever;
                }
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
            if (state == State.Lever)
            {
                targetedAnchor.MoveLever((playerCamera.ScreenToViewportPoint(Input.mousePosition).y-0.5f)*2f);
            }
        }
        else
        {
            state = State.Nothing;
        }
        
    }
}

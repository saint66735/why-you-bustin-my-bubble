using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 10.0f;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        rb.AddRelativeForce(Vector3.right * Input.GetAxis("Vertical") * speed, ForceMode.Acceleration);
        rb.AddTorque(Vector3.forward * Input.GetAxis("Horizontal") * rotationSpeed, ForceMode.Acceleration);
    }
}

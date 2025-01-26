using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ShipControl : NetworkBehaviour
{
    public float speedIncrease = 0.8f;
    public float crankMoveSpeed = 1f;
    public float engineDrag = 0.99f;
    public float bubbleFloatForce = 10f;
    public float gravity = 0.2f;
    public float turnSpeed = 0.05f;
    public float keepUprightFactor = 0.01f;

    public GameObject crank;
    public GameObject wheel;
    public List<GameObject> assingedBubbleAnchors;
    public List<GunControl> guns;

    public Rigidbody rb;
    private List<(Anchor, BalloonFloat)> bubbles;
    private float engineSpeed = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bubbles = assingedBubbleAnchors.Select(x => (x.GetComponent<Anchor>(), x.GetComponentInChildren<BalloonFloat>())).ToList();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.centerOfMass = transform.InverseTransformPoint(bubbles.Aggregate(Vector3.zero, (acc, x) => acc + x.Item1.transform.position) / bubbles.Count);
        engineSpeed *= engineDrag;

        Movement();
        
        VisualEffects();
    }
    
    void Movement()
    {
        //forward
        rb.AddForce(transform.forward * engineSpeed);
        
        //turning
        transform.Rotate(new Vector3(0,Mathf.Sin(wheel.transform.localRotation.eulerAngles.y*Mathf.PI / 2) * turnSpeed,0));
        
        //prevent rotation
        KeepUpright();
        
        //rotate ship on x due to rotation on y
            
        //gravity
        Debug.DrawRay(rb.centerOfMass, Vector3.down * gravity * 50f);
        rb.AddForce(Vector3.down * rb.mass * gravity);
       

        //up
        //FindAll(bubble => !bubble.Item1.isShot)
        bubbles.ForEach(bubble =>
        {
            BalloonFloat floating = bubble.Item2;
            Vector3 direction = Vector3.up * floating.SizeToVolume(floating.size) * bubbleFloatForce *
                                bubble.Item1.adjustBalloonForce;
            Debug.DrawRay(bubble.Item1.transform.position + direction * 200f,
                direction * 100f);
            rb.AddForceAtPosition(direction, bubble.Item1.transform.position);
        });
        
        //move wheel back
        Quaternion deltaQuat = wheel.transform.localRotation * Quaternion.FromToRotation(-transform.forward, wheel.transform.up);
        wheel.transform.localRotation = Quaternion.Lerp(wheel.transform.localRotation, Quaternion.FromToRotation(-transform.forward, wheel.transform.up), 0.05f);
        wheel.transform.localRotation = Quaternion.Euler(0, wheel.transform.localRotation.eulerAngles.y, 0);
    }

    void KeepUpright()
    {
        Quaternion deltaQuat = Quaternion.FromToRotation(transform.up, Vector3.up);

        Vector3 axis;
        float angle;
        deltaQuat.ToAngleAxis(out angle, out axis);
        
        rb.AddTorque(-rb.angularVelocity * keepUprightFactor, ForceMode.Acceleration);

        float adjustFactor = keepUprightFactor; // this value requires tuning
        rb.AddTorque(axis.normalized * angle * adjustFactor, ForceMode.Acceleration);
    }

    void VisualEffects()
    {
        crank.transform.localRotation *= Quaternion.Euler( Vector3.forward * engineSpeed * crankMoveSpeed);
    }


    public void RotateCrank(float amount)
    {
        engineSpeed += amount * speedIncrease;
    }

    public void RotateWheel(Ray ray)
    {
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        Plane plane = new Plane(transform.up, wheel.transform.position);
        //Plane plane = new Plane(wheel.transform.forward, wheel.transform.position);
        float enter = 0.0f;

        if (plane.Raycast(ray, out enter))
        {
            Debug.DrawRay(ray.origin, ray.direction * enter, Color.yellow);
            Vector3 intersection = ray.origin + (ray.direction * enter);
            Vector3 direction = (intersection - wheel.transform.position).normalized;
            
            wheel.transform.localRotation = Quaternion.FromToRotation(direction,wheel.transform.up); //new Vector3(Mathf.Atan2(direction.y, direction.x)* 180 / Mathf.PI,0,0 )
            //
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ShipControl : MonoBehaviour
{
    public float speedIncrease = 0.8f;
    public float crankMoveSpeed = 1f;
    public float engineDrag = 0.99f;
    public float bubbleFloatForce = 10f;
    public float gravity = 10f;

    public GameObject crank;
    public GameObject wheel;
    public List<GameObject> assingedBubbleAnchors;

    private Rigidbody rb;
    private List<(GameObject, BalloonFloat)> bubbles;
    private float engineSpeed = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bubbles = assingedBubbleAnchors.Select(x => (x, x.GetComponentInChildren<BalloonFloat>())).ToList();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        engineSpeed *= engineDrag;

        //Movement();
        VisualEffects();
    }
    
    void Movement()
    {
        //forward
        rb.AddForce(transform.forward * engineSpeed);
        
        //prevent rotation
        
        //gravity
        Vector3 center = bubbles.Aggregate(Vector3.zero, (acc, x) => acc + x.Item1.transform.position) / bubbles.Count;
        rb.AddForceAtPosition(Vector3.down * rb.mass,
            center);
       

        //up
        bubbles.ForEach(bubble =>
        {
            BalloonFloat floating = bubble.Item2;
            Debug.DrawRay(bubble.Item1.transform.position,
                Vector3.up * floating.SizeToVolume(floating.size) * bubbleFloatForce / 10f);
            rb.AddForceAtPosition(Vector3.up * floating.SizeToVolume(floating.size) * bubbleFloatForce,
                bubble.Item1.transform.position);
        });
    }
    
    void VisualEffects()
    {
        crank.transform.Rotate(crank.transform.forward * engineSpeed * crankMoveSpeed);
    }


    public void RotateCrank(float amount)
    {
        engineSpeed += amount * speedIncrease;
    }

    public void RotateWheel(Ray ray)
    {
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        Plane plane = new Plane(wheel.transform.right, wheel.transform.position);
        //Plane plane = new Plane(wheel.transform.forward, wheel.transform.position);
        float enter = 0.0f;

        if (plane.Raycast(ray, out enter))
        {
            //Debug.DrawRay(ray.origin, ray.direction * enter, Color.yellow);
            Vector3 intersection = ray.origin + (ray.direction * enter);
            Vector3 direction = (intersection - wheel.transform.position).normalized;

            wheel.transform.rotation = Quaternion.FromToRotation(Vector3.up, direction); //new Vector3(Mathf.Atan2(direction.y, direction.x)* 180 / Mathf.PI,0,0 )
            //Debug.DrawRay(wheel.transform.position, direction * 10f, Color.yellow);
        }
    }
}

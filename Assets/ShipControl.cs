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
    
    public GameObject crank;
    public GameObject wheel;

    private float engineSpeed = 0f;
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        crank.transform.Rotate( crank.transform.forward * engineSpeed * crankMoveSpeed);
        engineSpeed *= engineDrag;
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

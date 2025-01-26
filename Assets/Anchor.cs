using System;
using UnityEngine;

public class Anchor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject lever;
    public BalloonFloat balloon;

    public float adjustBalloonForce = 1.0f;
    public float shotCoolDown = 5f;
    public bool isShot = false;

    private float leverMotionRange = 0.5f;

    private float leverPosition = 0f;
    private float originalLeverPosition;
    
    private float shotCoolDownTimer = 0.0f;
    void Start()
    {
        originalLeverPosition = lever.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    void FixedUpdate()
    {
        if (isShot)
        {
            shotCoolDownTimer += Time.deltaTime;
            if (shotCoolDownTimer >= shotCoolDown)
            {
                isShot = false;
                //balloon.gameObject.SetActive(true);
            }
        }

        leverPosition = ((lever.transform.localPosition.y - originalLeverPosition) / leverMotionRange) / 2f;
        balloon.size = Mathf.Lerp(balloon.size, Mathf.Clamp(leverPosition + 1f, 0.2f, 2f), 0.02f) ;
    }

    public void MoveLever(float amount)
    {
        /*
        //Create a new plane with normal (0,0,1) at the position away from the camera you define in the Inspector. This is the plane that you can click so make sure it is reachable.
        Plane plane = new Plane(lever.transform.right, lever.transform.position);
        //Plane plane = new Plane(wheel.transform.forward, wheel.transform.position);
        float enter = 0.0f;

        if (plane.Raycast(ray, out enter))
        {
            Debug.DrawRay(ray.origin, ray.direction * enter, Color.yellow);
            
            Vector3 intersection = ray.origin + (ray.direction * enter);
            Debug.DrawRay(intersection, lever.transform.right * enter, Color.yellow);
            
            Vector3 localPos = intersection - lever.transform.localPosition;
            
            lever.transform.localPosition = new Vector3(lever.transform.localPosition.x, Mathf.Clamp(localPos.y, originalLeverPosition - leverMotionRange, originalLeverPosition + leverMotionRange),
                lever.transform.localPosition.z);
        }
        */
        lever.transform.localPosition = new Vector3(lever.transform.localPosition.x, originalLeverPosition + leverMotionRange * (Mathf.Clamp(amount, -0.5f, 0.5f)),
            lever.transform.localPosition.z);
    }

    public void GotHit()
    {
        Debug.Log("shot");
        isShot = true;
        shotCoolDownTimer = 0;
        adjustBalloonForce *= 0.9f;
        balloon.sizeToFloatCoefficient *= 0.9f;
        balloon.sizeToPhysicalSize *= 0.9f;
        //balloon.gameObject.SetActive(false);
    }
}

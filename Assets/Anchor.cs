using System;
using Unity.Netcode;
using UnityEngine;

public class Anchor : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject lever;
    public BalloonFloat balloon;
    public AudioSource hitSource;
    public AudioClip hitSound;
    public AudioClip hissSound;

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

    void FixedUpdate()
    {
        if (isShot)
        {
            shotCoolDownTimer += Time.deltaTime;
            if (shotCoolDownTimer >= shotCoolDown)
            {
                isShot = false;
            }
        }

        leverPosition = ((lever.transform.localPosition.y - originalLeverPosition) / leverMotionRange) / 2f;
        balloon.size = Mathf.Lerp(balloon.size, Mathf.Clamp(leverPosition + 1f, 0.2f, 2f), 0.02f) ;
    }

    public void MoveLever(float amount)
    {
        lever.transform.localPosition = new Vector3(lever.transform.localPosition.x, originalLeverPosition + leverMotionRange * (Mathf.Clamp(amount, -0.5f, 0.5f)),
            lever.transform.localPosition.z);
    }

    [Rpc(SendTo.Everyone)]
    public void GotHitRpc()
    {
        Debug.Log("shot");
        isShot = true;
        shotCoolDownTimer = 0;
        adjustBalloonForce *= 0.9f;
        balloon.sizeToFloatCoefficient *= 0.9f;
        balloon.sizeToPhysicalSize *= 0.9f;
        hitSource.PlayOneShot(hitSound);
        hitSource.PlayOneShot(hissSound);
        //balloon.gameObject.SetActive(false);
    }
}

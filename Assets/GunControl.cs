using System;
using NUnit.Framework.Constraints;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class GunControl : NetworkBehaviour
{

    public float reloadTime = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera localCamera;
    public VisualEffect boom;
    private AudioSource audioSource;
    public AudioClip audioClip;
    public float recoilForce = 50f;

    private Action exitCallback;
    private bool isShoot = false;
    private float lastShot = 0f;

    private Rigidbody ship;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ship = GetComponentInParent<ShipControl>().GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    public void MyFixedUpdateRpc(UserInputStruct t)
    {
        lastShot += Time.fixedDeltaTime;
        if (isShoot)
        {
            transform.localRotation *= Quaternion.Euler(0, t.horizontalInput, -t.verticalInput);
            transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
            if (t.fire && lastShot > reloadTime)
            {
                boom.Play();
                audioSource.PlayOneShot(audioClip);
                lastShot = 0;
                ship.AddForce(transform.right * recoilForce, ForceMode.Impulse);

                Ray ray = new Ray(localCamera.transform.position, localCamera.transform.forward);
                Debug.DrawRay(ray.origin, ray.direction * 50f, Color.white, 5f);
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 5f);
                    if (hit.collider.gameObject.CompareTag("Bubble"))
                    {
                        hit.collider.GetComponentInParent<Anchor>().GotHitRpc();
                    }
                }
            }

            if (t.jump)
            {
                localCamera.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                isShoot = false;
                exitCallback.Invoke();
            }
        }
    }

    public void startShootingRpc(Action callback)
    {
        localCamera.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        exitCallback = callback;
        isShoot = true;
        lastShot = reloadTime - 0.1f;
    }

    [Rpc(SendTo.Everyone)]
    public void FireRpc()
    {
        boom.Play();
        audioSource.PlayOneShot(audioClip);
        ship.AddForce(transform.right * recoilForce, ForceMode.Impulse);
    }
}

using System;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class GunControl : MonoBehaviour
{

    public float reloadTime = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Camera localCamera;
    public VisualEffect boom;
    private AudioSource audioSource;
    public AudioClip audioClip;

    private Action exitCallback;
    private bool isShoot = false;
    private float lastShot = 0f;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lastShot += Time.fixedDeltaTime;
        if (isShoot)
        {
            transform.localRotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
            transform.localRotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
            if (Input.GetButton("Fire1") && lastShot > reloadTime)
            {
                boom.Play();
                audioSource.PlayOneShot(audioClip);
                lastShot = 0;

                Ray ray = new Ray(localCamera.transform.position, localCamera.transform.forward);
                Debug.DrawRay(ray.origin, ray.direction * 50f, Color.white, 5f);
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(ray, out RaycastHit hit, 50f))
                {
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.yellow, 5f);
                    if (hit.collider.gameObject.CompareTag("Bubble"))
                    {
                        hit.collider.GetComponentInParent<Anchor>().GotHit();
                    }
                }
            }

            if (Input.GetButton("Jump"))
            {
                localCamera.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                isShoot = false;
                exitCallback.Invoke();
            }
        }
    }

    public void startShooting(Action callback)
    {
        localCamera.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        exitCallback = callback;
        isShoot = true;
        lastShot = reloadTime - 0.1f;
    }

    //public void Fire()
}

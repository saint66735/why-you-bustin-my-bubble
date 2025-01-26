using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MainCamera : MonoBehaviour
{
    public float shakeMagnitude = 0.1f;
    public float shakeSpeed = 10f;
    
    public Transform player;

    //private Vector3 initialRotation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //initialRotation = transform.localRotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {   /*
        Vector3 moveExtraRotation = player.forward * Perlin(player.localPosition.x, player.localPosition.z, 0) 
                                    + player.right * Perlin(player.localPosition.x, -player.localPosition.z, 1);
        Vector3 rotateExtraRotation = player.forward * Perlin(player.localRotation.eulerAngles.y * 5, 0, 2) 
                                    + player.right * Perlin(0, -player.localRotation.eulerAngles.y * 5, 3);
                                    
        //Debug.Log(player.localPosition);
        //Debug.Log((moveExtraRotation) * shakeMagnitude);
        transform.localRotation = Quaternion.Euler( (moveExtraRotation) * shakeMagnitude);
        */
    }

    float Perlin(float x, float y, int iteration)
    {
        return Mathf.PerlinNoise(x * shakeSpeed + iteration * 100, y * shakeSpeed - iteration * 50) -
               0.5f;
    }
}

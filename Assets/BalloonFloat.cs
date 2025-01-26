using System;
using UnityEngine;

public class BalloonFloat : MonoBehaviour
{
    [Range(0.25f, 4.0f)]
    public float size = 1.0f;
    public float sizeToFloatCoefficient = 10f;
    public float sizeToPhysicalSize = 4f;
    
    private float originalMass = 1.0f;

    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalMass = rb.mass;
    }
    
    void FixedUpdate()
    {
        rb.AddForce(Vector3.up * SizeToVolume(size) * sizeToFloatCoefficient, ForceMode.Force);
        transform.localScale = Vector3.one * sizeToPhysicalSize * size;
        //rb.mass = SizeToVolume(size) * originalMass;
    }

    public float SizeToVolume(float size)
    {
        return size; //Mathf.Pow(size / 4, 3) * Mathf.PI * (4 / 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}

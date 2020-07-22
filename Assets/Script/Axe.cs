using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{

    public float rotationSpeed;

    public bool activated = false,returning = false;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gameObject.GetComponent<TrailRenderer>().emitting = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            //transform.localEulerAngles -= Vector3.up * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
        if (returning)
        {
            transform.Rotate(Vector3.up, 1000f * Time.deltaTime, Space.Self);
            rb.isKinematic = false;
            gameObject.GetComponent<ParticleSystem>().Play();
        }
                    
    }

    private void OnCollisionEnter(Collision collision)
    {
        activated = false;
        
        rb.isKinematic = true;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantVelocity : MonoBehaviour
{
    public float Velocity = 0f;

    public Vector3 Direction = Vector3.forward;


    // Update is called once per frame
    void Update()
    {
        this.transform.position += this.Direction * Velocity * Time.deltaTime;  
    }
}

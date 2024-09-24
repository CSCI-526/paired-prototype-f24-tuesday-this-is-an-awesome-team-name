using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotationSpeed = 50.0f; 
    private bool isSpinning = false; 

    public void StartFan()
    {
        isSpinning = true;
    }

 
    public void StopFan()
    {
        isSpinning = false;
    }

    void Update()
    {
        if (isSpinning)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
    }
}


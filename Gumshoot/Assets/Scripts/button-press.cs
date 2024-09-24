using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public GameObject targetObject; 
    public string functionName; 

    private Vector3 originalPosition;
    

    private bool isPressed = false;
    void Start()
    {
        originalPosition = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isPressed && other.CompareTag("Player"))
        {
            isPressed = true;
            StartCoroutine(PressButton());
        }
    }
   

    System.Collections.IEnumerator PressButton()
    {
        targetObject.SendMessage(functionName);
        yield return null;
    }
    
}


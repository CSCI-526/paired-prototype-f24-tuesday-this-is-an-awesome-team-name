using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDoor : MonoBehaviour
{
    private bool isOpening = false;
    public GameObject door; 

    public void OpentheDoor()
    {
        isOpening = true;
        gameObject.SetActive(false);
    }
}

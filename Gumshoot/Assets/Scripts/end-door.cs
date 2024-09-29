using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndDoor : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            UIManager.Instance.winText.SetActive(true);
            Destroy(collision.gameObject);
        }
    }
}

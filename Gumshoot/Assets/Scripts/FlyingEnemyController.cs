using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (horizontalInput != 0 || verticalInput != 0)
        {
            rb.gravityScale = 0f;
        }
        else
        {
            rb.gravityScale = 1f;
        }

        Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}
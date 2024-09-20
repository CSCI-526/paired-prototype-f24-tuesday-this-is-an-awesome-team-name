using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject gumPrefab;

    [HideInInspector] public bool gumExtended = false;
    [HideInInspector] public bool latched = false;
    [HideInInspector] public Vector2 latchedNormal = Vector2.up;
    [HideInInspector] public GameObject PulledObject = null;

    public float jumpForce = 100f;
    public float throwForce = 100f;
    public float maxDist;
    public float extractSpeed;
    public float retractSpeed;

    // Update is called once per frame
    void Update()
    {
        // Launch grappling hook on left click
        if (Input.GetKeyDown(KeyCode.Mouse0) && !gumExtended)
        {
            Vector3 direction = GetMouseForward();

            // If extending towards the latched surface, then launch off the surface
            if (latched && Vector3.Angle(-direction, latchedNormal) < 45f)
            {
                GetComponent<Rigidbody2D>().gravityScale = 1.6f;
                latched = false;
                GetComponent<Rigidbody2D>().AddForce(-direction * jumpForce);
            }
            else if (PulledObject != null)
            {
                // Disconnect the pulled object from the player
                PulledObject.GetComponent<FixedJoint2D>().connectedBody = null;
                PulledObject.GetComponent<FixedJoint2D>().enabled = false;
                PulledObject.GetComponent<Rigidbody2D>().gravityScale = 1.6f;

                // Launch the object in the mouse direction
                PulledObject.transform.position = transform.position + direction;
                PulledObject.GetComponent<Rigidbody2D>().AddForce(direction * throwForce);

                PulledObject = null;
            }
            else
            {
                gumExtended = true;
                // Spawn gum
                GameObject gumInstance = Instantiate(gumPrefab, transform);
                gumInstance.GetComponent<HookMovement>().Initialize(this, direction);
            }
        }
    }

    Vector3 GetMouseForward()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return (mousePos - transform.position).normalized;
    }
}

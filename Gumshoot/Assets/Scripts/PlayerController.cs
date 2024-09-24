using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject gumPrefab;
    public GameObject contactPrefab;

    [HideInInspector] public bool gumExtended = false;
    [HideInInspector] public GameObject PulledObject = null;
    [HideInInspector] public GameObject SurfaceContactInstance = null;
    [HideInInspector] public GameObject PullContactInstance = null;

    public float jumpForce = 100f;
    public float throwForce = 100f;
    public float maxDist;
    public float extractSpeed;
    public float retractSpeed;

    [HideInInspector] public bool stuckToSurface = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (stuckToSurface && SurfaceContactInstance)
        {
            Vector3 direction = (SurfaceContactInstance.transform.position - transform.position).normalized;
            float dist = (SurfaceContactInstance.transform.position - transform.position).magnitude;
            if (dist > 0.7f)
            {
                //transform.localPosition += (retractSpeed * Time.deltaTime * direction);
                rb.velocity = direction * dist * 5;
            }
        }

        // Launch grappling hook on left click
        if (Input.GetKeyDown(KeyCode.Mouse0) && !gumExtended)
        {
            Vector3 direction = GetMouseForward();

            // If extending towards the latched surface, then launch off the surface
            if (SurfaceContactInstance && Vector3.Angle(-direction, transform.position - SurfaceContactInstance.transform.position) < 30f)
            {
                stuckToSurface = false;
                GetComponent<Rigidbody2D>().gravityScale = 1.6f;
                if (SurfaceContactInstance)
                {
                    Destroy(SurfaceContactInstance);
                    SurfaceContactInstance = null;
                }
                GetComponent<Rigidbody2D>().AddForce(-direction * jumpForce);
            }
            else if (PulledObject != null)
            {
                // Disconnect the pulled object from the player
                PulledObject.GetComponent<FixedJoint2D>().connectedBody = null;
                PulledObject.GetComponent<FixedJoint2D>().enabled = false;
                PulledObject.GetComponent<Rigidbody2D>().gravityScale = 1.6f;
                DamageObject damageObj = PulledObject.GetComponent<DamageObject>();
                if (damageObj)
                {
                    StartCoroutine(damageObj.Launch());
                }
                if (PullContactInstance)
                {
                    Destroy(PullContactInstance);
                    PullContactInstance = null;
                }

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
                gumInstance.GetComponent<GumMovement>().Initialize(this, direction);
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

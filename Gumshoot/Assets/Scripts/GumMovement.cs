using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class HookMovement : MonoBehaviour
{
    // Initialization
    private PlayerController owner;
    public GameObject StringGroupPrefab;
    public GameObject StringPrefab;
    public float stringSpriteLength = 0.08f;
    public float playerRadius = 0.325f;
    private Vector3 direction;

    private bool isExtending = true;
    private bool isRetracting = false;
    private bool isPullingPlayer = false;
    private float dist = 0.0f; // Distance from starting point
    private GameObject StringGroupInstance;
    private bool isContact = false;

    // String drawing
    private readonly Stack<GameObject> stringList = new(); // First index is closest to hook

    public void Initialize(PlayerController newOwner, Vector3 direction)
    {
        owner = newOwner;
        this.direction = direction;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);
        StringGroupInstance = Instantiate(StringGroupPrefab, transform);
    }

    // Update is called once per frame
    void Update()
    {
        // Extend
        if (isExtending)
        {
            // Extend the gum string
            if (dist < owner.maxDist)
            {
                dist += Time.deltaTime * owner.extractSpeed;
                transform.localPosition += (owner.extractSpeed * Time.deltaTime * direction);
            }
            // If the gum string has reached max length, start retracting
            else
            {
                dist -= playerRadius;
                isExtending = false;
                isRetracting = true;
            }
        }
        // Retract
        else if (isRetracting)
        {
            if (isContact)
            {
                direction = (owner.PullContactInstance.transform.position - owner.transform.position).normalized;
                dist = (owner.PullContactInstance.transform.position - owner.transform.position).magnitude;
            }
            else
            {
                direction = (transform.position - owner.transform.position).normalized;
                dist = (transform.position - owner.transform.position).magnitude;
            }

            // Moves the gum string toward the player
            if (dist > 0.5f)
            {
                dist -= Time.deltaTime * owner.retractSpeed;
                transform.localPosition -= (owner.retractSpeed * Time.deltaTime * direction);
            }
            // If the gum string has reached the player, stop all gum movement
            else
            {
                isRetracting = false;
            }
        }
        // Pull the player
        else if (isPullingPlayer)
        {
            if (isContact)
            {
                direction = (owner.SurfaceContactInstance.transform.position - owner.transform.position).normalized;
                dist = (owner.SurfaceContactInstance.transform.position - owner.transform.position).magnitude;
            }
            else
            {
                direction = (transform.position - owner.transform.position).normalized;
                dist = (transform.position - owner.transform.position).magnitude;
            }

            // Pull the player toward the gum
            if (dist > 0.5f)
            {
                dist -= Time.deltaTime * owner.retractSpeed;
                owner.transform.localPosition += (owner.retractSpeed * Time.deltaTime * direction);
            }
            // If the player has reached the gum string, stop all gum movement
            else
            {
                owner.stuckToSurface = true;
                isPullingPlayer = false;
            }
        }
        else
        {
            // Detach the pulled object from the gum and attach it to the player instead
            if (owner.PulledObject != null)
            {
                transform.DetachChildren();
                owner.PulledObject.GetComponent<FixedJoint2D>().enabled = true;
                owner.PulledObject.GetComponent<FixedJoint2D>().connectedBody = owner.GetComponent<Rigidbody2D>();
            }
            owner.gumExtended = false;
            Destroy(gameObject);
            Destroy(StringGroupInstance);
            return;
        }

        DrawString();
    }

    // Handles drawing the gum string
    private void DrawString()
    {
        if (isContact)
        {
            if (isRetracting)
            {
                StringGroupInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);
                StringGroupInstance.transform.position = owner.PullContactInstance.transform.position;
            }
            else if (isPullingPlayer)
            {
                StringGroupInstance.transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);
                StringGroupInstance.transform.position = owner.SurfaceContactInstance.transform.position;
            }
        }
        // If the length of the string has exceeded the combined length of the string sprites, spawn another string sprite
        if (dist > stringList.Count * stringSpriteLength * 7.5f)
        {
            // Add chains
            GameObject StringInstance = Instantiate(StringPrefab, StringGroupInstance.transform);
            StringInstance.transform.localPosition = new Vector3(0f, (stringSpriteLength / 2f + stringSpriteLength * stringList.Count), 0f);
            stringList.Push(StringInstance);
        }
        // Destroy string sprites as it retracts
        else if (dist + playerRadius <= (stringList.Count - 1) * stringSpriteLength * 7.5f)
        {
            // Remove chains
            Destroy(stringList.Pop());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider.gameObject.name);
        // Only accept gum collisions when extending
        if (!isPullingPlayer && !isRetracting)
        {
            // Handles pulling the player toward a surface
            if (collision.collider.gameObject.CompareTag("Surface"))
            {
                GetComponent<SpriteRenderer>().enabled = false;
                owner.stuckToSurface = false;
                if (owner.SurfaceContactInstance)
                {
                    Destroy(owner.SurfaceContactInstance);
                }
                owner.SurfaceContactInstance = Instantiate(owner.contactPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, collision.GetContact(0).normal));
                owner.SurfaceContactInstance.transform.parent = collision.transform;
                isContact = true;

                // Pull the player toward the object
                isExtending = false;
                isRetracting = false;
                isPullingPlayer = true;

                // Rotate the hook to match the surface normal
                transform.DetachChildren();
                transform.rotation = Quaternion.LookRotation(Vector3.forward, collision.GetContact(0).normal);

                // Stop player movement
                owner.transform.DetachChildren();
                owner.GetComponent<Rigidbody2D>().gravityScale = 0f;
                owner.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

                // Recalculate distance
                dist = Vector3.Distance(owner.transform.position, transform.position);
            }
            // Handles pulling an object toward the player
            else if (collision.gameObject.CompareTag("Pullable"))
            {
                GetComponent<SpriteRenderer>().enabled = false;
                if (owner.PullContactInstance)
                {
                    Destroy(owner.PullContactInstance);
                }
                owner.PullContactInstance = Instantiate(owner.contactPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, collision.GetContact(0).normal));
                owner.PullContactInstance.transform.parent = collision.transform;
                isContact = true;

                // Pull the object toward the player
                isExtending = false;
                isRetracting = true;
                isPullingPlayer = false;

                // Attach the object onto the gum
                collision.gameObject.transform.parent = transform;
                owner.PulledObject = collision.gameObject;
                owner.PulledObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
            }
        }
    }
}

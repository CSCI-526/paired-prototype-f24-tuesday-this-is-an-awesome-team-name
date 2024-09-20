using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (isExtending)
        {
            // Extend
            if (dist < owner.maxDist)
            {
                dist += Time.deltaTime * owner.extractSpeed;
                transform.localPosition += (owner.extractSpeed * Time.deltaTime * direction);
            }
            else
            {
                dist -= playerRadius;
                isExtending = false;
                isRetracting = true;
            }
        }
        else if (isRetracting)
        {
            // Retract
            if (dist > 0f)
            {
                dist -= Time.deltaTime * owner.retractSpeed;
                transform.localPosition -= (owner.retractSpeed * Time.deltaTime * direction);
            }
            else
            {
                isRetracting = false;
            }
        }
        else if (isPullingPlayer)
        {
            // Pull the player
            if (dist > 0f)
            {
                dist -= Time.deltaTime * owner.retractSpeed;
                owner.transform.localPosition += (owner.retractSpeed * Time.deltaTime * direction);
            }
            else
            {
                owner.latched = true;
                isPullingPlayer = false;
            }
        }
        else
        {
            owner.gumExtended = false;
            Destroy(gameObject);
            Destroy(StringGroupInstance);
            return;
        }

        DrawChain();
    }

    private void DrawChain()
    {
        if (dist > stringList.Count * stringSpriteLength * 7.5f)
        {
            // Add chains
            GameObject StringInstance = Instantiate(StringPrefab, StringGroupInstance.transform);
            StringInstance.transform.localPosition = new Vector3(0f, (stringSpriteLength / 2f + stringSpriteLength * stringList.Count), 0f);
            stringList.Push(StringInstance);
        }
        else if (dist + playerRadius <= (stringList.Count - 1) * stringSpriteLength * 7.5f)
        {
            // Remove chains
            Destroy(stringList.Pop());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isPullingPlayer)
        {
            // Debug
            Debug.Log(collision.gameObject.name);

            if (collision.collider.gameObject.CompareTag("Surface") && !isRetracting)
            {
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
                owner.latchedNormal = collision.GetContact(0).normal;

                // Recalculate distance
                dist = Vector3.Distance(owner.transform.position, transform.position);
            }
            else if (collision.gameObject.CompareTag("Pullable"))
            {
                // Pull the object toward the player
                isExtending = false;
                isRetracting = true;
                isPullingPlayer = false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 1f;
    public float chaseSpeed = 2f;
    public float patrolRange = 5f;
    public float visionRange = 4f;

    [Header("Layer Settings")]
    public LayerMask obstacleLayer;
    public LayerMask playerLayer;

    [Header("References")]
    public Transform player;
    public GameObject controllerPrefab;

    private Vector2 patrolStartPos;
    private CircleCollider2D visionCollider;
    private bool isChasing = false;
    private bool isFacingRight = true;

    private Vector2 currentVelocity = Vector2.zero; // Used for SmoothDamp velocity

    private int patrolDirection = 1;    // 1 for right, -1 for left
    private float patrolLeftLimit;
    private float patrolRightLimit;

    void Start()
    {
        patrolStartPos = transform.position;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        patrolLeftLimit = patrolStartPos.x - patrolRange / 2f;
        patrolRightLimit = patrolStartPos.x + patrolRange / 2f;

        // set vision range collider
        visionCollider = gameObject.AddComponent<CircleCollider2D>();
        visionCollider.radius = visionRange;
        visionCollider.isTrigger = true;
    }

    void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Vector2 movement = Vector2.right * patrolDirection * patrolSpeed * Time.deltaTime;
        transform.Translate(movement);
        
        Vector2 rayDirection = patrolDirection == 1 ? Vector2.right : Vector2.left;
        // Visualize Raycast
        //Debug.DrawRay(transform.position, rayDirection * 1f, Color.blue);
        // Detect if there's an obstacle in front
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, 1f, obstacleLayer);

        // Hit an obstacle, reverse direction
        if (hit.collider != null)
        {
            FlipPatrolDirection();
        }

        // Reached the patrol range boundary, reverse direction
        if (patrolDirection == 1 && transform.position.x >= patrolRightLimit)
        {
            FlipPatrolDirection();
        }
        else if (patrolDirection == -1 && transform.position.x <= patrolLeftLimit)
        {
            FlipPatrolDirection();
        }
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        // Get the direction to chase
        Vector2 direction = (player.position - transform.position).normalized;
        Vector2 targetPosition = player.position;

        // Smoothly chase using SmoothDamp, limiting to maximum chase speed
        transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 0.1f, chaseSpeed);

        // Change the enemy's facing direction
        if (direction.x > 0 && !isFacingRight)
            Flip();
        else if (direction.x < 0 && isFacingRight)
            Flip();
    }

    void FlipPatrolDirection()
    {
        patrolDirection *= -1;
        Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    bool IsPlayerVisible()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Raycast to detect if there's an obstacle blocking the view
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        if (hit.collider == null && Physics2D.OverlapCircle(transform.position, visionRange, playerLayer) != null)
        {
            Debug.Log("Player is visible!");
            return true;
        }
        return false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (IsPlayerVisible())
            {
                isChasing = true;
                Debug.Log("Enemy started chasing the player.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            patrolStartPos = transform.position;
            patrolLeftLimit = patrolStartPos.x - patrolRange / 2f;
            patrolRightLimit = patrolStartPos.x + patrolRange / 2f;
            Debug.Log("Enemy stopped chasing the player.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}

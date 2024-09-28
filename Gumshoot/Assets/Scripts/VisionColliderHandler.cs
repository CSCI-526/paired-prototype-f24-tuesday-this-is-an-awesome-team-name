using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionColliderHandler : MonoBehaviour
{
    private CircleCollider2D visionCollider;
    public FlyingEnemyMovement flyingEnemy;

    private LineRenderer lineRenderer;

    [Header("Layer Settings")]
    public LayerMask obstacleLayer;
    public LayerMask playerLayer;

    [Header("Line Renderer Settings")]
    public int segments = 50;
    public Color lineColor = Color.red;

    void Start()
    {
        visionCollider = GetComponent<CircleCollider2D>();
        flyingEnemy = GetComponentInParent<FlyingEnemyMovement>();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = false;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        CreateVisionCircle();
    }

    void CreateVisionCircle()
    {
        float angle = 0f;
        float radius = visionCollider.radius;

        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));

            angle += (360f / segments);
        }
    }

    public bool IsPlayerVisible()
    {
        if (flyingEnemy == null || FlyingEnemyMovement.player == null)
            return false;

        Vector2 directionToPlayer = (FlyingEnemyMovement.player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, FlyingEnemyMovement.player.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        if (hit.collider == null && Physics2D.OverlapCircle(transform.position, visionCollider.radius, playerLayer) != null)
        {
            Debug.Log("Player is visible!");
            return true;
        }

        return false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && IsPlayerVisible())
        {
            flyingEnemy.StartChasing();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            flyingEnemy.StopChasing();
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (visionCollider != null)
    //    {
    //        Gizmos.color = Color.yellow;
    //        float scaleFactor = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y);
    //        Gizmos.DrawWireSphere(transform.position, visionCollider.radius * scaleFactor);
    //    }

    //}
}

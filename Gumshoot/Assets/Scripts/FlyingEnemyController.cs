using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;

    public GameObject playerPrefab;
    private bool isCooldown = false;

    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().Follow = transform;
        FlyingEnemyMovement.player = transform;
        StartCoroutine(CooldownCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCooldown)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(horizontalInput, verticalInput, 0f);
            //rb.MovePosition(transform.position + moveSpeed * Time.deltaTime * moveDirection);
            rb.velocity = moveDirection * moveSpeed;
            //transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(15f);
        isCooldown = true;
        GameObject newPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>().Follow = newPlayer.transform;
        FlyingEnemyMovement.player = newPlayer.transform;
        Destroy(gameObject);
    }
}

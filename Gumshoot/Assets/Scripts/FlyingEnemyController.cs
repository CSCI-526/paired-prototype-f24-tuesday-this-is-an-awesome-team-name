using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;

    public GameObject playerPrefab;
    private bool isCooldown = false;

    // Start is called before the first frame update
    void Start()
    {
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
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(5f);
        isCooldown = true;
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

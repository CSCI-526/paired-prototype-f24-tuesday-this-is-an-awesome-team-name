using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    private bool canExplode = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canExplode && !collision.collider.CompareTag("Gum"))
        {
            Health hpObj = collision.collider.GetComponent<Health>();
            if (hpObj)
            {
                hpObj.Damage(1);
            }

            if (!collision.collider.CompareTag("Gum"))
            {
                Destroy(gameObject);
            }
        }
        
    }

    public IEnumerator Launch()
    {
        yield return new WaitForSeconds(0.5f);
        canExplode = true;
    }
}

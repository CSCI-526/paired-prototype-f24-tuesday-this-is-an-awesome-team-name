using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObject : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private bool canExplode = true;
    public bool canHurtPlayer = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (canHurtPlayer)
            {
                Health hpObj = collision.collider.GetComponent<Health>();
                if (hpObj)
                {
                    hpObj.Damage(damage);
                }

                if (canExplode)
                {
                    Destroy(gameObject);
                }
            }
        }
        else if (collision.collider.CompareTag("Enemy"))
        {
            Health hpObj = collision.collider.GetComponent<Health>();
            if (hpObj)
            {
                hpObj.Damage(damage);
            }

            if (canExplode)
            {
                Destroy(gameObject);
            }
        }

    }

    public IEnumerator Launch()
    {
        canHurtPlayer = false;
        yield return new WaitForSeconds(0.4f);
        canHurtPlayer = true;
    }
}

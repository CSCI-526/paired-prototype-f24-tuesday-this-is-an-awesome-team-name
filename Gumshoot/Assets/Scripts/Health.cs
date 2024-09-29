using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public bool destroyOnDeath = false;

    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0 && destroyOnDeath)
        {
            if (CompareTag("Player"))
            {
                LevelManager.Instance.StartCoroutine(LevelManager.Instance.Die());
            }
            Destroy(gameObject);
        }
    }

    public void Heal(int heal)
    {
        health += heal;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPObject : MonoBehaviour
{
    private int health;

    public void Damage(int damage)
    {
        health -= damage;
    }

    public void Heal(int heal)
    {
        health += heal;
    }
}

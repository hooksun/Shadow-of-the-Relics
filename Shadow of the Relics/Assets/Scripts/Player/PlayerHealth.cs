using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : PlayerBehaviour
{
    public float maxHealth;

    float health;

    void Start()
    {
        health = maxHealth;
    }

    public override void TakeDamage(float damage, Vector2 origin)
    {
        health -= damage;
        if(health <= 0f)
        {
            //ded
        }
    }
}

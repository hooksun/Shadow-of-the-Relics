using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : PlayerBehaviour
{
    public float maxHealth;
    public AudioPlayer damagedAudio;

    float health;

    void Start()
    {
        health = maxHealth;
    }

    public override void TakeDamage(float damage, Vector2 origin)
    {
        health -= damage;
        damagedAudio.Play();
        if(health <= 0f)
        {
            //ded
        }
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public float GetHealth() => health;
}

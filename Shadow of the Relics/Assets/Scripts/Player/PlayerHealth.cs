using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : PlayerBehaviour
{
    public float maxHealth, healCooldown, healSpeed;
    public AudioPlayer damagedAudio;

    float health, cooldown;

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

    void Update()
    {
        if(player.detected)
        {
            cooldown = 0f;
            return;
        }
        if(cooldown < healCooldown)
        {
            cooldown += Time.deltaTime;
            return;
        }
        health = Mathf.MoveTowards(health, maxHealth, healSpeed * Time.deltaTime);
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public float GetHealth() => health;
}

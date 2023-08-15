using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : PlayerBehaviour
{
    public float maxHealth, healCooldown, healSpeed;
    public AudioPlayer damagedAudio;
    public Slider HealthBar;

    float health, cooldown;

    void Awake()
    {
        health = maxHealth;
    }

    public override void TakeDamage(float damage, Vector2 origin)
    {
        health -= damage;
        HealthBar.value = health;
        damagedAudio.Play();
        if(health <= 0f)
        {
            //ded
            player.dead = true;
        }
    }

    public override void Respawn()
    {
        health = maxHealth;
        HealthBar.value = health;
        cooldown = 0f;
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
        HealthBar.value = health;
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public float GetHealth() => health;
}

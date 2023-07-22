using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public float speed, rotateSpeed, radius, damage, despawnTime, fadeTime;
    public LayerMask ObstacleMask, HitMask;

    Vector2 direction;
    float despawn;
    bool stuck;

    public void Init(Vector2 position, Vector2 direction)
    {
        transform.position = (Vector3)position;
        this.direction = direction;
        stuck = false;
        gameObject.SetActive(true);
    }

    void Update()
    {
        if(stuck)
        {
            if(despawn <= 0f)
            {
                gameObject.SetActive(false);
                SetAlpha(1f);
                return;
            }
            if(despawn < fadeTime)
            {
                SetAlpha(Mathf.InverseLerp(0f, fadeTime, despawn));
            }

            despawn -= Time.deltaTime;

            return;
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position, radius, HitMask);
        if(hit)
        {
            Player player = hit.GetComponentInParent<Player>();
            if(!player.CantGetDamaged)
            {
                player.TakeDamage(damage, player.position - direction);
                gameObject.SetActive(false);
                return;
            }
        }

        hit = Physics2D.OverlapCircle(transform.position, radius, ObstacleMask);
        if(hit)
        {
            stuck = true;
            despawn = despawnTime;
            return;
        }

        transform.position += (Vector3)direction * speed * Time.deltaTime;
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    void SetAlpha(float alpha)
    {
        Color col = Renderer.color;
        col.a = alpha;
        Renderer.color = col;
    }
}

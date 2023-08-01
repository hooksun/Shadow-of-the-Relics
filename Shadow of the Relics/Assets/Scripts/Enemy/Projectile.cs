using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public float speed, rotateSpeed, radius, damage, despawnTime, fadeTime;
    public LayerMask ObstacleMask, HitMask;
    public AudioPlayer StuckAudio;

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

        Vector2 posDelta = direction * speed * Time.deltaTime;
        float dist = posDelta.magnitude;
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, posDelta, dist, HitMask);
        if(hit)
        {
            Player player = hit.transform.GetComponentInParent<Player>();
            if(!player.CantGetDamaged)
            {
                player.TakeDamage(damage, player.position - direction, true);
                gameObject.SetActive(false);
                return;
            }
        }

        hit = Physics2D.CircleCast(transform.position, radius, posDelta, dist, ObstacleMask);
        if(hit)
        {
            stuck = true;
            despawn = despawnTime;
            StuckAudio.Play();
            return;
        }

        transform.position += (Vector3)posDelta;
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }

    void SetAlpha(float alpha)
    {
        Color col = Renderer.color;
        col.a = alpha;
        Renderer.color = col;
    }
}

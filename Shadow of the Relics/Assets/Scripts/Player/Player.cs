using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerAnimator animator;
    public PlayerHealth health;
    public SpriteRenderer sprite;
    public Animator Anim;

    public float detectCooldown, stopDetectTime, latePositionTime, damageCooldown;

    public Vector2 position{get=>transform.position;}
    public float activeDir{get=>(sprite.flipX?-1f:1f); set=>sprite.flipX = (value < 0f);}
    [HideInInspector] public Vector2 latePosition, lastSeenPosition;
    public bool CantGetDamaged{get=>damaged > 0f;}

    void Awake()
    {
        movement.player = this;
        animator.player = this;
        health.player = this;
    }

    float detectTime, damaged;
    void FixedUpdate()
    {
        StartCoroutine(SetLatePosition(position));
        if(detectTime > 0f)
        {
            detectTime -= Time.fixedDeltaTime;
        }
        if(damaged > 0f)
            damaged -= Time.fixedDeltaTime;
    }

    public void TakeDamage(float damage, Vector2 origin)
    {
        if(CantGetDamaged)
            return;
        
        damaged = damageCooldown;
        movement.TakeDamage(damage, origin);
        animator.TakeDamage(damage, origin);
        health.TakeDamage(damage, origin);
    }

    IEnumerator SetLatePosition(Vector2 pos)
    {
        yield return new WaitForSeconds(latePositionTime);
        latePosition = pos;
    }

    IEnumerator SetSeenPosition(Vector2 pos)
    {
        yield return new WaitForSeconds(latePositionTime);
        lastSeenPosition = pos;
    }

    public bool detected{get=>detectTime > 0f;}
    public void Seen()
    {
        detectTime = stopDetectTime;
        StartCoroutine(SetSeenPosition(position));
    }

    public void Detect()
    {
        lastSeenPosition = position;
        Seen();
    }
}

public abstract class PlayerBehaviour : MonoBehaviour
{
    [HideInInspector] public Player player;

    public virtual void TakeDamage(float damage, Vector2 origin){}
}
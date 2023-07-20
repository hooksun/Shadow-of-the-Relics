using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolVision : EnemyBehaviour
{
    public SpriteRenderer sprite;
    
    public Vector2 bounds;
    public LayerMask PlayerMask;

    public float detectDelay;

    public Color DetectedColor;

    void Start()
    {
        normalColor = sprite.color;
    }

    Color normalColor;
    bool detected;
    void FixedUpdate()
    {
        if(detected)
            return;
        
        Collider2D hit = Physics2D.OverlapBox(transform.position, bounds, 0f, PlayerMask);
        if(enemy.SeePlayer(hit, Time.fixedDeltaTime, detectDelay))
            DetectPlayer();
    }

    public void DetectPlayer()
    {
        if(detected)
            return;

        sprite.color = DetectedColor;
        detected = true;
        enemy.patrol.enabled = false;
    }

    public void StartPatrol()
    {
        sprite.color = normalColor;
        detected = false;
    }
}

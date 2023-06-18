using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : EnemyBehaviour
{
    public SpriteRenderer sprite;
    
    public Vector2 bounds;
    public LayerMask PlayerMask;

    public Color DetectedColor;

    Color normalColor;
    bool detected;
    void FixedUpdate()
    {
        if(detected)
            return;
        
        if(Physics2D.OverlapBox(transform.position, bounds, 0f, PlayerMask))
        {
            normalColor = sprite.color;
            sprite.color = DetectedColor;
            detected = true;
            enemy.DetectPlayer();
            StartCoroutine(StopDetect());
        }
    }

    IEnumerator StopDetect()
    {
        yield return new WaitForSeconds(5f);
        detected = false;
        sprite.color = normalColor;
    }
}

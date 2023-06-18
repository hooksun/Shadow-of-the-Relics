using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : EnemyBehaviour
{
    public float speed, length, height, bumpLength, turnDelay;
    public Vector2 raycastPoint;
    public LayerMask ObstacleMask;

    float direction{get=>transform.localScale.x; set=>transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);}

    float turnCooldown;
    void Update()
    {
        if(turnCooldown > 0f)
        {
            turnCooldown -= Time.deltaTime;
            if(turnCooldown <= 0f)
            {
                direction *= -1f;
            }
            return;
        }

        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if(Turn())
        {
            turnCooldown = turnDelay;
        }
    }

    bool Turn()
    {
        if(Physics2D.Raycast(transform.position, new Vector2(direction, 0f), bumpLength, ObstacleMask))
            return true;
        Vector2 edge = (Vector2)transform.position + new Vector2(direction, 0f) * length;
        if(!Physics2D.Raycast(edge, Vector2.down, height, ObstacleMask))    
            return true;
        return false;
    }
}

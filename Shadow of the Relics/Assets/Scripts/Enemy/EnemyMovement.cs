using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed, length, height;
    public Vector2 raycastPoint;
    public LayerMask ObstacleMask;

    float direction{get=>transform.localScale.x; set=>transform.localScale = new Vector3(value, transform.localScale.y, transform.localScale.z);}

    void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;

        if(Turn())
            direction *= -1f;
    }

    bool Turn()
    {
        Vector2 edge = (Vector2)transform.position + new Vector2(direction, 0f) * length;
        if(Physics2D.Linecast(transform.position, edge, ObstacleMask))
            return true;
        if(!Physics2D.Raycast(edge, Vector2.down, height, ObstacleMask))    
            return true;
        return false;
    }
}

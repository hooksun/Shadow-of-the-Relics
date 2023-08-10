using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : EnemyBehaviour
{
    public LineRenderer LOSRenderer;
    public float nonAggroDetectRadius;
    public LayerMask ObstacleMask, PlayerMask;
    public bool enableLOSLine;

    float detectLevel;
    void Update()
    {
        if(enemy.aggro)
        {
            enemy.seePlayer = !Physics2D.Linecast(enemy.position + enemy.eyePosition, enemy.Target.position, ObstacleMask);
            if(enemy.seePlayer)
            {
                LOSRenderer.SetPosition(0, LOSRenderer.transform.position);
                LOSRenderer.SetPosition(1, enemy.Target.position);
                enemy.Target.Seen();
            }
            else if(!enemy.Target.detected)
                enemy.StopChase();

            LOSRenderer.enabled = enableLOSLine && enemy.seePlayer;
            return;
        }

        Collider2D hit = Physics2D.OverlapCircle(enemy.position + enemy.eyePosition, nonAggroDetectRadius, PlayerMask);
        if(hit && !Physics2D.Linecast(enemy.position + enemy.eyePosition, hit.transform.position, ObstacleMask))
        {
            enemy.SeePlayer(hit, Time.deltaTime);
        }
        
    }
}

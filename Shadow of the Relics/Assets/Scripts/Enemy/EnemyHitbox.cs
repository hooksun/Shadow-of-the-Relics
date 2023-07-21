using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : EnemyBehaviour
{
    public float damage;
    public Vector2 hitboxOffset, hitboxSize;
    public LayerMask damageMask;

    void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapBox((Vector2)transform.position + hitboxOffset, hitboxSize, 0f, damageMask);
        if(hit)
        {
            Player player = hit.GetComponentInParent<Player>();
            player.TakeDamage(damage, (Vector2)transform.position + hitboxOffset);

            if(!enemy.aggro)
                Enemy.AllDetectPlayer(player);
        }
    }
}

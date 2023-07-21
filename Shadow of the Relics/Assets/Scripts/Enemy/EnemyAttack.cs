using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyBehaviour
{
    public float Damage, Cooldown, GlobalCooldown, AttackDelay;
    public LayerMask ObstacleMask, PlayerMask;
    public LineRenderer IndicatorLine, DamageLine;

    static EnemyAttack current;
    static float globalcooldown;
    float cooldown;

    void Update()
    {
        if(current == this)
            CurrentUpdate();

        if(cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            return;
        }
    }

    void CurrentUpdate()
    {
        if(globalcooldown > 0f)
        {
            globalcooldown -= Time.deltaTime;
            if(globalcooldown <= 0f)
            {
                List<Enemy> EnemyAttackPool = new List<Enemy>();
                foreach(Enemy nme in Enemy.ActiveEnemies)
                {
                    if(nme.seePlayer)
                        EnemyAttackPool.Add(nme);
                }
                if(EnemyAttackPool.Count == 0)
                {
                    globalcooldown = GlobalCooldown;
                    return;
                }
                int rand = Random.Range(0, EnemyAttackPool.Count);
                current = EnemyAttackPool[rand].attack;
            }
            return;
        }

        if(!enemy.seePlayer)
        {
            globalcooldown = GlobalCooldown;
            return;
        }


    }
}

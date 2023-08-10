using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyBehaviour
{
    public float Cooldown, GlobalCooldown, StartCooldown, MinAttackDelay, ProjectileRadius;
    public Vector2 ProjectileSpawnOffset, NonAttackBounds, NonAttackBoundsOffset;
    public LayerMask ObstacleMask, PlayerMask;
    public AudioPlayer ProjectileAudio;

    static bool globalcooldown;
    float cooldown, seePlayer;

    void OnEnable()
    {
        if(globalcooldown)
            print("global cooldown true");
        
        cooldown = StartCooldown;
        seePlayer = 0f;
    }

    void Update()
    {
        if(cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        if(!enemy.aggro)
            return;
        
        TargetVision();

        if(globalcooldown || seePlayer < MinAttackDelay)
            return;
        
        Vector2 boundStart = (Vector2)transform.position + NonAttackBoundsOffset - NonAttackBounds * 0.5f;
        Vector2 boundEnd = (Vector2)transform.position + NonAttackBoundsOffset + NonAttackBounds * 0.5f;
        if(Vector2.Max(enemy.Target.position, boundEnd) == boundEnd && Vector2.Min(enemy.Target.position, boundStart) == boundStart)
            return;
        
        SpawnProjectile();
        StartCoroutine(SetGlobalCooldown());
        cooldown = Cooldown;
    }

    void SpawnProjectile()
    {
        Projectile projectile = ProjectilePool.GetItem();

        Vector2 direction = enemy.Target.position - (enemy.position + ProjectileSpawnOffset);

        projectile.Init(enemy.position + ProjectileSpawnOffset, direction.normalized);
        ProjectileAudio.Play();
    }

    IEnumerator SetGlobalCooldown()
    {
        globalcooldown = true;
        yield return new WaitForSeconds(GlobalCooldown);
        globalcooldown = false;
    }

    void TargetVision()
    {
        Vector2 dir = enemy.Target.position - (enemy.position + ProjectileSpawnOffset);
        bool see = !Physics2D.CircleCast(enemy.position + ProjectileSpawnOffset, ProjectileRadius, dir, dir.magnitude, ObstacleMask);
        seePlayer = (see?seePlayer + Time.deltaTime:0f);
    }
}

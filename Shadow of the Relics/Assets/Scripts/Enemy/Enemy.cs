using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyMovement movement;
    public EnemyVision vision;
    public EnemyAttack attack;
    public EnemyPatrol patrol;
    public EnemyHitbox hitbox;
    public EnemyAnimator animator;
    public EnemyPatrolVision patrolVision;

    public Vector2 eyePosition;
    public float halfWidth;

    public Vector2 position{get=>transform.position;}

    [HideInInspector] public bool aggro, seePlayer;
    [HideInInspector] public Path patrolPath;
    [HideInInspector] public Player Target;

    public static List<Enemy> ActiveEnemies = new List<Enemy>();

    void Awake()
    {
        movement.enemy = this;
        vision.enemy = this;
        attack.enemy = this;
        patrol.enemy = this;
        hitbox.enemy = this;
        animator.enemy = this;
        patrolVision.enemy = this;

        movement.enabled = false;
        vision.enabled = false;
        attack.enabled = false;
    }

    void Start()
    {
        if(patrolPath == null)
            patrolPath = PathManager.ClosestPathTo(transform.position);
    }

    void OnEnable()
    {
        ActiveEnemies.Add(this);
    }

    void OnDisable()
    {
        ActiveEnemies.Remove(this);
    }

    float detectLevel;
    public bool SeePlayer(Collider2D hit, float delta, float detectDelay = 0f)
    {
        if(!hit)
        {
            detectLevel = 0f;
            return false;
        }
    
        Player player = hit.GetComponent<Player>();
        detectLevel += delta;
        if(detectLevel < player.detectCooldown)
            return false;
        StartCoroutine(StartDetect(player, detectDelay));
        return true;
    }

    IEnumerator StartDetect(Player player, float detectDelay)
    {
        if(detectDelay > 0f)
            yield return new WaitForSeconds(detectDelay);
        if(aggro)
            yield break;
        AllDetectPlayer(player);
    }

    public static void AllDetectPlayer(Player player)
    {
        player.Detect();
        foreach(Enemy enemy in ActiveEnemies)
        {
            enemy.DetectPlayer(player);
        }
    }

    void DetectPlayer(Player player)
    {
        Target = player;
        if(!aggro)
        {
            patrolVision.gameObject.SetActive(false);
            patrol.enabled = false;
            movement.enabled = true;
            vision.enabled = true;
            attack.enabled = true;
            aggro = true;
            movement.StartChase();
        }
    }

    public void StopChase()
    {
        aggro = false;
        movement.StopChase();
    }

    public void StartPatrol()
    {
        patrolVision.gameObject.SetActive(true);
        patrol.enabled = true;
        patrol.StartPatrol();
        patrolVision.StartPatrol();
        movement.enabled = false;
        vision.enabled = false;
        attack.enabled = false;
    }
}

public abstract class EnemyBehaviour : MonoBehaviour
{
    [HideInInspector] public Enemy enemy;
}

public struct EnemySave
{
    public Vector2 position;
    public bool aggro;
}
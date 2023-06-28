using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyMovement movement;
    public EnemyVision vision;
    public EnemyPatrol patrol;
    public EnemyPatrolVision patrolVision;
    public EnemyHitbox hitbox;

    public Vector2 eyePosition;
    public float halfWidth;

    public Vector2 position{get=>transform.position;}

    [HideInInspector] public bool aggro;
    [HideInInspector] public Path patrolPath;
    [HideInInspector] public Player Target;

    public static List<Enemy> ActiveEnemies = new List<Enemy>();

    void Awake()
    {
        movement.enemy = this;
        vision.enemy = this;
        patrol.enemy = this;
        patrolVision.enemy = this;
        hitbox.enemy = this;

        movement.enabled = false;
        vision.enabled = false;
    }

    void Start()
    {
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

    public void AllDetectPlayer(Player player)
    {
        player.Seen();
        foreach(Enemy enemy in ActiveEnemies)
        {
            enemy.DetectPlayer(player);
        }
    }

    void DetectPlayer(Player player)
    {
        Target = player;
        aggro = true;
        patrolVision.gameObject.SetActive(false);
        patrol.enabled = false;
        movement.enabled = true;
        vision.enabled = true;
        movement.StartChase();
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
        patrolVision.StartPatrol();
        movement.enabled = false;
        vision.enabled = false;
    }
}

public abstract class EnemyBehaviour : MonoBehaviour
{
    [HideInInspector] public Enemy enemy;
}
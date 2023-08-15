using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerAnimator animator;
    public PlayerHealth health;
    public SpriteRenderer sprite;
    public Animator Anim;
    public GameObject Corpse;

    public Vector2 respawnPosition;
    public float detectCooldown, stopDetectTime, latePositionTime, damageCooldown;
    public AudioPlayer RangeHitAudio;

    public Vector2 position{get=>transform.position;}
    public float activeDir{get=>(sprite.flipX?-1f:1f); set=>sprite.flipX = (value < 0f);}
    [HideInInspector] public Vector2 latePosition, lastSeenPosition;
    public bool CantGetDamaged{get=>dead || damaged > 0f;}

    public static Player activePlayer;

    [HideInInspector] public bool dead;

    void Awake()
    {
        activePlayer = this;

        movement.player = this;
        animator.player = this;
        health.player = this;

        SaveManager.OnLoad += OnLoad;
        SaveManager.OnSave += OnSave;
    }

    float detectTime, damaged;
    void FixedUpdate()
    {
        if(Time.timeScale == 0f)
            return;

        StartCoroutine(SetLatePosition(position));
        if(detectTime > 0f)
        {
            detectTime -= Time.fixedDeltaTime;
            if(detectTime <= 0f)
                GameplayMusic.SwitchMusic(false);
        }
        if(damaged > 0f)
            damaged -= Time.fixedDeltaTime;
    }

    public void TakeDamage(float damage, Vector2 origin, bool range = false)
    {
        if(CantGetDamaged)
            return;
        if(range)
            RangeHitAudio.Play();
        
        damaged = damageCooldown;
        health.TakeDamage(damage, origin);
        movement.TakeDamage(damage, origin);
        animator.TakeDamage(damage, origin);
    }

    public void Respawn()
    {
        transform.position = respawnPosition;
        detectTime = 0f;
        damaged = 0.1f;
        dead = false;
        health.Respawn();
        movement.Respawn();
        animator.Respawn();

        Corpse.SetActive(false);
        sprite.enabled = true;
        GameplayMusic.SwitchMusic(false);
    }

    public void EnterGate()
    {
        animator.EnterGate();
    }

    void OnLoad()
    {
        if(SaveManager.saver.playerSave.dead)
        {
            Respawn();
            return;
        }
        
        transform.position = SaveManager.saver.playerSave.position;
        movement.rb.velocity = SaveManager.saver.playerSave.velocity;
        health.SetHealth(SaveManager.saver.playerSave.health);
        activeDir = SaveManager.saver.playerSave.direction;
        detectTime = SaveManager.saver.playerSave.detectTime;

        if(detected)
            GameplayMusic.SwitchMusic(true);
    }

    void OnSave()
    {
        SaveManager.saver.playerSave.position = transform.position;
        SaveManager.saver.playerSave.velocity = movement.rb.velocity;
        SaveManager.saver.playerSave.health = health.GetHealth();
        SaveManager.saver.playerSave.direction = activeDir;
        SaveManager.saver.playerSave.detectTime = detectTime;
        SaveManager.saver.playerSave.dead = dead;
    }

    IEnumerator SetLatePosition(Vector2 pos)
    {
        yield return new WaitForSeconds(latePositionTime);
        latePosition = pos;
    }

    IEnumerator SetSeenPosition(Vector2 pos)
    {
        yield return new WaitForSeconds(latePositionTime);
        lastSeenPosition = pos;
    }

    public bool detected{get=>detectTime > 0f;}
    public void Seen()
    {
        detectTime = stopDetectTime;
        StartCoroutine(SetSeenPosition(position));
    }

    public void Detect()
    {
        lastSeenPosition = position;
        Seen();
    }
}

public abstract class PlayerBehaviour : MonoBehaviour
{
    [HideInInspector] public Player player;

    public virtual void TakeDamage(float damage, Vector2 origin){}
    public virtual void Respawn(){}
}

[System.Serializable]
public struct PlayerSave
{
    public Vector2 position, velocity;
    public float health, direction, detectTime;
    public bool dead;
}
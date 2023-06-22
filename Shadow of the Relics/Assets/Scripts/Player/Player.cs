using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerMovement movement;
    public Animator Anim;

    public float detectCooldown, stopDetectTime, latePositionTime;

    public Vector2 position{get=>transform.position;}
    [HideInInspector] public Vector2 latePosition;

    void Awake()
    {
        movement.player = this;
    }

    float detectTime;
    void FixedUpdate()
    {
        StartCoroutine(SetLatePosition(position));
        if(detectTime > 0f)
        {
            detectTime -= Time.fixedDeltaTime;
        }
    }

    IEnumerator SetLatePosition(Vector2 pos)
    {
        yield return new WaitForSeconds(latePositionTime);
        latePosition = pos;
    }

    public bool detected{get=>detectTime > 0f;}
    public void Seen()
    {
        detectTime = stopDetectTime;
    }
}

public abstract class PlayerBehaviour : MonoBehaviour
{
    [HideInInspector] public Player player;
}
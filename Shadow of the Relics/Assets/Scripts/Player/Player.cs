using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float detectCooldown, stopDetectTime, latePositionTime;

    public Vector2 position{get=>transform.position;}
    [HideInInspector] public Vector2 latePosition;

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

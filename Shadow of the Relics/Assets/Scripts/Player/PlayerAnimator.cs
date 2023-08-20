using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerAnimator : PlayerBehaviour
{
    public string idleAnim, runAnim, jumpAnim, fallAnim, wallAnim, grappleAnim, perchAnim, damagedAnim;
    public float rotateSpeed, damagedOpacity, enterGateTime;
    public AnimationCurve pulseOpacityCurve;
    public int damagedPulseAmount;

    void Update()
    {
        if(Time.timeScale == 0f)
            return;

        PlayDefaultAnimations();
        AnimateRotate();
    }

    void PlayDefaultAnimations()
    {
        if(playing)
            return;

        string newAnim = CurrentDefaultAnimation();
        if(currentAnim == newAnim)
            return;
        currentAnim = newAnim;
        player.Anim.Play(currentAnim);
    }

    string CurrentDefaultAnimation()
    {
        if(!player.movement.isGrounded)
        {
            if(player.movement.onWall != 0f)
                return wallAnim;
            return (player.movement.velocity.y>0f?jumpAnim:fallAnim);
        }
        if(player.movement.velocity.x != 0f)
            return runAnim;
        return idleAnim;
    }

    bool playing;
    string currentAnim = "";
    public void Play(string anim)
    {
        if(currentAnim == anim || anim == "")
            return;
        player.Anim.Play(anim);
        currentAnim = anim;
        playing = true;
    }

    public void Stop()
    {
        currentAnim = "";
        playing = false;
    }

    void AnimateRotate()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    Quaternion targetRotation;
    public void SetRotate(Vector2 direction)
    {
        direction = (direction == Vector2.zero?Vector2.right:direction.normalized * player.activeDir);

        targetRotation = Quaternion.LookRotation(Vector3.forward, new Vector2(-direction.y, direction.x));
    }

    public void SetRotateInstant(Vector2 direction)
    {
        SetRotate(direction);
        transform.rotation = targetRotation;
    }

    float grappleMaxDistance;
    public void GrappleRotateInit(float initDist)
    {
        grappleMaxDistance = Mathf.Sqrt(initDist);
    }

    public void GrappleRotate(Vector2 direction, float dist)
    {
        if(dist <= 0f)
            SetRotate(Vector2.zero);
        else
            SetRotateInstant(Vector3.Slerp(direction, Vector2.right * player.activeDir, Mathf.Sqrt(Mathf.Max(dist-1f,0f))/grappleMaxDistance));
    }

    public override void TakeDamage(float damage, Vector2 origin)
    {
        if(player.dead)
            return;
        StartCoroutine(PulseOpacity(player.damageCooldown, damagedPulseAmount, damagedOpacity));
    }

    public override void Respawn()
    {
        Stop();
    }

    IEnumerator PulseOpacity(float time, int amount, float opacity)
    {
        float pulseSpeed = 2f * (float)amount / time;
        float t = -1f;
        for(int i = 0; i < amount; i++)
        {
            while(t <= 1f)
            {
                Color newColor = player.sprite.color;
                newColor.a = Mathf.Lerp(opacity, 1f, pulseOpacityCurve.Evaluate(Mathf.Abs(t)));
                player.sprite.color = newColor;
                
                yield return null;
                t += pulseSpeed * Time.deltaTime;
            }
            t -= 2f;
        }
        Color newCol = player.sprite.color;
        newCol.a = 1f;
        player.sprite.color = newCol;
    }

    public void EnterGate()
    {
        StartCoroutine(EnterGateOpactity());
    }

    IEnumerator EnterGateOpactity()
    {
        float t = enterGateTime;
        while(t > 0f)
        {
            Color newCol = player.sprite.color;
            newCol.a = t/enterGateTime;
            player.sprite.color = newCol;

            yield return null;
            t -= Time.deltaTime;
        }

        player.SaveEnding();
        SaveManager.Save();
        SaveManager.ResetDelegates();
        SceneManager.LoadScene(2);
    }
}

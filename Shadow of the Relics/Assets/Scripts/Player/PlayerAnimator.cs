using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : PlayerBehaviour
{
    public string idleAnim, runAnim, jumpAnim, fallAnim, wallAnim, grappleAnim, perchAnim;
    public float rotateSpeed;

    void Update()
    {
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
            return (player.movement.velocity.y>0f?jumpAnim:fallAnim);
        if(player.movement.velocity.x != 0f)
            return runAnim;
        return idleAnim;
    }

    bool playing;
    string currentAnim = "";
    public void Play(string anim)
    {
        if(currentAnim == anim)
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
        grappleMaxDistance = initDist;
    }

    public void GrappleRotate(Vector2 direction, float dist)
    {
        if(dist <= 0f)
            SetRotate(Vector2.zero);
        else
            SetRotateInstant(Vector3.Slerp(direction, Vector2.right * player.activeDir, dist/grappleMaxDistance));
    }
}

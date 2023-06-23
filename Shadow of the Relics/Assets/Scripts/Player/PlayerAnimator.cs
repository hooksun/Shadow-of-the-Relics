using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : PlayerBehaviour
{
    public string idleAnim, runAnim, jumpAnim, wallAnim, grappleAnim, perchAnim;
    public float rotateSpeed;

    void Update()
    {
        PlayDefaultAnimations();
        AnimateRotate();
    }

    void PlayDefaultAnimations()
    {
        if(currentAnim != "")
            return;

        if(!player.movement.isGrounded)
        {
            player.Anim.Play(jumpAnim);
            return;
        }
        if(player.movement.velocity.x != 0f)
        {
            player.Anim.Play(runAnim);
            return;
        }
        player.Anim.Play(idleAnim);
    }

    string currentAnim = "";
    public void Play(string anim)
    {
        if(currentAnim == anim)
            return;
        player.Anim.Play(anim);
        currentAnim = anim;
    }

    public void Stop()
    {
        currentAnim = "";
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

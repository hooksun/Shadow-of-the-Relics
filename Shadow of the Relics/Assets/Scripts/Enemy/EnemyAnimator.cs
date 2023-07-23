using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : EnemyBehaviour
{
    public Animator animator;
    public string idleAnim, patrolAnim, noticeAnim, runAnim, jumpAnim, fallAnim;

    string currentAnim;

    public void Play(string anim)
    {
        if(anim == currentAnim)
            return;
        currentAnim = anim;
        animator.Play(anim);
    }
}

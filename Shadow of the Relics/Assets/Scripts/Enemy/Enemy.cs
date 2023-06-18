using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyMovement movement;
    public EnemyVision vision;

    void Awake()
    {
        movement.enemy = this;
        vision.enemy = this;
    }

    public void DetectPlayer()
    {

    }
}

public abstract class EnemyBehaviour: MonoBehaviour
{
    [HideInInspector] public Enemy enemy;
}
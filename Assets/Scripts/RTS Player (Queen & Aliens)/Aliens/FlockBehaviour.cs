using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlockBehaviour : MonoBehaviour
{

    //taken from lab 3
    float repulsionQueryRadius = 5.0f;
    float cohesionQueryRadius = 10.0f;
    public float cohesionFactor = 1.5f;
    public float repulsionFactor = 2.0f;
    //float alignmentFactor = 1.0f;
    public float seekSpeed = 0.5f;
    public Vector3 GoToAttackTarget;
    // Use this for initialization
    public List<EnemyBehaviour> enemies;

    void Awake()
    {
        enemies = new List<EnemyBehaviour>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isFlockGoToAttacking())
        {
            directFlockGoToAttack();
        }
        else if (enemies.Count > 1)
        {
            moveFlock();
        }
    }

    void moveFlock()
    {

        Vector3 avgPos = new Vector3(0.0f, 0.0f, 0.0f);
        foreach (EnemyBehaviour en in enemies)
        {
            avgPos += en.transform.position;
        }
        avgPos /= enemies.Count;
        foreach (EnemyBehaviour en in enemies)
        {
            Vector3 accel = Vector3.zero;
            //cohesion
            if ((avgPos - en.transform.position).magnitude >= cohesionQueryRadius)
            {
                accel += (avgPos - en.transform.position) * cohesionFactor;
            }

            //repulsion
            if ((avgPos - en.transform.position).magnitude < repulsionQueryRadius)
            {
                accel += -(avgPos - en.transform.position) * repulsionFactor;
            }
            //apply
            en.flockAcceleration = accel;
        }

    }

    bool isFlockGoToAttacking()
    {
        foreach (EnemyBehaviour en in enemies)
        {
            if (en.currentState == EnemyBehaviour.State.GoToAttack)
            {
                GoToAttackTarget = en.enemyNextTarget;
                return true;
            }
        }
        return false;
    }
    void directFlockGoToAttack()
    {
        for (int i = 0; i < enemies.Count; i++)
        {

            enemies[i].currentState = EnemyBehaviour.State.GoToAttack;
            enemies[i].enemyNextTarget = GoToAttackTarget;
            //make the npcs surround the tps player
            if (i % 4 == 0)
            {
                enemies[i].assignTarget(GoToAttackTarget + Vector3.forward);
            }
            else if (i % 4 == 1)
            {
                enemies[i].assignTarget(GoToAttackTarget + Vector3.back);
            }
            else if (i % 4 == 2)
            {
                enemies[i].assignTarget(GoToAttackTarget + Vector3.left);
            }
            else
            {
                enemies[i].assignTarget(GoToAttackTarget + Vector3.right);
            }
        }

    }
}
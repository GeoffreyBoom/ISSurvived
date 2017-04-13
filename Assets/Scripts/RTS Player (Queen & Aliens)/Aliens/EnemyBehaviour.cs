using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine.AI;

public class EnemyBehaviour : Photon.MonoBehaviour
{

    public enum State { Idle, MoveTo, Patrol, GoToAttack, AttackTarget, Hide };

    public State currentState = State.Idle;

    public bool isSelected = false;
    bool returnToIdle = false;

    public Vector3 target;
    public Vector3 patrolNextTarget;
    public Vector3 enemyNextTarget;

    //variables for the arrive behaviour
    float nearRadius = 0.5f;
    float arriveRadius = 0.1f;
    float nearMaxSpeed = 10.0f;
    float arriveMaxSpeed = 2.0f;
    float acceleration = 5.0f;

    //variables for flocking
    public Vector3 flockAcceleration = Vector3.zero;
    public Quaternion flockRotation = Quaternion.identity;

    public List<State> futurStates;
    public List<Vector3> futurTargets;

    RTSInterface inter;

    GameObject player;

    NavMeshAgent agent;

    public Vector3 velocity;

    Animator anim;


    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        clearAllFutur();
        inter = FindObjectOfType<RTSInterface>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        checkCurrentState();
    }

    void checkCurrentState()
    {
        /*if (isEnemyDetected())
        {
            if(currentState != State.GoToAttack)
            {
                storeStateAsNext(currentState,target);
                currentState = State.GoToAttack;
                Debug.Log("Enemy Detected");
            }                      
            target = enemyNextTarget;
        }else
        {
            if (currentState == State.GoToAttack)
            {
                currentState = State.Idle;
            }
        }*/
        if (currentState != State.AttackTarget)
        {
            if (isEnemyDetected())
            {
                Debug.Log("Enemy Detected");
            }
            else
            {
                isResourceDetected();
            }
            if (currentState == State.GoToAttack)
            {
                GoToAttackBehaviour();
            }
            else if (currentState == State.MoveTo)
            {
                moveToBehaviour();
            }
            else if (currentState == State.Patrol)
            {
                patrolBehaviour();
            }
            else if (currentState == State.Hide)
            {
                hideBehaviour();
            }
            else
            {
                idleBehaviour();
            }
        }
        else
        {
            attackTargetBehaviour();
        }
    }


    void attackTargetBehaviour()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if ((player.transform.position - transform.position).magnitude < 10.0)
        {
            assignTarget(player.transform.position);
            moving();
        }
        else
        {
            currentState = State.Idle;
        }
    }

    void idleBehaviour()
    {
        if (futurStates.Count != 0)
        {
            if (!returnToIdle)
            {
                storeState(State.MoveTo, transform.position);
                returnToIdle = true;
            }
            goToNextState();
        }
        else
        {
            returnToIdle = false;
        }
    }

    void GoToAttackBehaviour()
    {
        if (!moving())
        {
            anim.SetBool("isWalking", false);
            //TO DO implement GoToAttacking behaviour
            currentState = State.AttackTarget;
        }

    }



    void moveToBehaviour()
    {
        if (!moving())
        {
            anim.SetBool("isWalking", false);
            currentState = State.Idle;
            //goToLastState();
        }
    }

    void patrolBehaviour()
    {
        if (futurStates.Count != 0)
        {
            storeState(currentState, target);
            goToNextState();
        }
        else if (!moving())
        {
            anim.SetBool("isWalking", false);
            //change target
            Vector3 temp = target;
            assignTarget(patrolNextTarget);
            assignPatrolTarget(temp);
        }
    }

    void hideBehaviour()
    {
        //implement hiding behaviour
    }

    //variables for enemy detection
    float detectionRadius = 20.0f;
    float detectionAngle = 150.0f;
    GameObject enemy;
    bool isEnemyDetected()
    {
        /*RaycastHit hit;
        if (Physics.SphereCast(transform.position, 10.0f, transform.forward, out hit, 15.0f))
        {
            if (hit.collider.gameObject.name.Equals("Enemy"))
            {
                enemyNextTarget = hit.collider.gameObject.transform.position;
                return true;
            }
        }*/
        if (enemy != null)
        {
            if ((enemy.transform.position - transform.position).magnitude < detectionRadius)
            {
                Vector3 dir = enemy.transform.position - transform.position;
                if (Vector3.Angle(transform.forward, dir) < detectionAngle)
                {
                    return true;
                }
            }
        }
        else
        {
            enemy = GameObject.FindGameObjectWithTag("Player");
        }
        return false;
    }

    void isResourceDetected()
    {
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");
        foreach (GameObject res in resources)
        {
            if ((res.transform.position - transform.position).magnitude < detectionRadius)
            {
                Vector3 dir = res.transform.position - transform.position;
                if (Vector3.Angle(transform.forward, dir) > detectionAngle)
                {
                    storeState(State.MoveTo, res.transform.position);
                }
            }
        }
        /*RaycastHit hit;
        if (Physics.SphereCast(transform.position, 10.0f, transform.forward, out hit, 15.0f))
        {
            if (hit.collider.gameObject.name.Equals("Resource") && hit.collider.gameObject.transform.position != target)
            {
                storeState(State.MoveTo, hit.collider.gameObject.transform.position);
            }
        }*/
    }

    public bool moving()
    {
        Debug.Log("In moving() function");
        //the direction in which to move
        Vector3 direction = (target - transform.position).normalized;
        //the distance to the target
        float dist = (target - transform.position).magnitude;

        //if closer than arrive radius
        if (dist < arriveRadius)
        {
            //move directly to target then stop moving 
            transform.position = target;
            flockAcceleration = Vector3.zero;
            return false;
        }
        //if not at target
        //turn towards target (align)
        //move towards target (arrive)
        //if not oriented towards target
        if (transform.forward.Equals( direction))
        {
            Debug.Log("In the if statement of transform.foward " + transform.forward + " " + direction);
            //turn towards target using align
            Quaternion fwr = Quaternion.LookRotation(transform.forward);
            Quaternion dir = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(fwr, dir, 0.20f);
        }
        else
        {
            agent.SetDestination(target);
            Debug.Log(agent.nextPosition);
            //move to target
            //if before the near radius
            if (dist > nearRadius)
            {
                //accelerate towards target (or move at max speed)
                velocity += direction * acceleration + flockAcceleration;
                if (velocity.magnitude > nearMaxSpeed)
                {
                    velocity = direction * nearMaxSpeed;
                }
            }//if before arrive radius
            else if (dist > arriveRadius)
            {

                //accelerate towards target (or move at max speed)
                velocity += direction * acceleration;
                if (velocity.magnitude > arriveMaxSpeed)
                {
                    velocity = direction * arriveMaxSpeed;
                }
            }
            //move npc
            agent.speed = velocity.magnitude;
        }
        flockAcceleration = Vector3.zero;
        return true;
    }

    public void assignTarget(Vector3 newTarget)
    {
        target = newTarget;
    }
    public void assignPatrolTarget(Vector3 newTarget)
    {
        patrolNextTarget = newTarget;
    }

    void goToNextState()
    {
        if (futurStates.Count != 0)
        {
            currentState = futurStates[0];
            target = futurTargets[0];
            futurStates.RemoveAt(0);
            futurTargets.RemoveAt(0);

        }
    }
    void storeStateAsNext(State nState, Vector3 nTarget)
    {
        bool canAdd = true;
        for (int i = 0; i < futurTargets.Count; i++)
        {
            if (futurTargets[i] == nTarget)
            {
                canAdd = false;
                break;
            }
        }

        if (canAdd)
        {
            futurTargets.Insert(0, nTarget);
            futurStates.Insert(0, nState);
        }
    }
    void storeState(State nState, Vector3 nTarget)
    {
        bool canAdd = true;
        for (int i = 0; i < futurTargets.Count; i++)
        {
            if (futurTargets[i] == nTarget)
            {
                canAdd = false;
                break;
            }
        }

        if (canAdd)
        {
            futurTargets.Add(nTarget);
            futurStates.Add(nState);
        }
    }
    public void clearAllFutur()
    {
        futurTargets = new List<Vector3>();
        futurStates = new List<State>();
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Hit");
        if (col.gameObject.tag.Equals("Resource"))
        {
            Debug.Log("Hit by npc");
            inter.GotResource();
        }
        if (col.gameObject.tag.Equals("Player"))
        {
            transform.position -= transform.forward * 2;
        }

    }

    public Vector3 getTarget()
    {
        return target;
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

        }
        else
        {
            // Network player, receive data
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();

        }
    }
}
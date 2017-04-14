using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine.AI;

public class EnemyBehaviour : Photon.MonoBehaviour
{
    //The possible states of the npc
    public enum State { Idle, MoveTo, Patrol, GoToAttack, AttackTarget };

    public State currentState = State.Idle;

    bool returnToIdle = false;

    public Vector3 target;
    public Vector3 patrolNextTarget;

    //variables for detection
    float detectionRadius = 20.0f;
    float detectionAngle = 150.0f;
    GameObject enemy;

    //variables for the arrive behaviour
    float nearRadius = 0.5f;
    float arriveRadius = 0.1f;
    float nearMaxSpeed = 10.0f;
    float arriveMaxSpeed = 2.0f;
    float acceleration = 5.0f;

    float currentSpeed = 0;

    //variables for flocking
    public Vector3 flockAcceleration = Vector3.zero;

    public List<State> futurStates;
    public List<Vector3> futurTargets;

    RTSInterface inter;

    GameObject player;

    NavMeshAgent agent;

    Animator anim;
    //variables to control npc state when going to attack player
    public bool alerted = false;
    public int currentIndex;
    int index = 0;
    bool surround = false;

    // Use this for initialization
    void Start()
    {
        //getting the references to other objects
        agent = GetComponent<NavMeshAgent>();
        inter = FindObjectOfType<RTSInterface>();
        anim = GetComponent<Animator>();
        //initializing lists
        clearAllFutur();
    }

    // Update is called once per frame
    void Update()
    {
        checkCurrentState();
    }
    //method to check which state the npc is in and act accordingly
    void checkCurrentState()
    {

        if (currentState != State.AttackTarget)
        {//if not attacking the player
            if (!isEnemyDetected())
            {
                //see if resources are detected
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

    //if close enough to the player to attack her
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
            //if the player gets to far away, return to idle
            currentState = State.Idle;
        }
    }

    void idleBehaviour()
    {
        //default state
        alerted = false;
        currentIndex = 0;
        index = 0;
        //see if there are states in the state list to go to
        if (futurStates.Count != 0)
        {
            //add a state so that the enemy returns to this position after going through the other states
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
        //the enemies try to surround the player
        if (surround == false)
        {
            if (currentIndex % 4 == 0)
            {
                assignTarget(target + Vector3.forward);
            }
            else if (currentIndex % 4 == 1)
            {
                assignTarget(target + Vector3.back);
            }
            else if (currentIndex % 4 == 2)
            {
                assignTarget(target + Vector3.left);
            }
            else
            {
                assignTarget(target + Vector3.right);
            }
            surround = true;
        }
        //if they get close enough, they start attacking
        if (!moving())
        {
            anim.SetBool("isWalking", false);
            currentState = State.AttackTarget;
            surround = false;
        }
    }


    void callAllFlock()
    {
        //alerts the surrounding enemies of the presence of the player if the npc hasn't been alerted first
        if (alerted == false)
        {
            alerted = true;
            currentIndex = index++;
            GameObject[] aliens = GameObject.FindGameObjectsWithTag("Alien");
            foreach (GameObject alien in aliens)
            {
                //checks to see which npcs are close enough
                if ((alien.transform.position - transform.position).magnitude < detectionRadius)
                {
                    EnemyBehaviour en = alien.GetComponent<EnemyBehaviour>();
                    en.alerted = true;
                    //makes them go attack
                    if (en.currentState != State.AttackTarget)
                    {
                        en.currentState = State.GoToAttack;
                        en.assignTarget(target);
                        en.currentIndex = index++;
                    }
                }
            }
        }
    }

    void moveToBehaviour()
    {
        //state where the npc moves towards its target
        if (!moving())
        {
            anim.SetBool("isWalking", false);
            currentState = State.Idle;
        }
    }

    void patrolBehaviour()
    {
        //if there are other states in the list (the npc has seen some resource) it will go there before resuming its patrol
        if (futurStates.Count != 0)
        {
            storeState(currentState, target);
            goToNextState();
        }
        else if (!moving())
        {   //if not, it will patrol until someone makes it stop
            anim.SetBool("isWalking", false);
            //change target
            Vector3 temp = target;
            assignTarget(patrolNextTarget);
            assignPatrolTarget(temp);
        }
    }

    bool isEnemyDetected()
    {

        if (enemy != null)
        {   //tries to see if the player is close enough to detect
            if ((enemy.transform.position - transform.position).magnitude < detectionRadius)
            {   //sees if the player is in front
                Vector3 dir = enemy.transform.position - transform.position;
                if (Vector3.Angle(transform.forward, dir) < detectionAngle)
                {
                    //if that is the case, the npc alerts the others, and goes to attack
                    assignTarget(enemy.transform.position);
                    currentState = State.GoToAttack;
                    callAllFlock();
                    return true;
                }
            }
        }
        else
        {
            //if the player hasn't been instantiated yet, try referencing him until he is
            enemy = GameObject.FindGameObjectWithTag("Player");
        }
        return false;
    }

    void isResourceDetected()
    {
        //a method to detect if the npc can see resources
        GameObject[] resources = GameObject.FindGameObjectsWithTag("Resource");
        foreach (GameObject res in resources)
        {
            if ((res.transform.position - transform.position).magnitude < detectionRadius)
            {
                // if that is the case, store its location 
                Vector3 dir = res.transform.position - transform.position;
                if (Vector3.Angle(transform.forward, dir) > detectionAngle)
                {
                    storeState(State.MoveTo, res.transform.position);
                }
            }
        }
    }

    public bool moving()
    {
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
        if (transform.forward.Equals(direction))
        {
            //turn towards target using align
            Quaternion fwr = Quaternion.LookRotation(transform.forward);
            Quaternion dir = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(fwr, dir, 0.20f);
        }
        else
        {
            agent.SetDestination(target);
            //move to target
            //if before the near radius
            if (dist > nearRadius)
            {
                //accelerate towards target (or move at max speed)
                currentSpeed += acceleration;
                if (currentSpeed > nearMaxSpeed)
                {
                    currentSpeed = nearMaxSpeed;
                }
            }//if before arrive radius
            else if (dist > arriveRadius)
            {
                //accelerate towards target (or move at max speed)
                currentSpeed += acceleration;
                if (currentSpeed > arriveMaxSpeed)
                {
                    currentSpeed = arriveMaxSpeed;
                }
            }
            //move npc
            agent.speed = currentSpeed;
        }
        flockAcceleration = Vector3.zero;
        return true;
    }
    //method to assign a new target
    public void assignTarget(Vector3 newTarget)
    {
        target = newTarget;
    }
    //method to assign a new patrol target
    public void assignPatrolTarget(Vector3 newTarget)
    {
        patrolNextTarget = newTarget;
    }

    void storeState(State nState, Vector3 nTarget)
    {
        //a method to add a futur state (for when resources are detected)
        bool canAdd = true;
        for (int i = 0; i < futurTargets.Count; i++)
        {
            //if the target already exists, dont add it
            if (futurTargets[i] == nTarget)
            {
                canAdd = false;
                break;
            }
        }
        //otherwise, add it as a futur state
        if (canAdd)
        {
            futurTargets.Add(nTarget);
            futurStates.Add(nState);
        }
    }

    void goToNextState()
    {
        //go to the first state in the list, and remove it from the list
        if (futurStates.Count != 0)
        {
            currentState = futurStates[0];
            target = futurTargets[0];
            futurStates.RemoveAt(0);
            futurTargets.RemoveAt(0);

        }
    }

    public void clearAllFutur()
    {
        //reinitialize the list
        futurTargets = new List<Vector3>();
        futurStates = new List<State>();
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
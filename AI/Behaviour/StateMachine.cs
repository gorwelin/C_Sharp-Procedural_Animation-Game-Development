using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// https://www.dropbox.com/s/5njl0652xfiiw78/ChickenFSM.cs?dl=0
// Used as reference to build entire class
public class StateMachine : MonoBehaviour
{
    // Determines behaviour of AI
    public int AIlevel;
    // Enum for behavioural state of AI at any given time
    public enum State {Patrol, Seek, Flee, Attack, Wander};
    public State state;

    // Object classes
    public GameObject player;
    public NavMeshAgent agent;
    public Animator animator;

    // Determines how far AI will flee
    public float maxDistance = 50;
    // Distance between player and AI, updated constantly
    public float distance;
    // Speed of AI, NavMeshAgent handles speed mostly
    public float speed;
    
    // Behaviour variables, e.g. waypoints and destPoint for Patrol
    public Transform[] waypoints;
    public int destPoint = 0;
    // Distances for AI to determine which state to exhibit
    public float distanceToFlee;
    public float distanceToSeek;
    public float distanceToAttack;

    // AI can only see at a radius of approx 90 degrees ahead using this class
    public FieldOfView fov;

    // 3 vars used for Wander function
    private float timer;
    public float wanderRadius;
    public float wanderTimer;

    // Initalising objects, assigning defaults and setting default State
    void Start()
    {
        fov = GetComponent<FieldOfView>();
        agent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        speed = agent.speed;
        animator.SetBool("isMoving", true);
        animator.SetBool("inAttackDistance", false);

        switch (AIlevel) {
            case 1:
                state = State.Patrol;
                GotoNextPoint();
                break;
            case 2:
                state = State.Wander;
                break;
            default:
                state = State.Wander;
                break;
        }
    }

    void Update()
    {
        distance = Vector3.Distance(transform.position, player.transform.position);

        // Change the behaviour of an AI depending on level, small bees will flee unless player gets too close whereas bigger bee will attack on sight and wanders in a radius
        if(AIlevel == 1) {
            if ((distance <= distanceToFlee && distance > distanceToSeek) && fov.visibleTargets.Count > 0) {
                state = State.Flee;
            } else if ((distance < distanceToSeek && distance > distanceToAttack) && fov.visibleTargets.Count > 0) {
                state = State.Seek;
            } else if (distance < distanceToAttack) {
                state = State.Attack;
            } else {
                state = State.Patrol;
            }
        } else if(AIlevel == 2) {
            if (distance <= distanceToFlee && distance > distanceToSeek && fov.visibleTargets.Count > 0) {
                state = State.Seek;
            } else if (distance <= distanceToSeek) {
                state = State.Attack;
            } else {
                state = State.Wander;
            }
        }
 

            // switch state machine, calls methods depending on if statement above, resets attack animation also
            switch (state) {
            case State.Patrol:
                animator.SetBool("inAttackDistance", false);
                StartCoroutine(Patrol());
                break;
            case State.Seek:
                animator.SetBool("inAttackDistance", false);
                Seek();
                break;
            case State.Flee:
                animator.SetBool("inAttackDistance", false);
                Flee();
                break;
            case State.Attack:
                Attack();
                break;
            case State.Wander:
                animator.SetBool("inAttackDistance", false);
                StartCoroutine(Wander());
                break;
            default:

                StartCoroutine(Wander());
                break;
        }
    }

    // Used Dr Cenydd's Seek class to implement
    public void Seek() {
        agent.speed = (float)(speed / 0.75);
        agent.SetDestination(player.transform.position);
    }

    // similar to seek, but implements attacking animations also and increases speed
    public void Attack() {
        agent.SetDestination(player.transform.position);

            animator.SetBool("inAttackDistance", true);
    }

    // Coroutine to issue request to keep AI path following when not in vicinity of player
    public IEnumerator Patrol() {
       
        // if agent has almost reaches the target, repeat same method
        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            GotoNextPoint();
        }
            


        yield return null;
    }

    void GotoNextPoint() {
        // Returns if no points have been set up
        if (waypoints.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = waypoints[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % waypoints.Length;
    }

    // Using Dr cenydds tutorials as guidance, modifying for flee and changing the state if it's successful
    public void Flee() {

        Vector3 desired_velocity2 = (transform.position - player.transform.position).normalized * maxDistance;
        Vector3 steering = desired_velocity2 - agent.velocity;


        agent.SetDestination(steering);

        if (!agent.pathPending && agent.remainingDistance < 0.5f) {
            state = State.Patrol;
        }
    }

    /**
     * https://gist.github.com/Templar2020/8e4f5296de96d8ccf03263bf1a9f277f
     * 
     * Algorithm inspired by this github post
     * couroutine similar to patrol so AI will always follow it as default
     * 
     **/
    public IEnumerator Wander() {
        timer += Time.deltaTime;

        // Radius of area AI can search and setting destination using that sphere
        if (timer >= wanderTimer) {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
        yield return null;
    }

    // Method to generate new sphere for navigation with Wander() 
    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

}

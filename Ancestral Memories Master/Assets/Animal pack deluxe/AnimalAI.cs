//You are free to use this script in Free or Commercial projects
//sharpcoderblog.com @2019
//https://sharpcoderblog.com/blog/unity-3d-deer-ai-tutorial

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using FIMSpace.FLook;

public class AnimalAI : MonoBehaviour
{

    const string IDLE = "idle";
    const string RUN = "run";
    const string WALK = "walk";
    const string ATTACK = "attack";
    const string EAT = "eat";
    const string DIE = "die";

    public enum AIState { Idle, Walking, Eating, Running, Following, Dialogue }

    [SerializeField] private AIState state = AIState.Idle;


    [SerializeField] private AIState currentAIState;
    [SerializeField] private bool behaviourActive = false;

    [SerializeField] private float walkingSpeed = 3.5f;
    [SerializeField] private float runningSpeed = 11;

    private Animator animator;
    [SerializeField] private float animationCrossFade = 0.5f;

    NavMeshAgent agent;

    [SerializeField] float fleeMultiplier = 1;
    bool reverseFlee = false;
    Vector3 closestEdge;
    float distanceToEdge;

    [SerializeField] float distanceToPlayer;

    public FLookAnimator lookAnimator;
    private Interactable interactable;

    List<Vector3> previousIdlePoints = new List<Vector3>();

    public CharacterBehaviours playerBehaviours;

    float timeStuck = 0;
    [SerializeField] private bool shouldFlee = true;
    [SerializeField] private bool follow = false;

    public Player player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        agent.autoBraking = false;

        animator = transform.GetComponentInChildren<Animator>();

        interactable = transform.GetComponent<Interactable>();

        //ChangeState(AIState.Idle);
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        UpdateRange();

        if (player.faith <= 50)
        {
            shouldFlee = true;
            interactable.enabled = false;

            if (!inRange)
            {
                lookAnimator.enabled = false;
            }
        }

        else if (player.faith >= 50)
        {
            shouldFlee = false;
            interactable.enabled = true;

            if (inRange)
            {
                lookAnimator.enabled = true;
            }
        }

        if (playerBehaviours.isPsychdelicMode)
        {
            follow = true;
            agent.stoppingDistance = 15f;
        }
        else
        {
            follow = false;
            agent.stoppingDistance = 0f;
        }

    }

    [SerializeField] float time;

    private IEnumerator Idle()
    {
        ChangeAnimationState(IDLE);

        behaviourActive = true;

        previousIdlePoints.Add(transform.position);

        if (previousIdlePoints.Count > 5)
        {
            previousIdlePoints.RemoveAt(0);
        }

        time = Random.Range(4, 8);

        while (behaviourActive)
        {
            while (time >= 0)
            {
                time -= Time.deltaTime;

                yield return null;
            }

            if (time <= 0)
            {
                if (inRange && follow)
                {
                    ChangeState(AIState.Following);
                }
                else
                {
                    ChangeState(AIState.Walking);
                }
            }

            yield return null;
        }

        yield break;
    }

    private IEnumerator Walk(Vector3 destination)
    {
        ChangeAnimationState(WALK);

        behaviourActive = true;

        agent.speed = walkingSpeed;
        agent.SetDestination(destination);

        while (behaviourActive)
        {
            if (DoneReachingDestination())
            {
                if (!inRange)
                {
                    ChangeState(AIState.Eating);
                } 
            }

            yield return null;
        }

        yield break;
    }

    private void ChangeState(AIState newState)
    {
        StopAllCoroutines();
        behaviourActive = false;
        currentAIState = newState;
        state = currentAIState;

        switch (state)
        {
            case AIState.Idle:
                StartCoroutine(Idle());
                break;
            case AIState.Eating:
                StartCoroutine(Eat());
                break;
            case AIState.Walking:
                StartCoroutine(Walk(RandomNavSphere(transform.position, Random.Range(3, 7))));
                break;
            case AIState.Running:
                StartCoroutine(Run());
                break;
            case AIState.Following:
                StartCoroutine(Follow());
                break;
            case AIState.Dialogue:
                StartCoroutine(DialogueActive());
                break;
            default:
                break;
        }
       
        Debug.Log(currentAIState);
    }

    private IEnumerator DialogueActive()
    {
        behaviourActive = true;

        while (behaviourActive)
        {
            ChangeAnimationState(IDLE);

            //agent.transform.LookAt(player.transform);

            yield return null;
        }

        yield break;
    }

    private IEnumerator Eat()
    {
        ChangeAnimationState(EAT);

        behaviourActive = true;

        while (behaviourActive)
        {
            if (!animator || animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f)
            {
                ChangeState(AIState.Walking);
            }

            yield return null;
        }

        yield break;
    }

    float distance;

    private IEnumerator Run()
    {
        ChangeAnimationState(RUN);

        agent.speed = runningSpeed;

        behaviourActive = true;

        while (behaviourActive)
        {
            //Set NavMesh Agent Speed
            agent.speed = runningSpeed;
            if (reverseFlee)
            {
                if (DoneReachingDestination() && timeStuck < 0)
                {
                    reverseFlee = false;
                }
                else
                {
                    timeStuck -= Time.deltaTime;
                }
            }
            else
            {
                Vector3 runTo = transform.position + ((transform.position - player.transform.position) * fleeMultiplier);
                distance = (transform.position - player.transform.position).sqrMagnitude;
                //Find the closest NavMesh edge
                NavMeshHit hit;
                if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
                {
                    closestEdge = hit.position;
                    distanceToEdge = hit.distance;
                    //Debug.DrawLine(transform.position, closestEdge, Color.red);
                }
                if (distanceToEdge < 1f)
                {
                    if (timeStuck > 1.5f)
                    {
                        if (previousIdlePoints.Count > 0)
                        {
                            runTo = previousIdlePoints[Random.Range(0, previousIdlePoints.Count - 1)];
                            reverseFlee = true;
                        }
                    }
                    else
                    {
                        timeStuck += Time.deltaTime;
                    }
                }
                if (distance < rangeThreshold * rangeThreshold)
                {
                    agent.SetDestination(runTo);
                }
            }

            if (agent.velocity.sqrMagnitude < 0.1f * 0.1f)
            {
                ChangeState(AIState.Idle);
            }

            else
            {
                if (DoneReachingDestination())
                {
                    ChangeState(AIState.Idle);
                }
            }

            yield return null;
        }
    }

    private IEnumerator Follow()
    {
        behaviourActive = true;

        while (behaviourActive)
        {
            if (!inRange || !follow)
            {
                ChangeState(AIState.Idle);
            }
            else
            {
                ChangeAnimationState(WALK);
                agent.SetDestination(player.transform.position);
            }

            yield return null;
        }

        yield break;
    }

    bool DoneReachingDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    //Done reaching the Destination
                    return true;
                }
            }
        }

        return false;
    }

    public float rangeThreshold = 50;
    public bool inRange = false;

    private void UpdateRange()
    {
        if (distanceToPlayer < rangeThreshold)
        {
            inRange = true;
        }
        else if (distanceToPlayer >= rangeThreshold)
        {
            inRange = false;
        }
    }


    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);

        return navHit.position;
    }

    private string currentState;

    public virtual void ChangeAnimationState(string newState)
    {

        float crossFadeLength = animationCrossFade;

        if (currentState == newState)
        {
            return;
        }

        animator.CrossFadeInFixedTime(newState, crossFadeLength);

        currentState = newState;
    }

    public virtual void AdjustAnimationSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }
}
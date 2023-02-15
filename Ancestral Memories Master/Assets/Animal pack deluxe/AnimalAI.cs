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

    private float agentRadius = 0.4f;

    private PlayFlute fluteControl;

    private FollowersManager followManager;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.autoBraking = false;
        agent.radius = agentRadius;

        animator = transform.GetComponentInChildren<Animator>();

        interactable = transform.GetComponent<Interactable>();
        fluteControl = player.GetComponentInChildren<PlayFlute>();
        followManager = player.GetComponentInChildren<FollowersManager>();

        ChangeState(AIState.Idle);
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        UpdateRange();

        if (player.faith <= 50)
        {
            shouldFlee = true;
            interactable.enabled = false;

            if (!inRange && !fluteControl.fluteActive)
            {
                lookAnimator.enabled = false;
            }
        }

        else if (player.faith >= 50)
        {
            shouldFlee = false;
            interactable.enabled = true;

            if (inRange || fluteControl.fluteActive)
            {
                lookAnimator.enabled = true;
            }
        }

    }

    [SerializeField] float time;
    [SerializeField] float minActionBuffer = 3;
    [SerializeField] float maxActionBuffer = 10;

    private bool overriden = false;

    private IEnumerator Idle()
    {
        ChangeAnimationState(IDLE);

        agent.speed = 0f;
        agent.ResetPath();

        previousIdlePoints.Add(transform.position);

        if (previousIdlePoints.Count > 5)
        {
            previousIdlePoints.RemoveAt(0);
        }

        time = Random.Range(minActionBuffer, maxActionBuffer);

        behaviourActive = true;

        while (behaviourActive)
        {
            while (time >= 0)
            {
                if (!inRange)
                {
                    if (playerBehaviours.isPsychdelicMode && inRange && player.isBlessed || fluteControl.fluteActive)
                    {
                        ChangeState(AIState.Following);
                    }
                }

                time -= Time.deltaTime;

                yield return null;
            }

            int minChance = 0;
            int maxChance = 4;
            int randNumber = Random.Range(minChance, maxChance);

            if (time <= 0)
            {

                if (randNumber <= maxChance / 2)
                {
                    ChangeState(AIState.Walking);
                } else {
                    ChangeState(AIState.Idle);
                }
            }

            yield return null;
        }

        yield break;
    }

    private IEnumerator Walk(Vector3 destination)
    {
        ChangeAnimationState(WALK);

        agent.speed = walkingSpeed;
        agent.SetDestination(destination);

        behaviourActive = true;

        while (behaviourActive)
        {
            if (playerBehaviours.isPsychdelicMode && inRange && player.isBlessed || fluteControl.fluteActive)
            {
                ChangeState(AIState.Following);
            } else if (DoneReachingDestination())
            {
                ChangeState(AIState.Eating);
            }

            yield return null;
        }

        yield break;
    }

    [SerializeField] float minRandWalkDistance = 0;
    [SerializeField] float maxRandWalkDistance = 15;
    [SerializeField] float randWalkDistance;

    private void ChangeState(AIState newState)
    {
        StopAllCoroutines();
        behaviourActive = false;
        currentAIState = newState;
        state = currentAIState;

        if (state == AIState.Following)
        {
            followManager.AddFollower(transform.gameObject);
            agent.stoppingDistance = 15f;
        } else
        {
            agent.stoppingDistance = 0f;
        }

        if (state != AIState.Following && followManager.followers.Contains(transform.gameObject))
        {
            followManager.RemoveFollower(transform.gameObject);
        }

        randWalkDistance = Random.Range(minRandWalkDistance, maxRandWalkDistance);

        switch (state)
        {
            case AIState.Idle:
                StartCoroutine(Idle());
                break;
            case AIState.Eating:
                StartCoroutine(Eat());
                break;
            case AIState.Walking:
                StartCoroutine(Walk(RandomNavSphere(transform.position, randWalkDistance)));
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
            while (time >= 0)
            {
                if (!inRange)
                {
                    if (playerBehaviours.isPsychdelicMode && inRange && player.isBlessed || fluteControl.fluteActive)
                    {
                        ChangeState(AIState.Following);
                    }
                }

                time -= Time.deltaTime;

                yield return null;
            }

            if (time <= 0)
            {
                int minChance = 0;
                int maxChance = 4;
                int randNumber = Random.Range(minChance, maxChance);

                if (randNumber <= maxChance / 2)
                {
                    ChangeState(AIState.Walking);
                }
                else
                {
                    ChangeState(AIState.Idle);
                }
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
        agent.speed = walkingSpeed;

        while (behaviourActive)
        {
            ChangeAnimationState(WALK);

            agent.SetDestination(player.transform.position);

            if (DoneReachingDestination() || inRange)
            {              
                ChangeState(AIState.Idle);
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
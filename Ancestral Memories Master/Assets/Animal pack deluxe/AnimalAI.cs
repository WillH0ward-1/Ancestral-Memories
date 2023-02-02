//You are free to use this script in Free or Commercial projects
//sharpcoderblog.com @2019
//https://sharpcoderblog.com/blog/unity-3d-deer-ai-tutorial

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class AnimalAI : MonoBehaviour
{

    const string IDLE = "idle";
    const string RUN = "run";
    const string WALK = "walk";
    const string ATTACK = "attack";
    const string EAT = "eat";
    const string DIE = "die";

    public enum AIState { Idle, Walking, Eating, Running, Following, Dialogue }

    [SerializeField] public AIState state = AIState.Idle;

    [SerializeField] private float walkingSpeed = 3.5f;
    [SerializeField] private float runningSpeed = 11;
    public Animator animator;

    [SerializeField] private float animationCrossFade = 0.5f;

    SphereCollider sphereCollider;
    NavMeshAgent agent;

    [SerializeField] private bool shouldFlee = true;
    bool switchAction = false;
    float actionTimer = 0; 
    public Player player;

    [SerializeField] float fleeDistanceMultiplier = 1;
    bool reverseFlee = false; // If stuck, send back to a previous Idle point

    //Detect whether the AI is stuck
    Vector3 closestEdge;
    float distanceToEdge;
    float distance;

    //How long the AI has been near the edge of NavMesh, if too long, send it to one of the random previousIdlePoints
    float timeStuck = 0;
    private Interactable interactable;

    //Store previous idle points for reference
    List<Vector3> previousIdlePoints = new List<Vector3>();

    private CharacterBehaviours playerBehaviours;

    public LookAtTarget lookAtTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        agent.autoBraking = false;

        state = AIState.Idle;
        actionTimer = Random.Range(0.1f, 2.0f);

        interactable = transform.GetComponent<Interactable>();
        lookAtTarget = transform.GetComponentInChildren<LookAtTarget>();
    }

    void Update()
    {
        if (player.faith <= 50)
        {
            shouldFlee = true;
            interactable.enabled = false;
            lookAtTarget.enabled = false;
        }
        else if (player.faith >= 50 || playerBehaviours.isPsychdelicMode)
        {
            shouldFlee = false;
            interactable.enabled = true;
            lookAtTarget.enabled = true;
        }

        if (agent.velocity.sqrMagnitude < 0.1f * 0.1f && currentAIState != AIState.Idle);
        {
            state = AIState.Idle;
        }

        if (!behaviourActive)
        {
            switch (state)
            {
                case AIState.Idle:
                    if (currentAIState != AIState.Idle)
                    {
                        StartCoroutine(Idle());
                    }
                    break;
                case AIState.Eating:
                    if (currentAIState != AIState.Eating)
                    {
                        StartCoroutine(Eat());
                    }
                    break;
                case AIState.Walking:
                    if (currentAIState != AIState.Walking)
                    {
                        StartCoroutine(Walk());
                    }
                    break;
                case AIState.Running:
                    if (currentAIState != AIState.Running)
                    {
                        StartCoroutine(Run());
                    }
                    break;
                case AIState.Following:
                    if (currentAIState != AIState.Following)
                    {
                        StartCoroutine(Follow());
                    }
                    break;
                case AIState.Dialogue:
                    if (currentAIState != AIState.Dialogue)
                    {
                        StartCoroutine(DialogueActive());
                    }
                    break;
                default:
                    break;
            }
        }

    }

    private bool behaviourActive = true;

    private IEnumerator Idle()
    {
        ChangeAnimationState(IDLE);

        behaviourActive = true;

        while (behaviourActive)
        {
            if (InRange() && !shouldFlee)
            {
                ChangeState(AIState.Following);
            } else 

            if (InRange() && shouldFlee)
            {  
                ChangeState(AIState.Running);
            }

            else if (!InRange()) 
            {

                //No enemies nearby, start eating
                actionTimer = Random.Range(14, 22);

                ChangeState(AIState.Eating);

                //Keep last 5 Idle positions for future reference
                previousIdlePoints.Add(transform.position);

                if (previousIdlePoints.Count > 5)
                {
                    previousIdlePoints.RemoveAt(0);
                }
            }

            yield return null;
        }

        yield break;
    }

    private IEnumerator RandomWalk()
    {
   
        agent.destination = RandomNavSphere(transform.position, Random.Range(3, 7));
        state = AIState.Walking;
        ChangeAnimationState(WALK);

        yield return null;
    }

    private void ChangeState(AIState newState)
    {
        behaviourActive = false;
        currentAIState = newState;
        state = newState;
    }

    private IEnumerator Walk()
    {
        behaviourActive = true;

        while (behaviourActive)
        {
            agent.speed = walkingSpeed;

            // Check if we've reached the destination
            if (DoneReachingDestination())
            {
                ChangeState(AIState.Idle);
            }

            yield return null;
        }

        yield break;
    }

    private IEnumerator DialogueActive()
    {
        behaviourActive = true;

        while (behaviourActive)
        {
            ChangeAnimationState(IDLE);

            agent.transform.LookAt(player.transform);

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
            //Wait for current animation to finish playing
            if (!animator || animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f)
            {
                //Walk to another random destination
                agent.destination = RandomNavSphere(transform.position, Random.Range(3, 7));
                ChangeState(AIState.Walking);

            }
            yield return null;
        }

        yield break;
    }

    private IEnumerator Run()
    {
        ChangeAnimationState(RUN);
        agent.speed = runningSpeed;

        agent.SetDestination(RandomNavSphere(transform.position, Random.Range(1, 2.4f)));

        behaviourActive = true;

        while (behaviourActive)
        {
            if (reverseFlee)
            {
                if (DoneReachingDestination() && timeStuck < 0)
                {
                    reverseFlee = false;
                    ChangeState(AIState.Idle);
                }
                else
                {
                    timeStuck -= Time.deltaTime;
                }
            }
            else
            {
                Vector3 runTo = transform.position + ((transform.position - player.transform.position) * fleeDistanceMultiplier);
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

                if (agent.velocity.sqrMagnitude < 0.1f * 0.1f)
                {
                    ChangeAnimationState(IDLE);
                }
                else
                {
                    ChangeAnimationState(RUN);
                }
            }

            yield return null;
        }

        yield break;
    }

    public AIState currentAIState;

    private IEnumerator Follow()
    {
        behaviourActive = true;

        while (behaviourActive)
        {
            if (InRange())
            {
                ChangeAnimationState(WALK);
                agent.SetDestination(player.transform.position);
            } else
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

    private bool InRange()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= rangeThreshold)
        {
            inRange = true;
            return true;
        } else
        {
            inRange = false;
            return false;
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
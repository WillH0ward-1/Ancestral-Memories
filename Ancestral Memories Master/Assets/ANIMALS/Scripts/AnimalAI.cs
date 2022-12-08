//You are free to use this script in Free or Commercial projects
//sharpcoderblog.com @2019

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AnimalAI : MonoBehaviour
{

    const string IDLE = "idle";
    const string RUN = "run";
    const string WALK = "walk";
    const string ATTACK = "attack";
    const string EAT = "eat";
    const string DIE = "die";

    public enum AIState { Idle, Walking, Eating, Running }
    [SerializeField] public AIState state = AIState.Idle;
    [SerializeField] private int awarenessArea = 15;
    [SerializeField] private float walkingSpeed = 3.5f;
    [SerializeField] private float runningSpeed = 7f;
    public Animator animator;

    [SerializeField] private float animationCrossFade = 2f;

    //Trigger collider that represents the awareness area
    SphereCollider sphereCollider;
    //NavMesh Agent
    NavMeshAgent agent;

    [SerializeField] private bool shouldFlee = true;
    bool switchAction = false;
    float actionTimer = 0; 
    [SerializeField] Transform player;
    [SerializeField] float range = 20; //How far the Deer have to run to resume the usual activities
    float multiplier = 1;
    bool reverseFlee = false; //In case the AI is stuck, send it to one of the original Idle points

    //Detect NavMesh edges to detect whether the AI is stuck
    Vector3 closestEdge;
    float distanceToEdge;
    float distance; //Squared distance to the enemy
    //How long the AI has been near the edge of NavMesh, if too long, send it to one of the random previousIdlePoints
    float timeStuck = 0;
    //Store previous idle points for reference
    List<Vector3> previousIdlePoints = new List<Vector3>();

    private CharacterBehaviours playerBehaviours;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        agent.autoBraking = false;

        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = awarenessArea;

        //Initialize the AI state
        state = AIState.Idle;
        actionTimer = Random.Range(0.1f, 2.0f);
        ChangeAnimationState(IDLE);

        playerBehaviours = player.GetComponent<CharacterBehaviours>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerBehaviours.dialogueIsActive)
        {

            //Wait for the next course of action
            if (actionTimer > 0)
            {
                actionTimer -= Time.deltaTime;
            }
            else
            {
                switchAction = true;
            }

            if (state == AIState.Idle)
            {
                if (switchAction)
                {
                    if (player && shouldFlee)
                    {
                        //Run away
                        agent.SetDestination(RandomNavSphere(transform.position, Random.Range(1, 2.4f)));
                        state = AIState.Running;
                        ChangeAnimationState(RUN);
                    }
                    else
                    {
                        //No enemies nearby, start eating
                        actionTimer = Random.Range(14, 22);

                        state = AIState.Eating;
                        ChangeAnimationState(EAT);

                        //Keep last 5 Idle positions for future reference
                        previousIdlePoints.Add(transform.position);

                        if (previousIdlePoints.Count > 5)
                        {
                            previousIdlePoints.RemoveAt(0);
                        }
                    }
                }
            }

            else if (state == AIState.Walking)
            {
                //Set NavMesh Agent Speed
                agent.speed = walkingSpeed;

                // Check if we've reached the destination
                if (DoneReachingDestination())
                {
                    state = AIState.Idle;
                }
            }
            else if (state == AIState.Eating)
            {
                if (switchAction)
                {
                    //Wait for current animation to finish playing
                    if (!animator || animator.GetCurrentAnimatorStateInfo(0).normalizedTime - Mathf.Floor(animator.GetCurrentAnimatorStateInfo(0).normalizedTime) > 0.99f)
                    {
                        //Walk to another random destination
                        agent.destination = RandomNavSphere(transform.position, Random.Range(3, 7));
                        state = AIState.Walking;
                        ChangeAnimationState(WALK);
                    }
                }
            }
            else if (state == AIState.Running)
            {
                //Set NavMesh Agent Speed
                agent.speed = runningSpeed;

                //Run away
                if (player)
                {
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
                        Vector3 runTo = transform.position + ((transform.position - player.position) * multiplier);
                        distance = (transform.position - player.position).sqrMagnitude;

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

                        if (distance < range * range)
                        {
                            agent.SetDestination(runTo);
                        }
                    }

                    //Temporarily switch to Idle if the Agent stopped
                    if (agent.velocity.sqrMagnitude < 0.1f * 0.1f)
                    {
                        ChangeAnimationState(IDLE);
                    }
                    else
                    {
                        ChangeAnimationState(RUN);
                    }
                }
                else
                {
                    //Check if we've reached the destination then stop running
                    if (DoneReachingDestination())
                    {
                        actionTimer = Random.Range(1.4f, 3.4f);
                        state = AIState.Eating;
                        ChangeAnimationState(IDLE);
                    }
                }
            }

            switchAction = false;
        } else if (playerBehaviours.dialogueIsActive)
        {
            state = AIState.Idle;
            ChangeAnimationState(IDLE);
            //agent.transform.LookAt(player.transform);
        }
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


    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);

        return navHit.position;
    }

    private string currentState;

    void OnTriggerEnter(Collider other)
    {
        //Make sure the Player instance has a tag "Player"
        if (!other.CompareTag("Player"))
            return;

        player = other.transform;

        actionTimer = Random.Range(0.24f, 0.8f);
        state = AIState.Idle;
        ChangeAnimationState(IDLE);
    }

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
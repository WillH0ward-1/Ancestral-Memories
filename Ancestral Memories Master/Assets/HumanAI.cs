//You are free to use this script in Free or Commercial projects
//sharpcoderblog.com @2019
//https://sharpcoderblog.com/blog/unity-3d-deer-ai-tutorial

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using FIMSpace.FLook;
using Pathfinding;

public class HumanAI : MonoBehaviour
{

    const string IDLE = "Citizen_Idle";
    const string IDLESAD = "Citizen_IdleSad";
    const string RUN = "Citizen_Run";
    const string WALK = "Citizen_Walk";
    const string WALKSAD = "Citizen_WalkSad";
    const string HARVEST = "Citizen_HarvestTree";
    const string PLANT = "Citizen_PlantTree";
    const string PICKUP = "Citizen_PickUp";
    const string FLEE = "Citizen_RunScared";
    public const string GETUPFRONT = "Citizen_StandUpFromFront";
    public const string GETUPBACK = "Citizen_StandUpFromBack";

    public enum AIState { Idle, Walking, Harvesting, Running, Following, Dialogue }

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

    private FluteControl fluteControl;

    private FollowersManager followManager;

    public MapObjGen mapObjGen;

    private string currentState;

    private PlayerWalk playerWalk;

    public RagdollController ragdollController;

    [SerializeField] private float defaultStoppingDistance = 1f;

    private RichAI aiPath;

    void Start()
    {
        /*
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        agent.autoBraking = false;
        agent.radius = agentRadius;
        */

        aiPath = transform.GetComponentInChildren<RichAI>();
        aiPath.endReachedDistance = defaultStoppingDistance;
        aiPath.acceleration = 10000;

        animator = transform.GetComponentInChildren<Animator>();

        interactable = transform.GetComponent<Interactable>();
        fluteControl = player.GetComponentInChildren<FluteControl>();
        followManager = player.GetComponentInChildren<FollowersManager>();

        ragdollController = transform.GetComponentInChildren<RagdollController>();

        treeLayer = LayerMask.GetMask("Trees");

        playerWalk = player.GetComponentInChildren<PlayerWalk>();

        ChangeState(AIState.Idle);

    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        UpdateRange();

        if (!ragdollController.isRagdollActive)
        {
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

    }


    [SerializeField] float time;
    [SerializeField] float minActionBuffer = 3;
    [SerializeField] float maxActionBuffer = 10;

    private bool overriden = false;

    public IEnumerator GetUp(bool isFacingUp)
    {
        if (!aiPath.enabled)
        {
            aiPath.enabled = true;
        }
        if (!animator.enabled)
        {
            animator.enabled = true;
        }
        if (!lookAnimator.enabled)
        {
            lookAnimator.enabled = true;
        }

        if (isFacingUp)
        {
            ChangeAnimationState(GETUPFRONT);
        }
        else
        {
            ChangeAnimationState(GETUPBACK);
        }

        float time = 0;
        float duration = GetAnimLength();

        while (time <= duration)
        {
            time += Time.deltaTime / duration;

            yield return null;
        }

        ChangeState(AIState.Idle);
        StartCoroutine(ragdollController.TriggerRagdollTest());
        yield break;

    }

    private IEnumerator Idle()
    {
        ChangeAnimationState(IDLE);

        //agent.speed = 0f;
        //agent.ResetPath();

        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

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

    private IEnumerator Walk(Vector3 destination)
    {
        ChangeAnimationState(WALK);

        //agent.speed = walkingSpeed;
        //agent.SetDestination(destination);
        aiPath.canMove = true;
        aiPath.maxSpeed = walkingSpeed;

        aiPath.destination = destination;

        behaviourActive = true;

        while (behaviourActive)
        {

            if (playerBehaviours.isPsychdelicMode && inRange && player.isBlessed || fluteControl.fluteActive)
            {
                ChangeState(AIState.Following);
            }
            else if (aiPath.reachedDestination)
            {
                ChangeState(AIState.Idle);
            }

            yield return null;
        }

        yield break;
    }

    [SerializeField] float minRandWalkDistance = 0;
    [SerializeField] float maxRandWalkDistance = 15;
    [SerializeField] float randWalkDistance;

    public float followDistance = 15f;

    public void ChangeState(AIState newState)
    {
        if (!ragdollController.isRagdollActive)
        {
            StopAllCoroutines();
            behaviourActive = false;
            currentAIState = newState;
            state = currentAIState;

            if (state == AIState.Following)
            {
                followManager.AddFollower(transform.gameObject);
                //agent.stoppingDistance = 15f;
                aiPath.endReachedDistance = followDistance;
            }
            else
            {
                //agent.stoppingDistance = 0f;
                aiPath.endReachedDistance = defaultStoppingDistance;
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
                case AIState.Harvesting:
                    StartCoroutine(WalkToward(treeLayer));
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
    }

    private LayerMask treeLayer;

    public float inRangeThreshold = 5f;
    public float sphereCastRadius = 10f;

    [SerializeField] private List<Transform> hitTrees;

    private bool isSphereCastVisualizerEnabled = false;

    private IEnumerator WalkToward(LayerMask layer)
    {
        if (isSphereCastVisualizerEnabled)
        {
            StartCoroutine(SphereCastVisualizer());
        }

        Debug.Log("Looking for tree...");

        bool cast = true;

        Transform closestHit = null;

        if (layer == treeLayer)
        {
            inRangeThreshold = playerWalk.treeHarvestStopDistance;
        } else
        {
            inRangeThreshold = playerWalk.minimumStopDistance;
        }

        GameObject harvestObject = null;

        while (cast)
        {
            hitTrees.Clear(); // Clear the list before each sphere cast

            if (Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out RaycastHit hit, Mathf.Infinity, layer))
            {
                Transform hitTransform = hit.transform;
                hitTrees.Add(hitTransform);
            }

            if (hitTrees.Count == 0)
            {
                // No trees were hit by the sphere cast.
                yield return null;
                continue;
            }

            foreach (Transform hitTransform in hitTrees)
            {
                GameObject hitObject = hitTransform.gameObject;
                TreeDeathManager treeManager = hitObject.GetComponentInChildren<TreeDeathManager>();
                ScaleControl treeGrowControl = hitObject.GetComponentInChildren<ScaleControl>();

                if (treeGrowControl != null && !treeGrowControl.isFullyGrown)
                {
                    // The tree is not fully grown, look for another tree.
                    continue;
                }

                if (treeManager != null && treeManager.treeDead)
                {
                    // The tree is dead, look for another tree.
                    continue;
                }

                if (closestHit == null || Vector3.Distance(transform.position, hitTransform.position) < Vector3.Distance(transform.position, closestHit.position))
                {
                    closestHit = hitTransform;
                }
            }

            if (closestHit == null)
            {
                // No trees are alive.
                yield return null;
                continue;
            }

            harvestObject = closestHit.gameObject;
            Debug.Log("First " + closestHit.name);
            yield return null;
        }


        ChangeAnimationState(WALK);

        //agent.speed = walkingSpeed;
        //agent.SetDestination(closestHit.position);

        aiPath.canMove = true;
        aiPath.maxSpeed = walkingSpeed;

        Vector3 destination = closestHit.position;

        aiPath.destination = destination;

        behaviourActive = true;

        float distanceToDestination = Vector3.Distance(transform.position, destination);

        while (behaviourActive && distanceToDestination > inRangeThreshold)
        {
            distanceToDestination = Vector3.Distance(transform.position, destination);

            yield return null;
        }

        StartCoroutine(Harvest(harvestObject));

        yield break;

    }

    public float sphereCastVisualizerDuration = 2;

    private IEnumerator SphereCastVisualizer()
    {
        // Create a sphere mesh for the visualization.
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = new Vector3(sphereCastRadius * 2f, sphereCastRadius * 2f, sphereCastRadius * 2f);
        sphere.GetComponent<MeshRenderer>().material.color = Color.yellow;
        sphere.SetActive(false);

        // Enable the spherecast visualization.
        sphere.SetActive(true);

        // Wait for the specified duration.
        yield return new WaitForSeconds(sphereCastVisualizerDuration);

        // Disable the spherecast visualization.
        sphere.SetActive(false);

        // Destroy the sphere mesh.
        Destroy(sphere);
    }

    private IEnumerator Harvest(GameObject tree)
    {
        //agent.speed = 0f;
        //agent.ResetPath();

        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        ChangeAnimationState(HARVEST);

        behaviourActive = true;

        yield break;
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
        //ChangeAnimationState(EAT);

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

        //agent.speed = runningSpeed;
        aiPath.canMove = true;
        aiPath.maxSpeed = runningSpeed;

        behaviourActive = true;

        while (behaviourActive)
        {
            //Set NavMesh Agent Speed
            //agent.speed = runningSpeed;

            if (reverseFlee)
            {
                if (aiPath.reachedDestination && timeStuck < 0)
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

                /*
                NavMeshHit hit;
                if (NavMesh.FindClosestEdge(transform.position, out hit, NavMesh.AllAreas))
                {
                    closestEdge = hit.position;
                    distanceToEdge = hit.distance;
                    //Debug.DrawLine(transform.position, closestEdge, Color.red);
                }
                */

                GraphNode nearestNode = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
                if (nearestNode != null)
                {
                    Vector3 closestEdge = nearestNode.ClosestPointOnNode(transform.position);
                    distanceToEdge = Vector3.Distance(transform.position, closestEdge);
                    // Debug.DrawLine(transform.position, closestEdge, Color.red);
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
                    //agent.SetDestination(runTo);
                    aiPath.destination = runTo;
                }
            }
                

            if (agent.velocity.sqrMagnitude < 0.1f * 0.1f)
            {
                ChangeState(AIState.Idle);
            }

            else
            {
                if (aiPath.reachedDestination)
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

        //agent.speed = walkingSpeed;
        aiPath.canMove = true;
        aiPath.maxSpeed = walkingSpeed;

        aiPath.destination = player.transform.position;

        while (behaviourActive)
        {
            if (!inRange)
            {
                ChangeAnimationState(RUN);
                aiPath.maxSpeed = runningSpeed;
            }
            else
            {
                ChangeAnimationState(WALK);
                aiPath.maxSpeed = walkingSpeed;
            }
            //agent.SetDestination(player.transform.position);

            if (aiPath.reachedDestination || inRange)
            {
                ChangeState(AIState.Idle);
            }

            yield return null;
        }

        yield break;
    }

    bool DoneReachingDestination()
    {
        /*
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
        */

        if (!aiPath.pathPending)
        {
            if (aiPath.remainingDistance <= aiPath.endReachedDistance)
            {
                if (!aiPath.hasPath || aiPath.velocity.sqrMagnitude == 0f)
                {
                    // Done reaching the destination
                    return true;
                }
            }
        }

        return false;
    }

    public IEnumerator StopAllBehaviours()
    {
        //agent.enabled = false;
        aiPath.enabled = false;

        animator.enabled = false;
        lookAnimator.enabled = false;
        StopAllCoroutines();

        yield break;
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

    /*

    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;

        randomDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randomDirection, out navHit, distance, NavMesh.AllAreas);

        return navHit.position;
    }
    */

    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = Random.insideUnitSphere * distance;
        randomDirection += origin;

        NNInfo nearestNode = AstarPath.active.GetNearest(randomDirection);
        if (nearestNode.node != null && nearestNode.node.Walkable)
        {
            return (Vector3)nearestNode.position;
        }
        else
        {
            return origin;
        }
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

    public float animLength;

    private float GetAnimLength()
    {
        animLength = player.activeAnimator.GetCurrentAnimatorStateInfo(0).length;
        return animLength;
    }

    private IEnumerator WaitForAnimationCompletion(Animator animator)
    {
        int layerIndex = 0;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        int startStateNameHash = stateInfo.fullPathHash;

        // Wait for one frame to ensure the animation has started
        yield return null;

        // Wait until the animation has looped back or the animator has transitioned to a new state
        while (true)
        {
            stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
            if (stateInfo.fullPathHash != startStateNameHash || stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }
            yield return null;
        }
    }

}
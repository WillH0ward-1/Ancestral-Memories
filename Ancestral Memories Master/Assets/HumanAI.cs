//You are free to use this script in Free or Commercial projects
//sharpcoderblog.com @2019
//https://sharpcoderblog.com/blog/unity-3d-deer-ai-tutorial

using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;
using FIMSpace.FLook;
using Pathfinding;
using ProceduralModeling;
using System;

public class HumanAI : MonoBehaviour
{

    public enum AIState { Idle, Walking, Harvest, Running, Following, Dialogue, Conversate, HuntFood, Eat, HuntMeat, Attack }

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

    [SerializeField] float distanceToTarget;

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

    public bool inRange = false;

    private AICharacterStats stats;

    public enum EvolutionState { Neanderthal, MidSapien, Sapien }
    public EvolutionState currentEvolutionState;

    private void Awake()
    {
        formationController = FindObjectOfType<FormationController>();
        player = FindObjectOfType<Player>();
        playerBehaviours = player.GetComponentInChildren<CharacterBehaviours>();

    }

    public Dictionary<AIState, Action<GameObject>> stateActions = new Dictionary<AIState, Action<GameObject>>();

    void Start()
    {

        aiPath = transform.GetComponentInChildren<RichAI>();
        aiPath.endReachedDistance = defaultStoppingDistance;
        aiPath.acceleration = 10000;
        animator = transform.GetComponentInChildren<Animator>();
        aiBehaviours = transform.GetComponentInChildren<AIBehaviours>();
        interactable = transform.GetComponent<Interactable>();
        fluteControl = player.GetComponentInChildren<FluteControl>();
        followManager = player.GetComponentInChildren<FollowersManager>();
        ragdollController = transform.GetComponentInChildren<RagdollController>();
        playerWalk = player.GetComponentInChildren<PlayerWalk>();
        stats = transform.GetComponentInChildren<AICharacterStats>();

        stateActions[AIState.Harvest] = (target) => StartCoroutine(Harvest(target));
        stateActions[AIState.Eat] = (target) => StartCoroutine(Eat(target));
        stateActions[AIState.Attack] = (target) => StartCoroutine(Attack(target));

        ChangeState(AIState.Idle);

    }

    void Update()
    {
        if (stats.health <= 0 && !stats.isDead)
        {
            Die();
        }

        if (!stats.isDead)
        {
            switch (stats.evolution)
            {
                case var e when (e >= 0 && e < 33):
                    currentEvolutionState = EvolutionState.Neanderthal;
                    break;

                case var e when (e >= 33 && e < 66):
                    currentEvolutionState = EvolutionState.MidSapien;
                    break;

                case var e when (e >= 66):
                    currentEvolutionState = EvolutionState.Sapien;
                    break;

                default:
                    break;
            }
        }
    }

    void LookAt()
    {
        if (!ragdollController.isRagdollActive)
        {
            if (inRange || fluteControl.fluteActive)
            {
                lookAnimator.enabled = true;
            }
            else
            {
                lookAnimator.enabled = false;
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
            ChangeAnimationState(HumanControllerAnimations.OnBack_GetUp01);
        }
        else
        {
            ChangeAnimationState(HumanControllerAnimations.OnFront_ToStand_Dazed01);
        }

        float time = 0;
        float duration = GetAnimLength();

        while (time <= duration)
        {
            time += Time.deltaTime / duration;

            yield return null;
        }

        ChangeState(AIState.Idle);
        yield break;

    }

    public bool isDead = false;

    private void Die()
    {
        if (!stats.isDead)
        {
            stats.isDead = true;
            StartCoroutine(ragdollController.TriggerRagdoll());
        }
    }

    private IEnumerator Idle()
    {

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

        time = UnityEngine.Random.Range(minActionBuffer, maxActionBuffer);

        behaviourActive = true;

        while (behaviourActive)
        {
            if (currentEvolutionState == EvolutionState.Neanderthal)
            {
                ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);
            }
            else if (currentEvolutionState == EvolutionState.MidSapien)
            {
                ChangeAnimationState(HumanControllerAnimations.Idle_MidSapien01);
            }
            else if (currentEvolutionState == EvolutionState.Sapien)
            {
                ChangeAnimationState(HumanControllerAnimations.Idle_Sapien01);
            }

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
            int randNumber = UnityEngine.Random.Range(minChance, maxChance);

            if (time <= 0)
            {
                {
                    ChangeState(AIState.Idle);
                }
            }

            yield return null;
        }

        yield break;
    }

    public List<GameObject> NearbyAI;
    public bool isTalking = false;
    public bool isListening = false;
    private FormationController formationController;

    private IEnumerator EnterConversation()
    {
        if (currentEvolutionState == EvolutionState.Neanderthal)
        {
            ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);
        }
        else if (currentEvolutionState == EvolutionState.MidSapien)
        {
            ChangeAnimationState(HumanControllerAnimations.Idle_MidSapien01);
        }
        else if (currentEvolutionState == EvolutionState.Sapien)
        {
            ChangeAnimationState(HumanControllerAnimations.Idle_Sapien01);
        }

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

        time = UnityEngine.Random.Range(minActionBuffer, maxActionBuffer);

        behaviourActive = true;

        // Spherecast variables
        float spherecastRadius = 25f;

        LayerMask humanLayer = LayerMask.GetMask("Human");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, spherecastRadius, humanLayer);
        List<GameObject> nearbyAI = new List<GameObject>();

        // Add the detected Human objects to the nearbyAI list
        foreach (var collider in hitColliders)
        {
            GameObject nearbyObject = collider.gameObject;
            nearbyAI.Add(nearbyObject);
        }

        // Update the public defined list (NearbyAI) with nearbyAI
        NearbyAI = nearbyAI;

        yield break;
    }


    private IEnumerator Walk(Vector3 destination)
    {

        aiPath.canMove = true;
        aiPath.maxSpeed = walkingSpeed;

        aiPath.destination = destination;

        behaviourActive = true;

        while (behaviourActive)
        {
            if (currentEvolutionState == EvolutionState.Neanderthal)
            {
                ChangeAnimationState(HumanControllerAnimations.Walk_Neanderthal01);
            }
            else if (currentEvolutionState == EvolutionState.MidSapien)
            {
                ChangeAnimationState(HumanControllerAnimations.Walk_MidSapien01);
            }
            else if (currentEvolutionState == EvolutionState.Sapien)
            {
                ChangeAnimationState(HumanControllerAnimations.Walk_Sapien);
            }

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
    public FormationManager.FormationType Formation { get; set; }

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

            randWalkDistance = UnityEngine.Random.Range(minRandWalkDistance, maxRandWalkDistance);

            switch (state)
            {
                case AIState.Idle:
                    StartCoroutine(Idle());
                    break;
                case AIState.Conversate:
                    StartCoroutine(EnterConversation());
                    break;
                case AIState.Harvest:
                    GameObject tree = GetClosest(mapObjGen.treeList, aiBehaviours.ValidateTree);
                    if (tree != null)
                    {
                        // For example, make the agent walk towards a tree in a circle formation of size 5
                        StartCoroutine(WalkTowards(tree, formationController, FormationManager.FormationType.Circle, 5f, stateActions[AIState.Harvest], treeHarvestDistance));
                    }
                    else
                    {
                        Debug.Log("No valid tree found. Can't start WalkToward.");
                    }
                    break;

                case AIState.Walking:
                    StartCoroutine(Walk(RandomNavSphere(transform.position, randWalkDistance)));
                    break;
                case AIState.Running:
                    StartCoroutine(Run());
                    break;
                case AIState.Following:
                    StartCoroutine(Follow(player.transform));
                    break;
                case AIState.Dialogue:
                    StartCoroutine(DialogueActive());
                    break;
                case AIState.HuntFood:
                    GameObject fruit = GetClosest(mapObjGen.foodSourcesList, aiBehaviours.ValidateFruit);
                    StartCoroutine(WalkTowards(fruit, formationController, FormationManager.FormationType.None, 0f, stateActions[AIState.Eat], treeHarvestDistance));
                    break;
                case AIState.HuntMeat:
                    GameObject animal = GetClosest(mapObjGen.huntableAnimalsList, aiBehaviours.ValidateAnimal);
                    StartCoroutine(WalkTowards(animal, formationController, FormationManager.FormationType.Circle, 5f, stateActions[AIState.Attack], attackStoppingDistance));
                    break;
                default:
                    break;
            }

//            Debug.Log(currentAIState);
        }
    }

    private GameObject GetClosest(List<GameObject> objects, AIBehaviours.ValidateObject validate)
    {
        GameObject closestGameObject = null;
        float smallestDistance = float.MaxValue;

        foreach (GameObject obj in objects)
        {
            if (!validate(obj))
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, obj.transform.position);

            Debug.Log("Valid object " + obj.name + " at distance: " + distance);

            if (distance < smallestDistance)
            {
                Debug.Log("New closest object found: " + obj.name);
                smallestDistance = distance;
                closestGameObject = obj;
            }
        }

        if (closestGameObject == null)
        {
            Debug.Log("No valid objects found.");
        }

        return closestGameObject;
    }

    public float inRangeThreshold = 5f;
    public float sphereCastRadius = 10f;
    public float treeHarvestDistance = 15f;
    public float attackStoppingDistance = 5f;

    [SerializeField] private List<Transform> hitObjects;

    private bool isSphereCastVisualizerEnabled = false;

    private AIBehaviours aiBehaviours;

    private IEnumerator WalkTowards(GameObject target, FormationController formationController, FormationManager.FormationType formationType, float formationSize, Action<GameObject> action, float stoppingDistance)
    {
        Debug.Log("Walking towards: " + target + "!");

        aiPath.endReachedDistance = stoppingDistance;


        aiPath.canMove = true;

        Vector3 destination = target.transform.position;

        if (formationController != null && formationType != FormationManager.FormationType.None)
        {
            // First we need to get the group this agent belongs to
            FormationController.Group group = formationController.GetGroupForAgent(this);
            if (group != null)
            {
                // Get the positions of the formation
                List<Vector3> formationPositions = FormationManager.GetPositions(target.transform.position, formationType, group.agents.Count, formationSize);
                // Get this agent's index in the formation
                int index = group.agents.IndexOf(this);
                // Set the destination to the position corresponding to this agent in the formation
                destination = formationPositions[index];
            }
        }

        aiPath.destination = destination;

        behaviourActive = true;

        while (behaviourActive)
        {
            UpdateRange(target.transform);

            if (!aiPath.reachedDestination)
            {
                if (inRunningRange)
                {
                    if (currentEvolutionState == EvolutionState.Neanderthal)
                    {
                        ChangeAnimationState(HumanControllerAnimations.Run_Neanderthal_Jog01);
                    }
                    else if (currentEvolutionState == EvolutionState.MidSapien)
                    {
                        ChangeAnimationState(HumanControllerAnimations.Run_MidSapien_Jog);
                    }
                    else if (currentEvolutionState == EvolutionState.Sapien)
                    {
                        ChangeAnimationState(HumanControllerAnimations.Run_Sapien_Jog);
                    }

                    aiPath.maxSpeed = runningSpeed;
                }
                else if (inWalkingRange)
                {
                    if (currentEvolutionState == EvolutionState.Neanderthal)
                    {
                        ChangeAnimationState(HumanControllerAnimations.Walk_Neanderthal01);
                    }
                    else if (currentEvolutionState == EvolutionState.MidSapien)
                    {
                        ChangeAnimationState(HumanControllerAnimations.Walk_MidSapien01);
                    }
                    else if (currentEvolutionState == EvolutionState.Sapien)
                    {
                        ChangeAnimationState(HumanControllerAnimations.Walk_Sapien);
                    }

                    aiPath.maxSpeed = walkingSpeed;
                }
            }

            if (aiPath.reachedDestination)
            {
                behaviourActive = false;
            }

            yield return null;
        }

        action(target);

        if (formationController != null)
        {
            formationController.UnregisterAgent(this);
        }

        yield break;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        aiPath.destination = targetPosition;
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

    [SerializeField] float minAnimationSpeed = 0.9f;
    [SerializeField] float maxAnimationSpeed = 2.2f;

    TreeInteractions treeInteract;

    private IEnumerator Harvest(GameObject target)
    {
        PTGrowing ptGrow = target.GetComponentInChildren<PTGrowing>();
        treeInteract = target.GetComponentInChildren<TreeInteractions>();

        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        behaviourActive = true;
        StartCoroutine(HarvestAnimation(target.transform));

        while (behaviourActive)
        {
            if (ptGrow.isDead || !ptGrow.isFullyGrown)
            {
                ChangeState(AIState.Harvest);
                yield break;
            }

            yield return null;
        }

        yield break;
    }

    public void HitTree()
    {
        if (treeInteract != null) {
            treeInteract.TreeShake();
        }
    }

    public float minInterval = 0.5f; // Minimum interval in seconds
    public float maxInterval = 2.0f; // Maximum interval in seconds

    private float GetRandomAnimationSpeed()
    {
        return UnityEngine.Random.Range(minAnimationSpeed, maxAnimationSpeed);
    }

    private float GetRandomInterval()
    {
        return UnityEngine.Random.Range(minInterval, maxInterval);
    }

    private IEnumerator HarvestAnimation(Transform target)
    {
        behaviourActive = true;

        while (behaviourActive)
        {
            transform.LookAt(target);

            StartCoroutine(ChangeSpeedOverTime(target));

            if (currentEvolutionState == EvolutionState.Neanderthal)
            {
                ChangeAnimationState(HumanControllerAnimations.Attack_Neanderthal_Punch01);
            }
            else if (currentEvolutionState == EvolutionState.MidSapien)
            {
                ChangeAnimationState(HumanControllerAnimations.Action_Standing_HarvestTree);
            }
            else if (currentEvolutionState == EvolutionState.Sapien)
            {
                ChangeAnimationState(HumanControllerAnimations.Action_Standing_HarvestTree);
            }

            yield return null;
        }

        yield break;
    }

    List<string> attackAnimations = new List<string> { HumanControllerAnimations.Attack_Slash01, HumanControllerAnimations.Attack_Slash02, HumanControllerAnimations.Attack_Stab01, HumanControllerAnimations.Attack_Stab02, HumanControllerAnimations.Attack_Stab03, };

    private IEnumerator PerformRandomActionOverTime(Transform target, List<string> animations, AICharacterStats stats)
    {
        float randomAnimationSpeed = GetRandomAnimationSpeed();
        float randomInterval = GetRandomInterval();

        string randomAnimationState = animations[UnityEngine.Random.Range(0, animations.Count)];

        ChangeAnimationState(randomAnimationState);
        StartCoroutine(SmoothlyChangeAnimationSpeed(randomAnimationSpeed, randomInterval));

        // Adjust the duration of the animation based on the speed it's being played at
        float duration = GetAnimLength() / randomAnimationSpeed;

        float time = 0;
        while (time <= duration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        animator.speed = 1.0f; // Reset animation speed to normal when finished
    }

    private AICharacterStats attackTargetStats;

    public void DealDamage()
    {
        attackTargetStats.TakeDamage(attackMultiplier);
    }

    private IEnumerator ChangeSpeedOverTime(Transform target)
    {
        float randomAnimationSpeed = GetRandomAnimationSpeed();
        float randomInterval = GetRandomInterval();
        StartCoroutine(SmoothlyChangeAnimationSpeed(randomAnimationSpeed, randomInterval));

        yield return new WaitForSeconds(randomInterval);

        animator.speed = 1.0f; // Reset animation speed to normal when finished
    }

    [SerializeField] private float attackMultiplier = 0.1f;

    private IEnumerator Attack(GameObject target)
    {
        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        behaviourActive = true;
        AICharacterStats animalStats = target.GetComponentInChildren<AICharacterStats>();
        attackTargetStats = animalStats;

        while (behaviourActive)
        {
            transform.LookAt(target.transform);

            yield return StartCoroutine(PerformRandomActionOverTime(target.transform, attackAnimations, animalStats));

            if (animalStats.isDead)
            {
                ChangeState(AIState.HuntMeat);
                yield break;
            }

            yield return null;
        }

        yield break;
    }


    private IEnumerator SmoothlyChangeAnimationSpeed(float targetSpeed, float duration)
    {
        float startSpeed = animator.speed;
        float timeElapsed = 0;

        while (timeElapsed < duration)
        {
            animator.speed = Mathf.Lerp(startSpeed, targetSpeed, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        animator.speed = targetSpeed;
    }

    private IEnumerator PickUpEat(GameObject target)
    {
        // Use target here as needed

        //agent.speed = 0f;
        //agent.ResetPath();

        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        ChangeAnimationState(HumanControllerAnimations.Action_Item_PickUp);

        behaviourActive = true;

        yield break;
    }


    private IEnumerator DialogueActive()
    {
        behaviourActive = true;

        while (behaviourActive)
        {
            ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);

            //agent.transform.LookAt(player.transform);

            yield return null;
        }

        yield break;
    }

    private IEnumerator Eat(GameObject food)
    {
        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        float time = 0;
        float duration = 0;

        ChangeAnimationState(HumanControllerAnimations.Action_Item_PickUp);

        time = 0;
        duration = GetAnimLength();

        while (time <= duration)
        {
            time += Time.deltaTime / duration;

            yield return null;
        }

        ChangeAnimationState(HumanControllerAnimations.Run_Neanderthal_Jog02);

        time = 0;
        duration = GetAnimLength();

        while (time <= duration)
        {
            time += Time.deltaTime / duration;

            yield return null;
        }

        yield break;
    }

    float distance;


    private IEnumerator Run()
    {
        ChangeAnimationState(HumanControllerAnimations.Run_Neanderthal_Jog02);
        aiPath.canMove = true;
        aiPath.maxSpeed = runningSpeed;

        behaviourActive = true;

        while (behaviourActive)
        {
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
                GraphNode nearestNode = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;

                if (nearestNode != null)
                {
                    Vector3 closestEdge = nearestNode.ClosestPointOnNode(transform.position);
                    distanceToEdge = Vector3.Distance(transform.position, closestEdge);
                }

                if (distanceToEdge < 1f)
                {
                    if (timeStuck > 1.5f)
                    {
                        if (previousIdlePoints.Count > 0)
                        {
                            runTo = previousIdlePoints[UnityEngine.Random.Range(0, previousIdlePoints.Count - 1)];
                            reverseFlee = true;
                        }
                    }
                    else
                    {
                        timeStuck += Time.deltaTime;
                    }
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


    private IEnumerator Follow(Transform target)
    {
        behaviourActive = true;
        aiPath.canMove = true;

        while (behaviourActive)
        {
            UpdateRange(target);

            aiPath.destination = player.transform.position;

            if (!inRange) {
                if (inRunningRange)
                {
                    ChangeAnimationState(HumanControllerAnimations.Run_Neanderthal_Jog01);
                    aiPath.maxSpeed = runningSpeed;
                }
                else if (inWalkingRange)
                {
                    ChangeAnimationState(HumanControllerAnimations.Walk_Neanderthal02);
                    aiPath.maxSpeed = walkingSpeed;
                }
            }

            if (aiPath.reachedDestination || inRange)
            {
                ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);
                ChangeState(AIState.Idle);
            }

            yield return null;
        }

        yield break;
    }

    
    public float walkingRange = 50f; // Set to desired constant value
    public float minimumRange = 20f;
    public bool inWalkingRange;
    public bool inRunningRange;
    public Transform target;

    private void UpdateRange(Transform target)
    {
        // Calculate distance to the player
        distanceToTarget = Vector3.Distance(transform.position, target.transform.position);

        if (distanceToTarget <= minimumRange)
        {
            inWalkingRange = false;
            inRunningRange = false;
            inRange = true;
        }
        // If distance is less than or equal to walking range, AI is in walking range
        else if (distanceToTarget <= walkingRange)
        {
            inWalkingRange = true;
            inRunningRange = false;
            inRange = false;
        }
        // If distance is more than walking range, AI is in running range
        else
        {
            inWalkingRange = false;
            inRunningRange = true;
            inRange = false;
        }
    }

    public IEnumerator StopAllBehaviours()
    {
        aiPath.enabled = false;
        animator.enabled = false;
        lookAnimator.enabled = false;
        StopAllCoroutines();

        yield break;
    }

    Vector3 RandomNavSphere(Vector3 origin, float distance)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;

        NNInfo nearestNode = AstarPath.active.GetNearest(randomDirection);
        if (nearestNode.node != null && nearestNode.node.Walkable)
        {
            return nearestNode.position;
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
        animator.SetBool("Mirror", UnityEngine.Random.value > 0.5f);

        currentState = newState;
    }


    public virtual void AdjustAnimationSpeed(float newSpeed)
    {
        animator.speed = newSpeed;
    }

    public float animLength;

    private float GetAnimLength()
    {
        animLength = animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
        return animLength;
    }

    private IEnumerator WaitForAnimationCompletion(Animator animator)
    {
        int layerIndex = 0;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        int startStateNameHash = stateInfo.fullPathHash;

        yield return null;

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
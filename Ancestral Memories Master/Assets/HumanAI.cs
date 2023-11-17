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

    public enum AIState { Idle, Walking, Harvest, Running, Following, Dialogue, Conversate, HuntFood, Eat, HuntMeat, Attack, Wander }

    [SerializeField] private AIState state = AIState.Idle;

    [SerializeField] private AIState currentAIState;
    public bool behaviourIsActive = false;

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

    public bool isElectrocuted;

    public ResourcesManager resources;

    private void Awake()
    {

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

        AddToPopulation();

        ChangeState(AIState.Idle);

    }

    void Update()
    {
        if (stats.health <= 0 && !stats.isDead)
        {
            KillToRagdoll();
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

    private void KillToRagdoll()
    {
        if (!stats.isDead)
        {
            stats.isDead = true;
            RemoveFromPopulation();
            ragdollController.TriggerRagdoll();
        }
    }

    public void RemoveFromPopulation()
    {
        resources.RemoveResourceObject("Population", transform.gameObject);
    }

    public void AddToPopulation()
    {
        resources.AddResourceObject("Population", transform.gameObject);
    }

    [SerializeField] private float idleWalkRadius = 1000f;
    [SerializeField] private float minWalkDistance = 500f; // Adjusted for better logic
    [SerializeField] private float maxWalkDistance = 1000f; // Adjusted for better logic

    private IEnumerator Idle()
    {
        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        previousIdlePoints.Add(transform.position);

        if (previousIdlePoints.Count > 5)
        {
            previousIdlePoints.RemoveAt(0);
        }

        time = UnityEngine.Random.Range(minActionBuffer, maxActionBuffer);

        behaviourIsActive = true;

        while (behaviourIsActive)
        {
            SetIdleAnimation(currentEvolutionState); // Update the idle animation based on state

            while (time >= 0)
            {
                if (!inRange && (playerBehaviours.isPsychdelicMode && inRange && player.isBlessed || fluteControl.fluteActive))
                {
                    ChangeState(AIState.Following);
                }

                time -= Time.deltaTime;
                yield return null;
            }

            if (time <= 0)
            {
                if (ShouldWander())
                {
                    ChangeState(AIState.Wander);
                    yield break;
                } else
                {
                    ChangeState(AIState.Idle); // If not wandering, stay idle
                }
            }

            yield return null;
        }
    }

    public float timeSinceLastShock = 0;
    private Coroutine shockTimerCoroutine;
    public float shockCooldownTime = 30f; // You can adjust this value as needed

    public IEnumerator DeathElectrocution()
    {
        if (shockTimerCoroutine != null)
        {
            StopCoroutine(shockTimerCoroutine);
        }

        isElectrocuted = true;

        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        behaviourIsActive = true;

        // Random chance
        float chance = UnityEngine.Random.value; // Random value between 0 and 1
        float threshold = 0.5f; // Adjust threshold as needed, 0.5 implies 50% chance for each

        if (chance <= threshold)
        {
            // Play Electrocution animation
            ChangeAnimationState(HumanControllerAnimations.Death_Standing_Electrocution);

            // Wait for the length of the Electrocution animation
            Animator animator = GetComponent<Animator>(); // Assuming an Animator component is attached
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // Layer 0
            yield return new WaitForSeconds(stateInfo.length);

            // Now get up
            StartCoroutine(GetUp(true));
        }
        else
        {
            StartCoroutine(DeathKnockout());
        }

        isElectrocuted = false;
    }

    private IEnumerator DeathKnockout()
    {
        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        time = UnityEngine.Random.Range(minActionBuffer, maxActionBuffer);

        behaviourIsActive = true;

        while (behaviourIsActive)
        {
            KillToRagdoll();

            yield return null;
        }
    }

    private IEnumerator Wander()
    {
        
        aiPath.canMove = true;
        aiPath.maxSpeed = walkingSpeed;

        Vector3? newWanderPoint = null;

        // Attempt to find a valid wander point.
        while (!newWanderPoint.HasValue)
        {
            newWanderPoint = FindRandomPointWithinRadius(idleWalkRadius, minWalkDistance, maxWalkDistance);

            // Optional: A small delay to prevent the loop from running too fast
            // and overwhelming the processor in case valid points are rare.
            yield return new WaitForSeconds(0.1f);
        }

        // Set the destination once a valid point is found.
        aiPath.destination = newWanderPoint.Value;

        behaviourIsActive = true;

        while (behaviourIsActive)
        {
            SetWalkingAnimation(currentEvolutionState); // Update the walking animation based on state

            if (!inRange && (playerBehaviours.isPsychdelicMode && inRange && player.isBlessed || fluteControl.fluteActive))
            {
                ChangeState(AIState.Following);
            }

            else if (aiPath.reachedDestination)
            {
                ChangeState(AIState.Idle);
            }

            yield return null;
        }
    }

    private bool ShouldWander()
    {
        int chanceToWander = 1;
        return UnityEngine.Random.value <= chanceToWander;
    }

    private IEnumerator Walk(Vector3 destination)
    {
        aiPath.maxSpeed = walkingSpeed;
        aiPath.destination = destination;
        aiPath.canMove = true;

        behaviourIsActive = true;

        while (behaviourIsActive)
        {
            SetWalkingAnimation(currentEvolutionState); // Setting walking animation

            if (!aiPath.pathPending && (aiPath.reachedEndOfPath || !aiPath.hasPath))
            {
                behaviourIsActive = false;
            }

            yield return null;
        }

        ChangeState(AIState.Idle);
    }


    private Vector3? FindRandomPointWithinRadius(float radius, float minDistance, float maxDistance)
    {
        Vector2 randomDirection2D = UnityEngine.Random.insideUnitCircle * radius;
        Vector3 randomPoint = new Vector3(randomDirection2D.x, 0, randomDirection2D.y) + transform.position;
        Debug.DrawLine(transform.position, randomPoint, Color.red, 5f); // For visualization

        Vector3 elevatedPosition = new Vector3(randomPoint.x, 50f, randomPoint.z);
        Debug.DrawRay(elevatedPosition, Vector3.down * 100f, Color.blue, 5f); // For visualization

        RaycastHit hit;

        int layerMask = LayerMask.GetMask("Ground", "Water");

        if (Physics.Raycast(elevatedPosition, Vector3.down, out hit, Mathf.Infinity, layerMask))
        {
            float distanceToFinalPosition = Vector3.Distance(transform.position, hit.point);
            Debug.Log("Distance to Point: " + distanceToFinalPosition); // Log the distance

            if (distanceToFinalPosition >= minDistance && distanceToFinalPosition <= maxDistance)
            {
                return hit.point;
            }
        }

        return null;
    }



    private void SetWalkingAnimation(EvolutionState state)
    {
        switch (state)
        {
            case EvolutionState.Neanderthal:
                ChangeAnimationState(HumanControllerAnimations.Walk_Neanderthal01);
                break;
            case EvolutionState.MidSapien:
                ChangeAnimationState(HumanControllerAnimations.Walk_MidSapien01);
                break;
            case EvolutionState.Sapien:
                ChangeAnimationState(HumanControllerAnimations.Walk_Sapien);
                break;
        }
    }

    private void SetIdleAnimation(EvolutionState state)
    {
        switch (state)
        {
            case EvolutionState.Neanderthal:
                ChangeAnimationState(HumanControllerAnimations.Idle_Neanderthal);
                break;
            case EvolutionState.MidSapien:
                ChangeAnimationState(HumanControllerAnimations.Idle_MidSapien01);
                break;
            case EvolutionState.Sapien:
                ChangeAnimationState(HumanControllerAnimations.Idle_Sapien01);
                break;
        }
    }




    public List<GameObject> NearbyAI;
    public bool isTalking = false;
    public bool isListening = false;
    public FormationController formationController;

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

        behaviourIsActive = true;

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
            behaviourIsActive = false;
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

            switch (state)
            {
                case AIState.Idle:
                    StartCoroutine(Idle());
                    break;
                case AIState.Wander:
                    StartCoroutine(Wander());
                    break;
                case AIState.Conversate:
                    StartCoroutine(EnterConversation());
                    break;
                case AIState.Harvest:

                    GameObject tree = null;

                    tree = GetClosest(mapObjGen.treeList, aiBehaviours.ValidateTree);

                    if (tree != null)
                    {
                        // For example, make the agent walk towards a tree in a circle formation of size 5
                        target = tree.transform;
                        StartCoroutine(WalkTowards(tree, formationController, FormationManager.FormationType.Circle, 5f, stateActions[AIState.Harvest], treeHarvestDistance));
                    }
                    else
                    {
                        tree = GetClosest(mapObjGen.treeGrowingList, aiBehaviours.ValidateTree);

                        if (tree != null)
                        {
                            target = tree.transform;
                            StartCoroutine(WalkTowards(tree, formationController, FormationManager.FormationType.Circle, 5f, stateActions[AIState.Harvest], treeHarvestDistance));
                        } else
                        {
                            ChangeState(AIState.Idle);
                            Debug.Log("No valid tree found. Can't start WalkToward.");
                        }
                    }

                    break;

                case AIState.Walking:
                    StartCoroutine(Walk(aiPath.destination));
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
                    target = animal.transform;
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

        behaviourIsActive = true;
        PTGrowing ptGrow = null; // Declare outside of loop

        while (behaviourIsActive)
        {
            if (action == stateActions[AIState.Harvest])
            {
                // Only get the component if it's null or hasn't been fetched before
                if (ptGrow == null)
                {
                    ptGrow = target.GetComponentInChildren<PTGrowing>();
                }

                if (ptGrow != null && (ptGrow.isDead || !ptGrow.isFullyGrown))
                {
                    ChangeState(AIState.Harvest);

                    yield break;
                }
            }

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
                behaviourIsActive = false;
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

        behaviourIsActive = true;
        StartCoroutine(HarvestAnimation(target.transform));

        // Initialize hits and threshold for harvesting
        hits = 0;
        someThreshold = UnityEngine.Random.Range(2, 8);

        while (behaviourIsActive)
        {
            if (ptGrow.isDead || !ptGrow.isFullyGrown)
            {
                ChangeState(AIState.Harvest);
                yield break;
            }

            yield return null;
        }

        hits = 0; // Reset hits after harvesting is done or interrupted
    }

    [SerializeField] private float attackMultiplier = 0.1f;

    private IEnumerator Attack(GameObject target)
    {
        hits = 0;

        // Stop the AI character and disable its movement.
        aiPath.maxSpeed = 0f;
        aiPath.destination = transform.position;
        aiPath.canMove = false;

        behaviourIsActive = true;
        AICharacterStats animalStats = target.GetComponentInChildren<AICharacterStats>();
        attackTargetStats = animalStats;

        while (behaviourIsActive)
        {
            // Make the AI character face its target.
            transform.LookAt(target.transform);

            // Perform a random attack action.
            yield return StartCoroutine(PerformRandomActionOverTime(target.transform, attackAnimations, animalStats));

            if (target.transform.CompareTag("Animal"))
            {
                // Check if the target animal is dead, then change the state.
                if (animalStats.isDead)
                {
                    ChangeState(AIState.HuntMeat);
                    yield break;
                }
            }
            else
            {
                ChangeState(AIState.Idle);
                yield break;
            }

            yield return null;
        }

        hits = 0;
    }

    private AICharacterStats attackTargetStats;

    private int hits = 0;
    private int someThreshold; // Moved outside method scope

    public void DealDamage()
    {
        if (someThreshold == 0)
        { // Initialize if not set
            someThreshold = UnityEngine.Random.Range(2, 8);
        }

        hits++;

        // Define a chance for triggering physics
        float chanceToTriggerPhysics = 0.3f; // 30% chance to trigger physics
        float randomChance = UnityEngine.Random.value; // Random value between 0.0 and 1.0

        if (target.transform.CompareTag("Trees"))
        {
            ProceduralTree proceduralTree = target.GetComponent<ProceduralTree>();
            PTGrowing ptGrowing = target.GetComponent<PTGrowing>();
            // Check if the random chance is less than or equal to our defined chance
            if (randomChance <= chanceToTriggerPhysics && !ptGrowing.isDead && !ptGrowing.isGrowing && ptGrowing.isFullyGrown)
            {
                proceduralTree.EnablePhysicsInBurstMode();
            }
        }
        else
        {
            attackTargetStats.TakeDamage(attackMultiplier);
        }
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

    List<string> attackAnimations = new List<string> { HumanControllerAnimations.Attack_Slash01, HumanControllerAnimations.Attack_Slash02, HumanControllerAnimations.Attack_Stab01, HumanControllerAnimations.Attack_Stab02, HumanControllerAnimations.Attack_Stab03, };

    List<string> neanderthalAnimsHarvest = new List<string> { HumanControllerAnimations.Attack_Neanderthal_Punch01, HumanControllerAnimations.Attack_Neanderthal_Punch02 };
    List<string> midSapienAnimsHaevest = new List<string> { HumanControllerAnimations.Attack_Slash01, HumanControllerAnimations.Attack_Slash02 };
    List<string> sapienAnimHarvest = new List<string> { HumanControllerAnimations.Action_Standing_HarvestTree };

    private Coroutine currentActionCoroutine = null;

    private IEnumerator HarvestAnimation(Transform target)
    {
        behaviourIsActive = true;

        while (behaviourIsActive)
        {
            transform.LookAt(target);
            StartCoroutine(ChangeSpeedOverTime(target));

            // Ensure we stop the previous coroutine before starting a new one
            if (currentActionCoroutine != null)
            {
                StopCoroutine(currentActionCoroutine);
            }

            if (currentEvolutionState == EvolutionState.Neanderthal)
            {
                currentActionCoroutine = StartCoroutine(PerformRandomActionOverTime(target.transform, neanderthalAnimsHarvest, stats));
            }
            else if (currentEvolutionState == EvolutionState.MidSapien)
            {
                currentActionCoroutine = StartCoroutine(PerformRandomActionOverTime(target.transform, midSapienAnimsHaevest, stats));
            }
            else if (currentEvolutionState == EvolutionState.Sapien)
            {
                currentActionCoroutine = StartCoroutine(PerformRandomActionOverTime(target.transform, sapienAnimHarvest, stats));
            }

            yield return new WaitUntil(() => currentActionCoroutine == null); // Wait for the current action to finish
        }


        yield break;
    }

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

        currentActionCoroutine = null; // Indicate this coroutine has finished
    }


    private IEnumerator ChangeSpeedOverTime(Transform target)
    {
        float randomAnimationSpeed = GetRandomAnimationSpeed();
        float randomInterval = GetRandomInterval();
        StartCoroutine(SmoothlyChangeAnimationSpeed(randomAnimationSpeed, randomInterval));

        yield return new WaitForSeconds(randomInterval);

        animator.speed = 1.0f; // Reset animation speed to normal when finished
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

        behaviourIsActive = true;

        yield break;
    }


    private IEnumerator DialogueActive()
    {
        behaviourIsActive = true;

        while (behaviourIsActive)
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

        behaviourIsActive = true;

        while (behaviourIsActive)
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
        behaviourIsActive = true;
        aiPath.canMove = true;

        while (behaviourIsActive)
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
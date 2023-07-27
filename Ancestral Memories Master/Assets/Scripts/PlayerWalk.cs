using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;
using Pathfinding;

public class PlayerWalk : MonoBehaviour
{
    public RPCamera rpCam;

    public Transform lookAtTarget;

    public Camera cam;
    public CamControl cineCam;

    //public NavMeshAgent agent;

    public Player player;
    public GameObject playerObject;

    [SerializeField] private AudioSFXManager playerSFX;

    private string currentState;

    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_WALK = "Player_walk";
    const string PLAYER_JOG = "Player_jog";
    const string PLAYER_RUN = "Player_run";
    const string PLAYER_CROUCH = "Player_crouch";
    const string PLAYER_SNEAK = "Player_sneak";

    const string PLAYER_STARVINGIDLE = "Player_starvingIdle";
    const string PLAYER_STARVINGWALK = "Player_starvingWalk";
    const string PLAYER_STARVINGCRITICAL = "Player_starvingCritical";

    const string PLAYER_CURIOUSIDLE = "Player_curiousIdle";

    const string PLAYER_DRUNKIDLE = "Player_drunkIdle";
    const string PLAYER_DRUNKWALK = "Player_drunkWalk";
    const string PLAYER_DRUNKRUN = "Player_drunkRun";

    const string PLAYER_CURIOUS = "Player_curious";


    bool playerIsCrouched = false;

    [SerializeField] private float walkThreshold = 0;
    public float runThreshold = 25;
    public float speed = 0;
    [SerializeField] private float walkAnimFactor = 0;
    [SerializeField] private float distanceRatios = 2;
    [SerializeField] float distanceThreshold = 60;
    [SerializeField] float distance = 0;
    [SerializeField] float animFactor = 9;

    public CharacterBehaviours behaviours;

    public AreaManager areaManager;

    public LayerMask caveGroundLayer;
    public LayerMask grassGroundLayer;
    public LayerMask waterLayer;

    public LayerMask walkableLayers;
    public LayerMask terrainDetectionLayers;

    private RaycastHit rayHit;

    private float targetWaterDepth;
    private float waterDepth;

    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    [SerializeField] private List<Transform> head;
    [SerializeField] private List<Transform> feet;

    [SerializeField] private CamControl camControl;

    private RichAI aiPath;

    void Awake()
    {
        //agent.stoppingDistance = defaultStoppingDistance;
        //agent = GetComponent<NavMeshAgent>();


        //StopAgent();
        player = GetComponentInChildren<Player>();

        head.Add(playerHead);
        feet.Add(leftFoot);
        feet.Add(rightFoot);

        defaultBoundsSize = new Vector3(1, 1, 1);
    }
    //Water detection WIP

    float minParamDepth = 0;
    float maxParamDepth = 1;

    private string paramID;

    private void Start()
    {

        aiPath = GetComponentInChildren<RichAI>();
        aiPath.endReachedDistance = defaultStoppingDistance;

        ChangeState(PLAYER_IDLE);
        //StartCoroutine(DetectWater());
    }

    private IEnumerator GetWaterDepth(Transform raySource)
    {
        while (playerInWater)
        {
            if (Physics.Raycast(raySource.transform.position, Vector3.down, out RaycastHit rayHit, Mathf.Infinity, waterLayer))
            {
                distance = Vector3.Distance(playerHead.transform.position, rayHit.point);

                float t = Mathf.InverseLerp(rayHit.point.y, playerHead.transform.position.y, distance);
                float waterDepthOutput = Mathf.Lerp(minParamDepth, maxParamDepth, t);

                targetWaterDepth = waterDepthOutput;
                waterDepth = targetWaterDepth;

                playerSFX.UpdateWaterDepth(raySource, waterDepth);

                Debug.Log("WaterDepth:" + waterDepth);
            }

            yield return null;
        }

        playerSFX.UpdateWaterDepth(raySource, minParamDepth);

        yield break;
    }

    public bool playerInWater = false;

    private void WaterDetected(Transform raySource)
    {
        playerInWater = true;
        StartCoroutine(GetWaterDepth(raySource));
    }

    [SerializeField] private string lastTerrainHit;

    // The order of this enum
    // must correspond to the index order in the FMOD Studio project 'Terrain Type' parameter.

    public enum GroundTypes
    {
        GroundIndex,
        RockIndex,
        WaterIndex
    }

    [SerializeField] private bool checkActive = false;

    // The CheckAreaForFootSteps() method of checking the ground type uses the AreaManager script to set it directly.
    // This isn't ideal because it's not so dynamic as RayCasting,
    // It still works as a failsafe in the event RayCasting fails  (Currently, it has).

    public void CheckAreaForFootSteps(string currentRoom)
    {
        switch (currentRoom)
        {
        
        case "Outside":
                playerSFX.AreaUpdateGroundType((int)GroundTypes.GroundIndex);
                //playerSFX.RayUpdateGroundType((int)GroundTypes.GroundIndex);
                break;
        case "InsideCave":
                playerSFX.AreaUpdateGroundType((int)GroundTypes.RockIndex);
                //playerSFX.RayUpdateGroundType((int)GroundTypes.RockIndex);
                break;
        default:
        Debug.LogError("Unknown room in PlayerWalk/CheckAreaForFootSteps(): " + "room = " + currentRoom);
        break;
        }
    }

    // This is a separate RayCast method to detect water This is part of the area-driven method. But is currently unused.

    public IEnumerator DetectWater()
    {
        while (true)
        {
            if (Physics.Raycast(head[0].transform.position, Vector3.down, out RaycastHit rayHit, Mathf.Infinity, terrainDetectionLayers))
            {
                Debug.DrawRay(head[0].transform.position, Vector3.down * rayHit.distance, Color.green);


                // Check if  ground = water and the ray is coming from the player head
                if (rayHit.collider.gameObject.layer == waterLayer)
                {
                    WaterDetected(head[0]);

                    playerSFX.AreaUpdateGroundType((int)GroundTypes.WaterIndex);

                    Debug.Log("Ground type detected: Water");

                    lastTerrainHit = rayHit.ToString();

                }
                else if (rayHit.collider.gameObject.layer != waterLayer)
                {
                    if (areaManager.currentRoom == "Outside")
                    {
                        playerSFX.AreaUpdateGroundType((int)GroundTypes.GroundIndex);
                        lastTerrainHit = "Outside";
                    }
                    else if (areaManager.currentRoom == "InsideCave")
                    {
                        playerSFX.AreaUpdateGroundType((int)GroundTypes.RockIndex);
                        lastTerrainHit = "InsideCave";
                    }
                }
            }

            yield return null;
        }
    }

    // This is the RayCast method uses a raycast, and is not in use.
    // This is called from PlayerSoundEffects in the 'Human' child of the Player Transform.
    // Currently in a state that may not make sense due to attempting to fix.

    public IEnumerator DetectGroundType()
    {
        while (true)
        {
            bool groundTypeUpdated = false;

            foreach (Transform rayTransform in head)
            {
                if (Physics.Raycast(rayTransform.transform.position, Vector3.down, out RaycastHit rayHit, Mathf.Infinity, terrainDetectionLayers))
                {
                    // Check if  ground = water and the ray is coming from the player head
                    if (rayHit.collider.gameObject.layer == waterLayer)
                    {
                        WaterDetected(rayTransform);
          
                        playerSFX.RayUpdateGroundType(rayTransform, (int)GroundTypes.WaterIndex);

                        Debug.Log("Ground type detected: Water");

                        lastTerrainHit = rayHit.ToString();
                        groundTypeUpdated = true;

                        continue;
                    }
                }
            }

            if (!groundTypeUpdated)
            {
                foreach (Transform rayTransform in feet)
                {
                    if (Physics.Raycast(rayTransform.transform.position, Vector3.down, out RaycastHit rayHit, Mathf.Infinity, terrainDetectionLayers))
                    {
                        // Check if  ground = grass and the ray is not coming from the player head
                        if (rayHit.collider.gameObject.layer == grassGroundLayer)
                        {
                            playerSFX.RayUpdateGroundType(rayTransform, (int)GroundTypes.GroundIndex);
                            Debug.Log("Ground type detected: Grass");

                            if (rayHit.collider.ToString() != lastTerrainHit)
                            {
                                lastTerrainHit = rayHit.ToString();
                                groundTypeUpdated = true;
                            }

                            continue;
                        }

                        // Check if ground = cave and the ray is not coming from the player head
                        if (rayHit.collider.gameObject.layer == caveGroundLayer)
                        {
                            playerSFX.RayUpdateGroundType(rayTransform, (int)GroundTypes.RockIndex);
                            Debug.Log("Ground type detected: Cave");

                            if (rayHit.collider.ToString() != lastTerrainHit)
                            {
                                lastTerrainHit = rayHit.ToString();
                                groundTypeUpdated = true;
                            }

                            continue;
                        }

                    }
                }
            }

            yield return null;
        }
    }

    void Update()
    {
        if (!camControl.panoramaScroll)
        {
            if (!stopOverride)
            {
                if (!Input.GetMouseButton(1) && !behaviours.behaviourIsActive && !areaManager.traversing)
                {
                    if (Input.GetMouseButton(0) && !player.isDead)
                    {
                        CastRayToGround();
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        if (!aiPath.reachedEndOfPath && !player.isDead)
                        {
                            StopAgent();
                        }
                    }
                }
            }
        } else
        {
            StopAgent();
        }
    }

    private void CastRayToGround()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, walkableLayers)) return;

        Vector3 playerPosition = playerObject.transform.position;

        distance = Vector3.Distance(playerPosition, rayHit.point);

        if (distance >= distanceThreshold)
        {
            return;
        }

        MoveAgent(rayHit.point, distance, playerPosition);
    }

    private void MoveAgent(Vector3 hitPoint, float cursorDistance, Vector3 playerPosition)
    {
        if (!behaviours.isPsychdelicMode)
        {
            speed = cursorDistance / distanceRatios;
        } else if (behaviours.isPsychdelicMode)
        {
            speed = runThreshold + 1;
        }

        //agent.destination = hitPoint;
        //agent.speed = speed;
        //agent.isStopped = false;
        //agent.acceleration = 10000;

        walkAnimFactor = speed / animFactor;

        aiPath.destination = hitPoint;
        aiPath.maxSpeed = speed;
        aiPath.acceleration = 10000;
        walkAnimFactor = speed / animFactor;
        aiPath.canMove = true;

        player.AdjustAnimationSpeed(walkAnimFactor);

        if (speed < runThreshold)
        {
            if (!behaviours.isPsychdelicMode && !player.isStarving)
            {
                ChangeState(PLAYER_WALK);
            }
            else if (!behaviours.isPsychdelicMode && player.isStarving)
            {
                ChangeState(PLAYER_WALK); // Change this if we want to make the player walk animation respond to starvation
            }
            else if (behaviours.isPsychdelicMode)
            {
                ChangeState(PLAYER_WALK);
            }
        }

        if (speed >= runThreshold)
        {
            ChangeState(PLAYER_RUN);
        }
    }

    float defaultStoppingDistance = 0f;
    public float stoppingDistance;
    public bool reachedDestination = false;
    public float closeDistance = 5.0f;

    [SerializeField] private float walkTowardSpeed = 16;

    private Vector3 destination;
    [SerializeField] private Vector3 defaultBoundsSize;
    private Vector3 boundsSize;

    public bool showDestinationGizmos = false;

    [SerializeField] private GameObject destinationGizmo;

    // private DestinationGizmo trigger;

    [SerializeField] private float distanceToDestination;
    [SerializeField] private float inRangeThreshold = 30f;

    public float minimumStopDistance = 1f;
    public float treeHarvestStopDistance = 6f;
    public float talkStopDistance = 10f;
    public float enterRoomStopDistance = 10f;
    [SerializeField] float timeout = 10f;

    public MusicManager musicManager;

    public IEnumerator WalkToward(GameObject hitObject, string selectedChoice, Transform teleportTarget, RaycastHit rayHit)
    {
        // sizeCalculated = bounds of the selected (hitObject) object, divided by some acceptable factor to achieve the desired trigger bounds.

        destination = hitObject.transform.position;

        switch (selectedChoice)
        {

            case "Drink":
                destination = rayHit.point;
                boundsSize = player.transform.localScale;
                destinationGizmo.transform.localScale = boundsSize;
                inRangeThreshold = minimumStopDistance;
                break;
            case "KindleFire":
                destinationGizmo.transform.localScale = defaultBoundsSize * 2;
                inRangeThreshold = minimumStopDistance;
                break;
            case "Eat":
                destinationGizmo.transform.localScale = defaultBoundsSize * 2;
                inRangeThreshold = minimumStopDistance;
                break;
            case "Reflect":
                destinationGizmo.transform.localScale = defaultBoundsSize / 1;
                inRangeThreshold = minimumStopDistance;
                break;
            case "Look":
                destinationGizmo.transform.localScale = defaultBoundsSize;
                inRangeThreshold = minimumStopDistance;
                break;
            case "Pray":
                if (hitObject.transform.CompareTag("Trees"))
                {
                    boundsSize = hitObject.GetComponentInChildren<Renderer>().bounds.size;
                    destinationGizmo.transform.localScale = boundsSize / 3;
                    inRangeThreshold = treeHarvestStopDistance;
                }
                else
                {
                    destinationGizmo.transform.localScale = defaultBoundsSize;
                    inRangeThreshold = minimumStopDistance;
                }
                break;
            case "HarvestTree":
                boundsSize = hitObject.GetComponentInChildren<Renderer>().bounds.size;
                destinationGizmo.transform.localScale = boundsSize / 5;
                inRangeThreshold = treeHarvestStopDistance;
                break;
            case "Talk":
                destinationGizmo.transform.localScale = defaultBoundsSize * 18;
                inRangeThreshold = talkStopDistance;
                break;
            case "Enter":
            case "Exit":
                destinationGizmo.transform.localScale = defaultBoundsSize * 2;
                inRangeThreshold = enterRoomStopDistance;
                break;
            case "InstructHarvest":
                destinationGizmo.transform.localScale = defaultBoundsSize * 2;
                inRangeThreshold = talkStopDistance;
                break;
            case "InstructHunt":
                destinationGizmo.transform.localScale = defaultBoundsSize * 2;
                inRangeThreshold = talkStopDistance;
                break;
            default:
                boundsSize = hitObject.GetComponentInChildren<Renderer>().bounds.size;
                destinationGizmo.transform.localScale = boundsSize;
                inRangeThreshold = minimumStopDistance;
                break;
        }


        GameObject destinationGizmoInstance = Instantiate(destinationGizmo, destination, Quaternion.identity);
        //DestinationGizmo trigger = destinationGizmoInstance.GetComponent<DestinationGizmo>();
        Renderer gizmoRenderer = destinationGizmoInstance.transform.GetComponent<Renderer>();

        if (!showDestinationGizmos)
        {
            gizmoRenderer.enabled = false;
        }
        else { 
            gizmoRenderer.enabled = true;
        }

        reachedDestination = false;
        ChangeState(PLAYER_RUN);
        walkAnimFactor = walkTowardSpeed / animFactor;
        player.AdjustAnimationSpeed(walkAnimFactor);

        //agent.destination = destination;
        //agent.speed = walkTowardSpeed;
        //agent.isStopped = false;

        aiPath.destination = destination;
        aiPath.maxSpeed = speed;
        aiPath.canMove = true;

        float timeElapsed = 0f;
        float distanceToDestination = Vector3.Distance(player.transform.position, destination);

        while (distanceToDestination > inRangeThreshold && timeElapsed < timeout)
        {
            distanceToDestination = Vector3.Distance(player.transform.position, destination);

            if (Input.GetMouseButtonDown(0) && !areaManager.traversing)
            {
                Debug.Log("Behaviour cancelled!");
                CancelWalkToward();
                yield break;
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (timeElapsed >= timeout)
        {
            Debug.LogWarning("Timeout reached before destination reached.");
            CancelWalkToward();
            yield break;
        }

        void CancelWalkToward()
        {
            reachedDestination = true;
//agent.stoppingDistance = defaultStoppingDistance;
        
            behaviours.SheatheItem();
            Destroy(destinationGizmoInstance);
        }

        reachedDestination = true;

        if (!areaManager.traversing)
        {
            StopAgent();
            Destroy(destinationGizmoInstance);

//agent.stoppingDistance = defaultStoppingDistance;

            aiPath.endReachedDistance = defaultStoppingDistance;
            Debug.Log("Arrived.");
            transform.LookAt(hitObject.transform.position);

            behaviours.ChooseBehaviour(selectedChoice, hitObject);
            yield break;
        } else if (areaManager.traversing)
        {
            //agent.stoppingDistance = defaultStoppingDistance;
            aiPath.endReachedDistance = defaultStoppingDistance;
            Debug.Log("ToTeleport.");

            if (areaManager.isEntering)
            {
//StartCoroutine(areaManager.Teleport(agent, teleportTarget));

            }
            else if (!areaManager.isEntering)
            {
                Debug.Log("Entered");
                StopAgent();
                Destroy(destinationGizmoInstance);
                areaManager.traversing = false;
                yield break;
            }
        }

        yield break;

    }

    [SerializeField] private float defaultAnimSpeed = 1f;

    public void StopAgent()
    {
        player.AdjustAnimationSpeed(defaultAnimSpeed);

        if (!behaviours.isPsychdelicMode && !player.isStarving)
        {
            ChangeState(PLAYER_IDLE);
        }
        if (!behaviours.isPsychdelicMode && player.isStarving)
        {
            ChangeState(PLAYER_STARVINGIDLE);

        }
        else if (behaviours.isPsychdelicMode)
        {
            ChangeState(PLAYER_CURIOUS);
        }

        //agent.ResetPath();
        //agent.isStopped = true;

        aiPath.destination = transform.position;
        aiPath.canMove = false;

        //Debug.Log("Player moving?" + agent.isStopped);
    }

    public void ChangeState(string newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        player.ChangeAnimationState(newState);
    }

    [SerializeField] private bool stopOverride = false;

    public void StopAgentOverride()
    {
        stopOverride = true;

        StopAgent();
    }

    public void CancelAgentOverride()
    {
        stopOverride = false;
        
    }

}
        
    

using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerWalk : MonoBehaviour
{
    public RPCamera rpCam;

    public Transform lookAtTarget;

    public Camera cam;
    public CamControl cineCam;

    public NavMeshAgent agent;

    public CharacterClass player;
    public GameObject playerObject;

    [SerializeField] private PlayerSoundEffects playerSFX;

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

    bool playerIsCrouched = false;

    [SerializeField] private float walkThreshold = 0;
    [SerializeField] private float runThreshold = 25;
    [SerializeField] private float speed = 0;
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
    public LayerMask waterLayers;

    private RaycastHit rayHit;

    private float targetWaterDepth;
    private float waterDepth;

    [SerializeField] private Transform playerHead;
    [SerializeField] private Transform leftFoot;
    [SerializeField] private Transform rightFoot;

    [SerializeField] private List<Transform> raySources;

    [SerializeField] private CamControl camControl;

    void Awake()
    {
        agent.stoppingDistance = defaultStoppingDistance;
        agent = GetComponent<NavMeshAgent>();
        //StopAgent();

        raySources.Add(playerHead);
        raySources.Add(leftFoot);
        raySources.Add(rightFoot);

        defaultBoundsSize = new Vector3(1, 1, 1);
    }
    //Water detection WIP

    float minParamDepth = 0;
    float maxParamDepth = 1;

    private string paramID;

    private void Start()
    {
        ChangeState(PLAYER_IDLE);
    }

    private IEnumerator GetWaterDepth(Transform raySource)
    {
        while (playerInWater)
        {
            if (Physics.Raycast(playerHead.transform.position, Vector3.down, out RaycastHit rayHit, Mathf.Infinity, waterLayers))
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

        yield break;
    }

    public bool playerInWater = false;

    private void WaterDetected(Transform raySource)
    {
        StartCoroutine(GetWaterDepth(raySource));
    }

    public IEnumerator DetectGroundType()
    {
        foreach (Transform rayTransform in raySources)
        {

            //Vector3 raySource = new Vector3(rayTransform.position.x, rayTransform.position.y, rayTransform.position.z);

            if (Physics.Raycast(rayTransform.transform.position, Vector3.down, out RaycastHit rayHit, Mathf.Infinity, waterLayers))
            {
                //                Gizmos.DrawRay(rayTransform.transform.position, Vector3.down);

                if (rayHit.transform.gameObject.layer == waterLayer && rayTransform.CompareTag("PlayerHead"))
                {
                    playerInWater = true;
                    WaterDetected(rayTransform);
                    playerSFX.UpdateGroundType(rayTransform, (int)GroundTypes.WaterIndex);

                    Debug.Log("RayTransform:" + rayTransform);
                    Debug.Log("Hit:" + rayHit.transform.gameObject.layer);

                    continue;
                    //currentGround = "Water";
                }
                if (rayHit.transform.gameObject.layer == grassGroundLayer && !rayTransform.CompareTag("PlayerHead"))
                {
                    playerInWater = false;
                    playerSFX.UpdateGroundType(rayTransform, (int)GroundTypes.GroundIndex);
                    Debug.Log("RayTransform:" + rayTransform);
                    Debug.Log("Hit:" + rayHit.transform.gameObject.layer);
                    continue;
                    //currentGround = "Grass";
                }

                if (rayHit.transform.gameObject.layer == caveGroundLayer && !rayTransform.CompareTag("PlayerHead"))
                {
                    playerInWater = false;
                    playerSFX.UpdateGroundType(rayTransform, (int)GroundTypes.RockIndex);
                    Debug.Log("RayTransform:" + rayTransform);
                    Debug.Log("Hit:" + rayHit.transform.gameObject.layer);
                    continue;

                    //currentGround = "Rock";
                }
            }
        }

        yield break;
    }

    public enum GroundTypes
    {
        GroundIndex,
        RockIndex,
        WaterIndex
    }

    [SerializeField] private bool checkActive = false;

    private string currentGround;
    private string lastGround;


    void Update()
    {
      
        if (!stopOverride)
        {
            if (!Input.GetMouseButton(1) && !behaviours.behaviourIsActive && !areaManager.traversing)
            {
                if (Input.GetMouseButton(0) && !player.hasDied)
                {
                    CastRayToGround();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (!agent.isStopped && !player.hasDied)
                    {
                        StopAgent();
                    }
                }
            }
        }
    }

    private void CastRayToGround()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, walkableLayers)) return;

        Vector3 playerPosition = playerObject.transform.position;

        distance = Vector3.Distance(playerPosition, rayHit.point);

        if (distance >= distanceThreshold) return;

        MoveAgent(rayHit.point, distance, playerPosition);
    }

    private void MoveAgent(Vector3 hitPoint, float cursorDistance, Vector3 playerPosition)
    {
        speed = cursorDistance / distanceRatios;
        agent.destination = hitPoint;
        agent.speed = speed;
        walkAnimFactor = speed / animFactor;
        agent.isStopped = false;
        agent.acceleration = 10000;

        player.AdjustAnimationSpeed(walkAnimFactor);

        if (speed < runThreshold)
        {
            if (!behaviours.isPsychdelicMode && !player.isStarving)
            {
                ChangeState(PLAYER_WALK);
            }
            else if (player.isStarving)
            {
                ChangeState(PLAYER_STARVINGWALK);
            }
            else if (behaviours.isPsychdelicMode)
            {
                ChangeState(PLAYER_DRUNKWALK);
            }

        }

        if (speed >= runThreshold)
        {
            if (!behaviours.isPsychdelicMode)
            {
                ChangeState(PLAYER_RUN);
            }
            else if (behaviours.isPsychdelicMode)
            {
                ChangeState(PLAYER_RUN);
                //ChangeState(PLAYER_DRUNKRUN);
            }
        }
    }

    float defaultStoppingDistance = 0f;
    public float stoppingDistance;
    public bool reachedDestination = false;
    public float closeDistance = 5.0f;

    private float walkTowardSpeed;

    private Vector3 destination;
    [SerializeField] private Vector3 defaultBoundsSize;
    private Vector3 boundsSize;

    public bool showDestinationGizmos = false;

    [SerializeField] private GameObject destinationGizmo;

    // private DestinationGizmo trigger;

    [SerializeField] private float distanceToDestination;
    [SerializeField] private float inRangeThreshold = 30f;

    [SerializeField] private float minimumStopDistance = 1f;
    [SerializeField] private float treeHarvestStopDistance = 1f;
    [SerializeField] private float talkStopDistance = 10f;
    [SerializeField] private float enterRoomStopDistance = 10f;
    [SerializeField] float timeout = 10f;

    public IEnumerator WalkToward(GameObject hitObject, string selectedChoice, Transform teleportTarget, RaycastHit rayHit)
    {
        // sizeCalculated = bounds of the selected (hitObject) object, divided by some factor to achieve the desired trigger bounds.
        // Needs refactoring... 

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
        walkTowardSpeed = runThreshold + 1;
        walkAnimFactor = walkTowardSpeed / animFactor;
        player.AdjustAnimationSpeed(walkAnimFactor);
        agent.destination = destination;
        agent.speed = walkTowardSpeed;
        agent.isStopped = false;

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
            agent.stoppingDistance = defaultStoppingDistance;
            behaviours.SheatheItem();
            Destroy(destinationGizmoInstance);
        }

        yield return new WaitUntil(() => distanceToDestination <= inRangeThreshold);

        reachedDestination = true;

        if (!areaManager.traversing)
        {
            StopAgent();
            Destroy(destinationGizmoInstance);

            agent.stoppingDistance = defaultStoppingDistance;
            Debug.Log("Arrived.");
            agent.transform.LookAt(hitObject.transform.position);

            behaviours.ChooseBehaviour(selectedChoice, hitObject);
            yield break;
        } else if (areaManager.traversing)
        {
            agent.stoppingDistance = defaultStoppingDistance;
            Debug.Log("ToTeleport.");

            if (areaManager.isEntering)
            {
                StartCoroutine(areaManager.Teleport(agent, teleportTarget));

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
        player.AdjustAnimationSpeed(defaultAnimSpeed / 2);

        if (!behaviours.isPsychdelicMode && !player.isStarving)
        {
            ChangeState(PLAYER_IDLE);
        }
        if (player.isStarving)
        {
            ChangeState(PLAYER_STARVINGIDLE);

        }
        else if (behaviours.isPsychdelicMode)
        {
            ChangeState(PLAYER_DRUNKIDLE);
        }

        agent.ResetPath();

        agent.isStopped = true;
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
        
    

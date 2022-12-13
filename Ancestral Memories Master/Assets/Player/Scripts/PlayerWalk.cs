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

    public LayerMask walkableLayers;
    public LayerMask detectWaterLayers;

    private RaycastHit rayHit;

    private float targetWaterDepth;
    private float waterDepth;

    [SerializeField] private Transform playerHead;

    [SerializeField] private List<Transform> raySources;


    void Awake()
    {
        agent.stoppingDistance = defaultStoppingDistance;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

        raySources.Add(playerHead);


    }

    /*

    //Water detection WIP
 
    private IEnumerator SetWaterDepth(float targetWaterDepth)
    {
        while (playerInWater)
        {
            if (!playerInWater)
            {
                yield break;
            }

            waterDepth = targetWaterDepth;

            RuntimeManager.StudioSystem.setParameterByName("WaterDepth", waterDepth);
            Debug.Log("WaterDepth:" + waterDepth);
            yield return null;
        }

        yield break;

    }

    float minParamDepth = 0;
    float maxParamDepth = 1;

    private IEnumerator GetWaterDepth(Vector3 playerHead, RaycastHit rayHit, float distance)
    {
        while (playerInWater)
        {
            var t = Mathf.InverseLerp(playerHead.y, rayHit.point.y, distance);
            float output = Mathf.Lerp(minParamDepth, maxParamDepth, t);

            targetWaterDepth = output;
            StartCoroutine(SetWaterDepth(targetWaterDepth));
            yield return null;
        }

        if (!playerInWater)
        {
            yield break;
        }

        yield break;
    }

    public bool playerInWater = false;

    private void WaterDetected(Vector3 playerHead, RaycastHit rayHit, float distance)
    {
        StartCoroutine(GetWaterDepth(playerHead, rayHit, distance));
    }

    void Update()
    {

        foreach (Transform t in raySources)
        {
   
            Vector3 raySource = new Vector3(t.transform.position.x, t.transform.position.y, t.transform.position.z); 
            Vector3 rayDirection = Vector3.down;

            if (Physics.Raycast(t.transform.position, rayDirection, out RaycastHit rayHit, Mathf.Infinity, detectWaterLayers))
            {
                float distance = rayHit.distance;

                if (rayHit.transform.gameObject.layer == LayerMask.NameToLayer("Ground") || rayHit.transform.gameObject.layer == LayerMask.NameToLayer("CaveGround"))
                {
                    playerInWater = false;
                    return;

                } else if (rayHit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
                {
                    playerInWater = true;
                    Debug.Log("Water Detected!");
                    WaterDetected(raySource, rayHit, distance);
                }

                Debug.DrawRay(t.transform.position, rayDirection, Color.blue);
            }

        }

    */
    private void Update()
    {
        
        if (!stopOverride)
        {
            if (!Input.GetMouseButton(1) && !behaviours.behaviourIsActive && !areaManager.traversing)
            {
                if (Input.GetMouseButton(0) && player.hasDied == false && cineCam.cinematicActive == false)
                {
                    CastRayToGround();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (agent.isStopped == false && player.hasDied == false && cineCam.cinematicActive == false)
                    {
                        StopAgent();
                    }
                }

                void CastRayToGround()
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                    int groundLayerIndex = LayerMask.NameToLayer("Ground");
                    int groundLayerMask = (1 << groundLayerIndex);

                    int caveGroundLayerIndex = LayerMask.NameToLayer("InsideCave");
                    int caveGroundLayerMask = (1 << caveGroundLayerIndex);

                    if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, walkableLayers))
                    {
                        Vector3 playerPosition = playerObject.transform.position;

                        distance = Vector3.Distance(playerPosition, rayHit.point);

                        Debug.Log(distance);

                        if (distance >= distanceThreshold)
                        {
                            return;
                        }
                        else
                        {
                            MoveAgent(rayHit.point, distance, playerPosition);
                        }
                    }
                }

                void MoveAgent(Vector3 hitPoint, float cursorDistance, Vector3 playerPosition)
                {
                    speed = cursorDistance / distanceRatios;
                    agent.destination = hitPoint;
                    agent.speed = speed;
                    walkAnimFactor = speed / animFactor;

                    if (speed < runThreshold)
                    {
                        if (!behaviours.isPsychdelicMode && !player.starving)
                        {
                            ChangeState(PLAYER_WALK);
                        }
                        else if (player.starving)
                        {
                            ChangeState(PLAYER_STARVINGWALK);
                        }
                        else if (behaviours.isPsychdelicMode)
                        {
                            ChangeState(PLAYER_DRUNKWALK);
                        }

                        //player.AdjustAnimationSpeed(animSpeed);
                    }

                    if (speed > runThreshold)
                    {
                        if (!behaviours.isPsychdelicMode)
                        {
                            ChangeState(PLAYER_RUN);
                        }
                        else if (behaviours.isPsychdelicMode)
                        {
                            ChangeState(PLAYER_DRUNKRUN);
                        }

                    }

                    //player.AdjustAnimationSpeed(walkAnimFactor);

                    Debug.Log("Cursor Distance:" + cursorDistance);
                    Debug.Log("Speed:" + agent.speed);

                    agent.isStopped = false;
                    agent.acceleration = 10000;

                    //float runThreshold = cursorDistance / 2;
                }
            }

            return;
        }

        return;
    }

    float defaultStoppingDistance = 0f;
    public float stoppingDistance;
    public bool reachedDestination = false;
    public float closeDistance = 5.0f;

    private float walkTowardSpeed;

    private Vector3 destination;
    private Vector3 sizeCalculated;

    [SerializeField] private GameObject destinationGizmo;

    public IEnumerator WalkToward(GameObject hitObject, string selected, Transform teleportTarget, GameObject tempPortal, RaycastHit rayHit)
    {
        // sizeCalculated = bounds of the selected (hitObject) object, divided by some factor to achieve the desired trigger bounds.


        if (selected == "Drink")
        {
            destination = rayHit.point;
            sizeCalculated = player.transform.localScale;
            destinationGizmo.transform.localScale = sizeCalculated;
        }

        if (selected == "Reflect")
        {
            destinationGizmo.transform.localScale = sizeCalculated / 1;
        }

        if (selected != "Drink")
        {
            destination = hitObject.transform.position;
            sizeCalculated = hitObject.GetComponentInChildren<Renderer>().bounds.size;
            destinationGizmo.transform.localScale = sizeCalculated;
        }

        if (selected == "Look")
        {
            destinationGizmo.transform.localScale = sizeCalculated / 1;
        }

        if (selected == "Pray")
        {
            destinationGizmo.transform.localScale = sizeCalculated / 5;
        }

        if (selected == "HarvestTree")
        {
            destinationGizmo.transform.localScale = sizeCalculated / 8;
        }

        if (selected == "Talk")
        {
            destinationGizmo.transform.localScale = sizeCalculated * 2;
        }

        GameObject destinationGizmoInstance = Instantiate(destinationGizmo, destination, Quaternion.identity);
        DestinationGizmo trigger = destinationGizmoInstance.GetComponent<DestinationGizmo>();

        if (Input.GetMouseButtonDown(0) && !areaManager.traversing)
        {
            Debug.Log("Behaviour cancelled!");

            reachedDestination = true;
            agent.stoppingDistance = defaultStoppingDistance;

            behaviours.SheatheItem();

            Destroy(destinationGizmoInstance);

            yield break;
        }

        reachedDestination = false;

        if (!behaviours.isPsychdelicMode && !player.starving)
        {
            ChangeState(PLAYER_WALK);
        }
        else if (player.starving)
        {
            ChangeState(PLAYER_STARVINGWALK);
        } else if (behaviours.isPsychdelicMode)
        {
            ChangeState(PLAYER_DRUNKWALK);
        }

        walkTowardSpeed = runThreshold + 1;

        agent.destination = destination;
        agent.speed = speed;
        agent.isStopped = false;

        yield return new WaitUntil(() => trigger.hitDestination);

        if (areaManager.traversing)
        {
            reachedDestination = true;

            agent.stoppingDistance = defaultStoppingDistance;
            Debug.Log("ToTeleport.");

            if (areaManager.isEntering)
            {
                StartCoroutine(areaManager.Teleport(agent, teleportTarget, tempPortal));

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
        else
        {
            StopAgent();
            Destroy(destinationGizmoInstance);

            reachedDestination = true;
            agent.stoppingDistance = defaultStoppingDistance;
            Debug.Log("Arrived.");
            agent.transform.LookAt(hitObject.transform.position);

            behaviours.ChooseBehaviour(selected, hitObject);
            yield break;
        }
    }

    public void StopAgent()
    {
        if (!behaviours.isPsychdelicMode && !player.starving)
        {
            ChangeState(PLAYER_IDLE);
        }
        if (player.starving)
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

    bool stopOverride = false;

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
        
    

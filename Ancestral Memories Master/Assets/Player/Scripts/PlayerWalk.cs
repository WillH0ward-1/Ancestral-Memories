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

    const string PLAYER_DRUNKIDLE = "Player_drunkIdle";
    const string PLAYER_DRUNKWALK = "Player_drunkWalk";
    const string PLAYER_DRUNKRUN = "Player_drunkRun";

    bool playerIsCrouched = false;

    [SerializeField] private float walkThreshold = 0;
    [SerializeField] private float runThreshold = 25;
    [SerializeField] private float speed = 0;
    [SerializeField] private float animSpeed = 0;
    [SerializeField] private float distanceRatios = 2;
    [SerializeField] float distanceThreshold = 60;
    [SerializeField] float distance = 0;
    [SerializeField] float animFactor = 9;

    public CharacterBehaviours behaviours;

    public AreaManager areaManager;

    void Awake()
    {
        agent.stoppingDistance = defaultStoppingDistance;
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;

    }

    void Update()
    {
        if (!Input.GetMouseButton(1) && !behaviours.behaviourIsActive && !areaManager.traversing)
        {
            if (Input.GetMouseButton(0) && player.hasDied == false && cineCam.cinematicActive == false && player.isReviving == false)
            {
                CastRayToGround();
            }


            if (Input.GetMouseButtonUp(0))
            {
                if (agent.isStopped == false && player.hasDied == false && cineCam.cinematicActive == false && player.isReviving == false)
                {
                    StopAgent();
                }
            }

            void CastRayToGround()
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                int groundLayerIndex = LayerMask.NameToLayer("Ground");
                int groundLayerMask = (1 << groundLayerIndex);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
                {
                    Vector3 playerPosition = playerObject.transform.position;

                    distance = Vector3.Distance(playerPosition, hit.point);

                    Debug.Log(distance);

                    if (distance >= distanceThreshold)
                    {
                        return;
                    }
                    else
                    {
                        MoveAgent(hit.point, distance, playerPosition);
                    }
                }
            }

            void MoveAgent(Vector3 hitPoint, float cursorDistance, Vector3 playerPosition)
            {
                speed = cursorDistance / distanceRatios;
                agent.destination = hitPoint;
                agent.speed = speed;
                animSpeed = speed / animFactor;

                if (speed < runThreshold)
                {
                    if (playerIsCrouched)
                    {
                        changeState(PLAYER_SNEAK);
                    }
                    else
                    {
                        if (!behaviours.isPsychdelicMode)
                        {
                            changeState(PLAYER_WALK);
                        }
                        else if (behaviours.isPsychdelicMode)
                        {
                            changeState(PLAYER_DRUNKWALK);
                        }
                    }

                    //player.AdjustAnimationSpeed(animSpeed);
                }

                if (speed > runThreshold)
                {
                    if (!playerIsCrouched)
                    {
                        if (!behaviours.isPsychdelicMode)
                        {
                            changeState(PLAYER_RUN);
                        } else if (behaviours.isPsychdelicMode)
                        {
                            changeState(PLAYER_DRUNKRUN);
                        }
                    }
                    else
                    {
                        changeState(PLAYER_SNEAK);
                    }

        
                }

                player.AdjustAnimationSpeed(animSpeed);

                Debug.Log("Cursor Distance:" + cursorDistance);
                Debug.Log("Speed:" + agent.speed);

                agent.isStopped = false;
                agent.acceleration = 10000;

                //float runThreshold = cursorDistance / 2;

                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    EnterSneakMode();

                    void EnterSneakMode()
                    {
                        changeState(PLAYER_CROUCH);
                    }
                }
            }
        }

        return;
    }

    float defaultStoppingDistance = 0f;
    public float stoppingDistance;
    public bool reachedDestination = false;
    public float closeDistance = 5.0f;

    [SerializeField] private GameObject destinationGizmo;

    public IEnumerator WalkToward(GameObject hitObject, string selected, Transform teleportTarget, GameObject tempPortal)
    {

        Vector3 sizeCalculated = hitObject.GetComponentInChildren<Renderer>().bounds.size;
        destinationGizmo.transform.localScale = sizeCalculated;

        if (selected == "HarvestTree")
        {
            destinationGizmo.transform.localScale = sizeCalculated / 8;
        }

        GameObject destinationGizmoInstance = Instantiate(destinationGizmo, hitObject.transform.position, Quaternion.identity);
        DestinationGizmo trigger = destinationGizmoInstance.GetComponent<DestinationGizmo>();

        if (Input.GetMouseButtonDown(0) && !areaManager.traversing)
        {
            Debug.Log("Behaviour cancelled!");

            reachedDestination = true;
            agent.stoppingDistance = defaultStoppingDistance;

            Destroy(destinationGizmoInstance);

            yield break;
        }

        reachedDestination = false;

        changeState(PLAYER_WALK);

        speed = 12;
        agent.destination = hitObject.transform.position;
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
                Debug.Log("EnteredRoom.");
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

            behaviours.ChooseBehaviour(selected, null);
            yield break;
        }

    }

    public void StopAgent()
    {
        if (!behaviours.isPsychdelicMode)
        {
            changeState(PLAYER_IDLE);
        } else if (behaviours.isPsychdelicMode)
        {
            changeState(PLAYER_DRUNKIDLE);
        }

        agent.ResetPath();

        agent.isStopped = true;
        //Debug.Log("Player moving?" + agent.isStopped);
    }

    public void changeState(string newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        player.ChangeAnimationState(newState);
    }

    public void StopAgentOverride()
    {
        agent.ResetPath();
        agent.isStopped = true;
    }

}
        
    

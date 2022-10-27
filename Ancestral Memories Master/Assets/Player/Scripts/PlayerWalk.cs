using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class PlayerWalk : MonoBehaviour
{

    [SerializeField] private InputAction mouseClickAction;
    [SerializeField] private InputManager inputManager;
    public Camera cam;

    public CamFollow cineCam;

    //private string groundTag = "Ground";

    private string currentState;

    private NavMeshAgent agent;

    //private Animator animator;

    public CharacterClass player;

    public GameObject playerBase;

    private Vector3 RPCamOffset;

    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_WALK = "Player_walk";
    const string PLAYER_JOG = "Player_jog";
    const string PLAYER_RUN = "Player_run";
    const string PLAYER_CROUCH = "Player_crouch";
    const string PLAYER_SNEAK = "Player_sneak";

    bool playerIsCrouched = false;

    const string groundTag = "Walkable";

    public RPCamera rpCam;
    //public DirectedAgent agent;

    [SerializeField] private float walkThreshold = 0;

    [SerializeField] private float runThreshold = 25;

    [SerializeField] private float speed = 0;

    [SerializeField] private float animSpeed = 0;

    [SerializeField] private float distanceRatios = 2;

    [SerializeField] float distanceThreshold = 60;

    [SerializeField] float distance = 0;

    [SerializeField] float animFactor = 9;

    private Coroutine moving;

    private Ray ray;

    public bool isWalking;

    void Awake()
    {
        inputManager = GetComponent<InputManager>();
       
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
    }

    private void OnEnable()
    {
        mouseClickAction.Enable();
        mouseClickAction.performed += Move;
    }

    private void OnDisable()
    {
        mouseClickAction.performed -= Move;
        mouseClickAction.Disable();
        StopAgent();
    }

    private void Move(InputAction.CallbackContext context)
    {

        if (!player.hasDied && !cineCam.cinematicActive && !player.isReviving)
        {
            int groundLayerIndex = LayerMask.NameToLayer("Ground");
            int groundLayerMask = (1 << groundLayerIndex);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
            {
                Vector3 playerPosition = playerBase.transform.position;

                if (moving != null && distance <= distanceThreshold)
                {
                    moving = StartCoroutine(MoveAgent(hit.point, distance, playerPosition));
                }
            }
        }
    }


    private void Update()
    {
        ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
    }

    public IEnumerator MoveAgent(Vector3 hitPoint, float cursorDistance, Vector3 playerPosition)
    {
        if (inputManager.walking)
        {
            distance = Vector3.Distance(playerPosition, hitPoint);

            speed = cursorDistance / distanceRatios;
            agent.destination = hitPoint;
            agent.speed = speed;
            animSpeed = speed / animFactor;

            Debug.Log("Cursor Distance:" + cursorDistance);
            Debug.Log("Speed:" + agent.speed);

            agent.isStopped = false;

            if (speed < runThreshold)
            {
                if (playerIsCrouched)
                {
                    ChangeState(PLAYER_SNEAK);
                }
                else
                {
                    ChangeState(PLAYER_WALK);
                }
            }

            if (speed > runThreshold)
            {
                if (playerIsCrouched)
                {
                    yield return null;
                }
                else
                {
                    ChangeState(PLAYER_RUN);
                }
            }
        } else
        {
            StopAgent();
            yield return null;
        }
    }



// EnterSneakMode();
    void EnterSneakMode()
    {
        ChangeState(PLAYER_CROUCH);
    }

    void StopAgent()
    {

        ChangeState(PLAYER_IDLE);

        agent.ResetPath();

        agent.isStopped = true;
        //Debug.Log("Player moving?" + agent.isStopped);
    }

    void ChangeState(string newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        player.ChangeAnimationState(newState);
    }
}
        
    

using UnityEngine;
using UnityEngine.AI;

public class PlayerWalk : Human
{

    public Camera cam;

    public CamFollow cineCam;

    //private string groundTag = "Ground";

    private string currentState;

    private NavMeshAgent agent;

    //private Animator animator;

    public Human player;

    private Health health;

    public GameObject playerBase;

    private AnimationManager animator;

    const string PLAYER_IDLE = "Player_idle";
    const string PLAYER_WALK = "Player_walk";
    const string PLAYER_JOG = "Player_jog";
    const string PLAYER_RUN = "Player_run";
    const string PLAYER_CROUCH = "Player_crouch";
    const string PLAYER_SNEAK = "Player_sneak";

    const string groundTag = "Walkable";

    //public DirectedAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.isStopped = true;
    }

    [SerializeField]
    private float walkThreshold = 0;

    [SerializeField]
    private float runThreshold = 25;

    [SerializeField]
    private float speed = 0;

    [SerializeField]
    private float animSpeed = 0;

    [SerializeField]
    private float distanceRatios = 2;

    [SerializeField]
    float distanceThreshold = 60;

    [SerializeField]
    float distance = 0;

    [SerializeField]
    float animFactor = 9;

    private bool playerHasDied = false;

    void Update()
    {

        if (Input.GetMouseButton(0) && !health.IsDead() && cineCam.cinematicActive == false && !health.IsReviving())
        {
            CastRayToGround();
        }
        

        if (Input.GetMouseButtonUp(0))
        {
            if(agent.isStopped == false && !health.IsDead() && cineCam.cinematicActive == false && !health.IsReviving())
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
                //Vector3 screenPointToGround = ray.origin + ray.direction * 100;
                Vector3 playerPosition = playerBase.transform.position;

                distance = Vector3.Distance(playerPosition, hit.point);

                if (distance >= distanceThreshold)
                {
                    return;
                }

                else
                {

                    MoveAgent(hit.point, distance, playerPosition);
                    //Debug.DrawLine(ray.origin, screenPointToGround, Color.blue);
                    //Debug.DrawLine(screenPointToGround, playerPosition, Color.red);

                }
            }
        }

        void MoveAgent(Vector3 hitPoint, float cursorDistance, Vector3 playerPosition)
        {
           
            speed = cursorDistance / distanceRatios;

            //speed = Mathf.Clamp(speed, 0, 20);

            agent.destination = hitPoint;

            agent.speed = speed;

            animSpeed = speed / animFactor;

            if (speed < runThreshold)
            {
                animator.changeState(PLAYER_WALK);
                //player.AdjustAnimationSpeed(animSpeed);
            }

            if (speed > runThreshold)
            {
                animator.changeState(PLAYER_RUN);
                //player.AdjustAnimationSpeed(animSpeed);
            }

            Debug.Log("Cursor Distance:" + cursorDistance);
            Debug.Log("Speed:" + agent.speed);

            agent.isStopped = false;

            //Debug.Log("Player moving?" + agent.isStopped);

            //agent.acceleration = x;
            //float runThreshold = cursorDistance / 2;
            //player.ChangeAnimationState(PLAYER_WALK);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {

                void EnterSneakMode()
                {

                }
            }

            
        }

        void StopAgent()
        {
            animator.changeState(PLAYER_IDLE);

            agent.ResetPath();

            agent.isStopped = true;
            //Debug.Log("Player moving?" + agent.isStopped);
        }
    }
}
        
    

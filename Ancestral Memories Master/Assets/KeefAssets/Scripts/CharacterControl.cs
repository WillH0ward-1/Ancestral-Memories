using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/*
public class CharacterControl : MonoBehaviour
{

    public NavMeshAgent agent;

    public new Camera camera;

    private RaycastHit hit;

    private CamFollow cam;

    private readonly string groundTag = "Walkable";

    public Rigidbody rb;
    private Vector2 startTouchPosition, endTouchPosition;

    public float jumpForce = 300f;
    private bool jumpAllowed;

    public bool grounded = true;


    public SpawnManager spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        agent = GetComponent<NavMeshAgent>();

        GameManager.gameStarted = true;
    }

    void Update()
    {

        if (GameManager.gameStarted && !cam.cinematicActive)
        {

            if (Input.GetMouseButtonDown(0))

                {
                    jumpAllowed = true;
                    grounded = false;
                    
                }

            Ray ray = camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
            {
                if (hit.collider.CompareTag(groundTag))
                {
                    agent.SetDestination(hit.point);
                    Debug.DrawLine(ray.origin, hit.point, Color.red);
                } else
                {
                    Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.green);
                }
            }


        }

    }

    public void FixedUpdate()
    {
        agent.SetDestination(transform.position);

        if (jumpAllowed && agent.enabled)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.isStopped = true;

            rb.AddForce(Vector3.up * jumpForce);
            jumpAllowed = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        spawnManager.SpawnTriggerEntered();
    }
}

*/
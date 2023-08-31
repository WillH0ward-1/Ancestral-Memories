using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCamera : MonoBehaviour
{
    // Cached components and variables
    private Camera cam;
    private Ray ray;
    private RaycastHit hit;
    private OutlineControl outlineControl;
    private bool selected;
    private GameObject hoverObj;
    
    // Serialized fields and public properties
    [SerializeField] private float maxSelectionDistance = 5f;
    [SerializeField] private float minSelectionDistance = 0f;

    // Reference to other scripts and game objects
    public GameObject player;
    public LayerMask layer;
    public Vector3 collision = Vector3.zero;
    public GameObject lastHit;
    public PlayerWalk playerWalk;
    public RadialMenu radialMenu;
    public CharacterBehaviours behaviour;
    public AreaManager areaManager;
    public Transform lookAtTarget;
    public Transform defaultTarget;
    public FluteControl fluteControl;
    
    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start()
    {
        InitializeRadialMenu();
    }

    private void InitializeRadialMenu()
    {
        radialMenu.player = player;
        radialMenu.playerWalk = playerWalk;
        radialMenu.hitObject = lastHit;
        radialMenu.behaviours = behaviour;
        radialMenu.areaManager = areaManager;
    }

    private void Update()
    {
        if (behaviour.behaviourIsActive || behaviour.dialogueIsActive) return;

        ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, layer)) return;

        hoverObj = hit.transform.gameObject;

        if (Input.GetMouseButtonDown(1)) HandleMouseDown();
        if (Input.GetMouseButtonUp(1)) HandleMouseUp();

        UpdateLookAtTarget();
    }

    private void HandleMouseDown()
    {
        if (hoverObj == null || behaviour.behaviourIsActive || areaManager.traversing || behaviour.isPsychdelicMode) return;

        lastHit = hoverObj;
        outlineControl = lastHit.GetComponentInChildren<OutlineControl>();

        if (IsSelectable(hit.distance))
        {
            selected = true;
            ToggleOutline(true);
            ActivateInteractable();
        }
    }

    private void HandleMouseUp()
    {
        if (hoverObj == null) return;
        ToggleOutline(false);
    }

    private bool IsSelectable(float distance)
    {
        return distance < maxSelectionDistance && distance > minSelectionDistance;
    }

    private void ToggleOutline(bool state)
    {
        if (outlineControl != null) outlineControl.outline.enabled = state;
    }

    private void ActivateInteractable()
    {
        Interactable interactable = hit.collider.GetComponentInParent<Interactable>();
        if (interactable != null) interactable.SpawnMenu(hoverObj, hit);

        if (hit.collider.CompareTag("Player"))
        {
            lookAtTarget.position = cam.WorldToScreenPoint(hit.point);
        }
    }

    private void UpdateLookAtTarget()
    {
        if (!behaviour.behaviourIsActive && !behaviour.dialogueIsActive && !areaManager.traversing)
        {
            lookAtTarget.position = hit.point;
        }
        else
        {
            lookAtTarget.position = defaultTarget.transform.position;
        }
    }
}

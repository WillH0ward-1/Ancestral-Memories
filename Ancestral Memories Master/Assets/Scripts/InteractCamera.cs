using System.Collections;
using System.Collections.Generic;
using FIMSpace.FLook;
using UnityEngine;

public class InteractCamera : MonoBehaviour
{
    private Camera cam;
    private Ray ray;
    private RaycastHit hit;
    private OutlineControl outlineControl;
    private bool selected;
    private GameObject hoverObj;
    private Transform temporaryLookAtTarget;

    [SerializeField] private float maxSelectionDistance = 5f;
    [SerializeField] private float minSelectionDistance = 0f;

    public Player player;
    public LayerMask layer;
    public GameObject lastHit;
    public PlayerWalk playerWalk;
    public RadialMenu radialMenu;
    public CharacterBehaviours behaviour;
    public AreaManager areaManager;
    public FluteControl fluteControl;
    public LookAnimManager lookAnimManager;

    private MapObjGen mapObjGen;

    private void Awake()
    {
        cam = Camera.main;
        temporaryLookAtTarget = new GameObject("TemporaryLookAtTarget").transform;
    }

    public void InitInteractions()
    {
        mapObjGen = FindObjectOfType<MapObjGen>();
        player = mapObjGen.player;
        lookAnimManager = player.GetComponentInChildren<LookAnimManager>();
        InitializeRadialMenu();
    }

    private void InitializeRadialMenu()
    {
        radialMenu.player = player.transform.gameObject;
        radialMenu.playerWalk = playerWalk;
        radialMenu.hitObject = lastHit;
        radialMenu.behaviours = behaviour;
        radialMenu.areaManager = areaManager;
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
    }

    private void Update()
    {
        if (behaviour.behaviourIsActive || behaviour.dialogueIsActive) return;

        ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            if (temporaryLookAtTarget == null)
            {
                temporaryLookAtTarget = new GameObject("TemporaryLookAtTarget").transform;
            }

            temporaryLookAtTarget.position = hit.point;
            hoverObj = hit.transform.gameObject; // Assign hoverObj only if raycast hits
        }
        else
        {
            hoverObj = null; // Assign null if raycast doesn't hit
        }

        UpdateLookAtTarget();

        if (Input.GetMouseButtonDown(1)) HandleMouseDown();
        if (Input.GetMouseButtonUp(1)) HandleMouseUp();
    }

    private void UpdateLookAtTarget()
    {
        if (!behaviour.behaviourIsActive && !behaviour.dialogueIsActive && !areaManager.traversing && temporaryLookAtTarget != null)
        {
            lookAnimManager.LookAt(temporaryLookAtTarget);
        }
        else
        {
            lookAnimManager.DisableLookAt();
        }
    }

    private void OnDestroy()
    {
        if (temporaryLookAtTarget != null)
        {
            Destroy(temporaryLookAtTarget.gameObject);
        }
    }

}

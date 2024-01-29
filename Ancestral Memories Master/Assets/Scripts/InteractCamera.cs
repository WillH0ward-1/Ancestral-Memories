using System.Collections;
using UnityEngine;

public class InteractCamera : MonoBehaviour
{
    private Camera cam;
    private RaycastHit hit;
    private GameObject hoverObj;
  
    private GameObject previousHoverObj; // To track the previously hovered object
    private GameObject selectedObj; // To track the selected object
    private Transform lookAtTarget;
    private Outline outlineComponent; // To store the dynamically created outline component

    [SerializeField] private float maxSelectionDistance = 5f;
    [SerializeField] private float minSelectionDistance = 0f;

    [SerializeField] private Color outlineColor = Color.green; // Set the desired outline color

    [SerializeField] private float outlineWidth = 10f;

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
        lookAtTarget = new GameObject("LookAtTarget").transform;
    }

    public void InitInteractions()
    {
        mapObjGen = FindObjectOfType<MapObjGen>();
        player = mapObjGen.player;
        behaviour = player.GetComponentInChildren<CharacterBehaviours>();
        lookAnimManager = player.GetComponentInChildren<LookAnimManager>();
        InitializeRadialMenu();
    }

    private void InitializeRadialMenu()
    {
        radialMenu.player = player.transform.gameObject;
        radialMenu.playerWalk = playerWalk;
        radialMenu.lastHit = lastHit;
        radialMenu.behaviours = behaviour;
        radialMenu.areaManager = areaManager;
    }

    Interactable interactable;

    private void HandleMouseDown()
    {
        if (hoverObj == null || behaviour.behaviourIsActive || areaManager.traversing || behaviour.isPsychdelicMode) return;

        interactable = hoverObj.GetComponentInChildren<Interactable>();

        if (IsSelectable(hit.distance) && interactable != null && interactable.enabled)
        {
            if (selectedObj != null)
            {
                RemoveOutline(selectedObj); // Remove outline from previously selected object
            }

            selectedObj = hoverObj; // Set selectedObj only if it's selectable
            lastHit = selectedObj; // Set lastHit to the selected object
            AddOutline(selectedObj, outlineColor); // Add outline to the selected object
            interactable.SpawnMenu(selectedObj, hit);
        }
    }

    private void HandleMouseUp()
    {
        if (selectedObj != null)
        {
            RemoveOutline(selectedObj); // Remove outline from the selected object
            selectedObj = null; // Clear the selected object reference
        }

        // Reset lastHit variable
        lastHit = null;
    }

    private IEnumerator BuildMode()
    {
        yield return null;
    }

    private void AddOutline(GameObject obj, Color color)
    {
        if (obj != null)
        {
            Outline outline = obj.GetComponent<Outline>() ?? obj.AddComponent<Outline>();
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = color;
            outline.OutlineWidth = outlineWidth;
            outline.enabled = true;
        }
    }

    private void AddHoverOutline(GameObject obj)
    {
        if (obj != null)
        {
            // Calculate hover outline color based on outlineColor
            Color hoverOutlineColor = new Color(outlineColor.r * 0.75f, outlineColor.g * 0.75f, outlineColor.b * 0.75f, outlineColor.a * 0.1f);

            outlineComponent = obj.GetComponent<Outline>() ?? obj.AddComponent<Outline>();
            outlineComponent.OutlineMode = Outline.Mode.OutlineVisible;
            outlineComponent.OutlineColor = hoverOutlineColor;
            outlineComponent.OutlineWidth = outlineWidth;
            outlineComponent.enabled = true;
        }
    }

    private void RemoveOutline(GameObject obj)
    {
        if (obj != null)
        {
            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                Destroy(outline);
            }
        }
    }


    private bool IsSelectable(float distance)
    {
        return distance <= maxSelectionDistance && distance >= minSelectionDistance;
    }

    private void Update()
    {
        if (behaviour.behaviourIsActive || behaviour.dialogueIsActive) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer))
        {
            lookAtTarget.position = hit.point;
            GameObject newHoverObj = hit.transform.gameObject;

            if (lastHit == null) // If no object is selected
            {
                // Apply hover outline to new hovered objects
                if (hoverObj != newHoverObj)
                {
                    if (hoverObj != null)
                    {
                        RemoveOutline(hoverObj);
                    }

                    hoverObj = newHoverObj;
                    Interactable interactable = hoverObj.GetComponentInChildren<Interactable>();

                    if (interactable != null && interactable.enabled && IsSelectable(hit.distance))
                    {
                        AddHoverOutline(hoverObj);
                    }
                }
            }
            else if (hoverObj != lastHit) // If an object is selected, maintain its outline
            {
                if (hoverObj != null)
                {
                    RemoveOutline(hoverObj);
                }

                hoverObj = newHoverObj;
            }
        }
        else
        {
            if (hoverObj != null)
            {
                RemoveOutline(hoverObj);
            }
            hoverObj = null;
        }

        UpdateLookAtTarget();

        if (Input.GetMouseButtonDown(1)) HandleMouseDown();
        if (Input.GetMouseButtonUp(1)) HandleMouseUp();
    }



    private void UpdateLookAtTarget()
    {
        if (lookAnimManager != null)
        {
            if (!behaviour.behaviourIsActive && !behaviour.dialogueIsActive && !areaManager.traversing && lookAtTarget != null)
            {
                lookAnimManager.LookAt(lookAtTarget);
            }
            else
            {
                lookAnimManager.DisableLookAt();
            }
        }
    }

    private void OnDestroy()
    {
        if (lookAtTarget != null)
        {
            Destroy(lookAtTarget.gameObject);
        }
    }
}

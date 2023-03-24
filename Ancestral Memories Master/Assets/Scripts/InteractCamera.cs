using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCamera : MonoBehaviour
{
    public GameObject player;

    public LayerMask layer;
    public Vector3 collision = Vector3.zero;
    public GameObject lastHit;
    public Camera cam;

    public PlayerWalk playerWalk;
    public RadialMenu radialMenu;
    public CharacterBehaviours behaviour;

    public AreaManager areaManager;

    public Transform lookAtTarget;
    public Transform defaultTarget;

    private OutlineControl outlineControl;

    Ray ray;

    private void Start()
    {
        cam = player.GetComponent<Player>().interactableCam; 

        radialMenu.player = player;
        radialMenu.playerWalk = playerWalk;
        radialMenu.hitObject = lastHit;
        radialMenu.behaviours = behaviour;
        radialMenu.areaManager = areaManager;
    }

    private bool selected;

    [SerializeField] private float maxSelectionDistance = 5f;
    [SerializeField] private float minSelectionDistance = 0f;

    private GameObject hoverObj;

    public FluteControl fluteControl;

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        
        if (!behaviour.behaviourIsActive || !behaviour.dialogueIsActive)
        {
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
            {
                hoverObj = hit.transform.gameObject;

                if (Input.GetMouseButtonDown(1))
                {
                    if (hoverObj == null)
                    {
                        return;
                    }

                    lastHit = hit.transform.gameObject;
                    outlineControl = lastHit.transform.GetComponentInChildren<OutlineControl>();

                    if (!Input.GetMouseButtonUp(1) && !behaviour.behaviourIsActive && !areaManager.traversing)
                    {
                        if (behaviour.isPsychdelicMode)
                        {
                            return;
                        }

                        if (hit.distance >= maxSelectionDistance || hit.distance <= minSelectionDistance)
                        {
                            Debug.Log("Selection out of range!");
                            return;
                        }

                        selected = true;

                        if (outlineControl != null)
                        {
                            outlineControl.outline.enabled = true;
                        }

                        Interactable interactable = hit.collider.gameObject.GetComponentInParent<Interactable>();

                        if (interactable != null)
                        interactable.SpawnMenu(hoverObj, hit);

                        if (hit.collider.transform.CompareTag("Player"))
                        {
                            //selected = true;
                            lookAtTarget.position = cam.WorldToScreenPoint(hit.point);
                            //Debug.Log(lastHit + "selected");
                        }
                    }

                }


                if (Input.GetMouseButtonUp(1))
                {

                    if (hoverObj == null)
                    {
                        return;
                    }
                    if (outlineControl != null)
                    {
                        outlineControl.outline.enabled = false;
                    }
                }


                if (!behaviour.behaviourIsActive && !behaviour.dialogueIsActive && !areaManager.traversing)
                {
                    lookAtTarget.position = hit.point;
                } else if (behaviour.behaviourIsActive || behaviour.dialogueIsActive || areaManager.traversing) //|| fluteControl.fluteActive)
                {
                    lookAtTarget.position = defaultTarget.transform.position;
                }
            }

        } 
    }
}

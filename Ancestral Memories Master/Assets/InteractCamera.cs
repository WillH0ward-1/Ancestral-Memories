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

    Ray ray;

    private void Start()
    {
        cam = player.GetComponent<Player>().interactableCam;

        radialMenu.player = player;
        radialMenu.playerWalk = playerWalk;
        radialMenu.hitObject = lastHit;
        radialMenu.behaviours = behaviour;

    }

    void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);

        if (!Input.GetMouseButtonDown(0) && !behaviour.behaviourIsActive)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
            {
                lastHit = hit.collider.gameObject;

                if (Input.GetMouseButtonDown(1))
                {
                    Interactable interactable = hit.collider.gameObject.GetComponentInParent<Interactable>();

                    interactable.SpawnMenu(lastHit);

                    Debug.Log(lastHit + "selected");
                }
            }
        }
    }
}

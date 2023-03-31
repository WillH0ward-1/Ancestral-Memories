using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AreaManager : MonoBehaviour
{
    public CharacterBehaviours behaviours;

    public PlayerWalk playerWalk;

    [SerializeField] private Camera currentCam;
    [SerializeField] private Camera newCam;

    public string currentRoom = "";

    private GameObject room;

    [SerializeField] private CamControl cineCam;

    public bool traversing = false;

    private RaycastHit nullHit;

    [SerializeField] private CircleWipeController transition;

    private void Awake()
    {
        traversing = false;
        currentRoom = "Outside";

    }

    public IEnumerator Teleport(NavMeshAgent traveller, Transform targetDestination)
    {
        //cineCam.EnterRoomZoom(tempPortal);

        //transition.FadeOut();

        Portal portal = targetDestination.GetComponentInChildren<Portal>();

        traveller.Warp(portal.enterPortal.transform.position);

        StartCoroutine(ExitPortal(portal));

       // transition.FadeIn();

        yield break;
    }

    public bool isEntering = false;

    public IEnumerator EnterPortal(GameObject interactedPortal)
    {
        behaviours.ToFrontCam();

        traversing = true;
        isEntering = true;

        GameObject enterPortal = FindGameObjectInChildWithTag(interactedPortal, "Portal");
        Portal portal = enterPortal.GetComponent<Portal>();

        GameObject tempPortal = portal.enterPortal.gameObject;

        Transform newDestination = portal.destination;

        playerWalk.StartCoroutine(playerWalk.WalkToward(portal.enterPortal.gameObject, "Enter", newDestination.transform, nullHit));

        yield break;
    }

    public IEnumerator ExitPortal(Portal portal)
    {
        behaviours.ToFrontCam();

        string area = portal.exitPortal.GetComponent<AreaName>().areaName;
        currentRoom = area;

        playerWalk.CheckAreaForFootSteps(currentRoom);

        isEntering = false;

        playerWalk.StartCoroutine(playerWalk.WalkToward(portal.exitPortal.gameObject, "Exit", null, nullHit));

        //cineCam.ToGameZoom();

        yield break;
    }

    public GameObject FindGameObjectInChildWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;

        for (int i = 0; i < t.childCount; i++)
        {
            if (t.GetChild(i).gameObject.tag == tag)
            {
                return t.GetChild(i).gameObject;
            }

        }

        return null;
    }
}

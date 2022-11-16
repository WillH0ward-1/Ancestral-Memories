using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AreaManager : MonoBehaviour
{
    public CharacterBehaviours behaviours;

    public PlayerWalk playerWalk;

    [SerializeField] private Transform walkTarget;

    [SerializeField] private Camera currentCam;
    [SerializeField] private Camera newCam;

    private enum Room {Outside, InsideCave};

    private Room currentRoom;

    private GameObject room;

    public bool traversing = false;

    private void Start()
    {
        traversing = false;
        currentRoom = Room.Outside;
    }

    public IEnumerator Teleport(NavMeshAgent traveller, Transform targetDestination)
    {

        Portal portal = targetDestination.GetComponentInChildren<Portal>();

        traveller.Warp(portal.enterPortal.transform.position);

        StartCoroutine(ExitPortal(portal));

        yield return new WaitUntil(() => !traversing);

        yield break;
    }

    public bool isEntering = false;

    public IEnumerator EnterPortal(GameObject interactedPortal)
    {
        traversing = true;

        isEntering = true;

        GameObject enterPortal = FindGameObjectInChildWithTag(interactedPortal, "Portal");
        Portal portal = enterPortal.GetComponent<Portal>();

        Transform newDestination = portal.destination;

        StartCoroutine(playerWalk.WalkToward(portal.enterPortal.gameObject, "Enter Portal", newDestination.transform));

        yield return new WaitUntil(() => !traversing);

        yield break;
    }

    public IEnumerator ExitPortal(Portal portal)
    {
        isEntering = false;

        StartCoroutine(playerWalk.WalkToward(portal.exitPortal.gameObject, "Exit Portal", null));

        yield return new WaitUntil(() => playerWalk.reachedDestination);

        traversing = false;

        yield return new WaitUntil(() => !traversing);

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

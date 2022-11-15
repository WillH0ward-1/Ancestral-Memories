using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool isEnteringRoom = false;
    public bool isExitingRoom = false;

    public bool traversing = false;

    private void Start()
    {
        currentRoom = Room.Outside;
    }
    private void SwitchCam()
    {
        currentCam.enabled = false;
        newCam.enabled = true;
    }

    public IEnumerator Teleport(GameObject traveller, Transform target)
    {
        traveller.transform.position = target.transform.position;

        yield return new WaitForSeconds(3f);

        StartCoroutine(ExitPortal(target.gameObject));

        yield return new WaitUntil(() => !traversing);

        yield return null;
    }

    private void Update()
    {
        if (isEnteringRoom || isExitingRoom)
        {
            traversing = true;
        } else
        {
            traversing = false;
        }
    }

    public IEnumerator EnterPortal(GameObject interactedPortal)
    {
        GameObject enterPortal = FindGameObjectInChildWithTag(interactedPortal, "Portal");
        Portal portal = enterPortal.GetComponent<Portal>();

        StartCoroutine(playerWalk.WalkToward(portal.enterPortal.gameObject, "Enter Portal", enterPortal, null));

        while (traversing)
        {
            isEnteringRoom = true;
            isExitingRoom = false;

            yield return null;
        }

        if (!traversing)
        {
            yield break;
        }
    }

    public IEnumerator ExitPortal(GameObject newAreaPortal)
    {
        while (traversing)
        {
            GameObject exitPortal = FindGameObjectInChildWithTag(newAreaPortal, "ExitPortal");

            Portal portal = exitPortal.GetComponent<Portal>();

            isExitingRoom = true;
            isEnteringRoom = false;

            StartCoroutine(playerWalk.WalkToward(portal.exitPortal.gameObject, "Exit Portal", exitPortal, null));
            yield return null;
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    public CharacterBehaviours behaviours;

    public PlayerWalk playerWalk;
    [SerializeField] private Transform target;

    [SerializeField] private GameObject player;

    [SerializeField] private Camera currentCam;
    [SerializeField] private Camera newCam;

    private Transform teleportTarget;

    private enum Room {Outside, InsideCave};

    private Room currentRoom;

    private void Start()
    {
        currentRoom = Room.Outside;
    }
    private void SwitchCam()
    {
        currentCam.enabled = false;
        newCam.enabled = true;
    }

    public IEnumerator WalkTowardNewRoom()
    {
        if (currentRoom == Room.Outside)
        {

        }

        StartCoroutine(playerWalk.WalkToward(target.gameObject, "Inside"));
        yield return null;
    }

    public IEnumerator WalkIntoNewRoom()
    {
        player.transform.LookAt(target);


        StartCoroutine(playerWalk.WalkToward(target.gameObject, "Inside"));
        yield return null;
    }

    public void Teleport(Transform target)
    {
        teleportTarget = target;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterRoom : MonoBehaviour
{
    public CharacterBehaviours behaviours;

    public PlayerWalk playerWalk;
    [SerializeField] private Transform destination;
    [SerializeField] private Transform walkOutOfRoomTarget;
    [SerializeField] private GameObject player;


    [SerializeField] private Camera currentCam;
    [SerializeField] private Camera newCam;

    private void SwitchCam()
    {
        currentCam.enabled = false;
        newCam.enabled = true;
    }

    public IEnumerator Teleport(Vector3 destination)
    {
        yield return new WaitForSeconds(1);
        player.transform.position = destination;
        player.transform.LookAt(walkOutOfRoomTarget);
       // StartCoroutine(behaviours.WalkIntoRoom());
    }
}

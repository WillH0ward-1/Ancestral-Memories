using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterRoom : MonoBehaviour
{
    public CharacterBehaviours behaviours;

    public PlayerWalk playerWalk;
    [SerializeField] private Transform teleporter;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform walkIndoorTransform;

    [SerializeField] private Camera currentCam;
    [SerializeField] private Camera newCam;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other + "Entered!");
        behaviours.StartCoroutine(behaviours.WalkIntoRoom(walkIndoorTransform));
        
        //SwitchCam();
    }

    private void SwitchCam()
    {
        currentCam.enabled = false;
        newCam.enabled = true;
    }

    public IEnumerator Teleport(Transform walkTowardRoom)
    {
        yield return new WaitForSeconds(1);
        player.transform.position = new Vector3(teleporter.position.x, teleporter.position.y, teleporter.position.z);
        player.transform.LookAt(walkTowardRoom);
    }
}

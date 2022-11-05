using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    public PortalScript[] Portals;

    public void Start()
    {
        RoomController.staticRef.SetupRoom(this);
    }

    public void OnTriggerExit(Collider other)
    {
        Destroy(gameObject);
    }
}

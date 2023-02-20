using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestinationGizmo : MonoBehaviour
{
    public bool hitDestination = false;

    public bool OnTriggerEnter(Collider other)
    {

        if (other.transform.CompareTag("Player"))
        {
            hitDestination = true;
            return true;
        }
        else
        {

            hitDestination = false;
            return false;
        }
    }


}

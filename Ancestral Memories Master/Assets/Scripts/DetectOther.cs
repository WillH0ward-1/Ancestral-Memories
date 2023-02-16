using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectOther : MonoBehaviour
{
    void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.transform.CompareTag("WindZone")){
            Destroy(other.transform.gameObject);
        }
    }
}

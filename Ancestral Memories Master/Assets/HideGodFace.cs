using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGodFace : MonoBehaviour
{
    public bool GodSpeaking;

    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("GodFaceFire"))
        {
            gameObject.GetComponent<Renderer>().enabled = false;
            other.transform.GetComponent<Renderer>().enabled = true;
        } 
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("GodFaceFire"))
        {
            gameObject.GetComponent<Renderer>().enabled = true;
            other.transform.GetComponent<Renderer>().enabled = false;
        }
    }

}

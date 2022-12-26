using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGodFace : MonoBehaviour
{
    public bool outOfRange = false;
    private Dialogue dialogue;

    private Renderer meshRenderer;
    // Start is called before the first frame update
    private void Start()
    {
        meshRenderer = gameObject.GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        outOfRange = false;
        dialogue = other.transform.root.GetComponent<Dialogue>();

        if (other.transform.CompareTag("GodFaceFire"))
        {
            meshRenderer.enabled = false;
        } 
    }

    private void Update()
    {
        if (dialogue != null)
        {
            if (!dialogue.dialogueIsActive)
            {
                meshRenderer.enabled = false;
            }
        } else
        {
            return;
        }
    }

    void OnTriggerExit(Collider other)
    {
        outOfRange = true;

        if (other.transform.CompareTag("GodFaceFire"))
        {
            gameObject.GetComponent<Renderer>().enabled = true;
        }
    }

}

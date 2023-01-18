using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideGodFace : MonoBehaviour
{
    public bool outOfRange = false;
    private Dialogue dialogue;

    private Renderer meshRenderer;

    [SerializeField] public Transform target;

    [SerializeField] private Camera mainCam;

    // Start is called before the first frame update
    private void Start()
    {
        target = mainCam.transform;
        meshRenderer = gameObject.GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("GodFaceFire"))
        {
            outOfRange = false;
            dialogue = other.transform.root.GetComponent<Dialogue>();
        } 
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("GodFaceFire"))
        {
            outOfRange = true;
        }
    }

    private void Update()
    {
        gameObject.transform.LookAt(target.position);

        if (dialogue.dialogueActive && outOfRange)
        {
            meshRenderer.enabled = true;
        }
        else if (!dialogue.dialogueActive)
        {

            meshRenderer.enabled = false;

        }
    }

}

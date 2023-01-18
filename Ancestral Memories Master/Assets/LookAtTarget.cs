using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] public Transform target;
    float y;
    private Renderer meshRenderer;

    private Dialogue dialogue;

    private float distanceFromFire;

    [SerializeField] private bool inRange;

    private void Start()
    {
        meshRenderer = transform.GetComponent<Renderer>();
        meshRenderer.enabled = false;

        y = gameObject.transform.rotation.eulerAngles.y;
        y += 180;
        dialogue = transform.root.GetComponent<Dialogue>();

    }

    void Update()
    { 
        gameObject.transform.LookAt(target.position);

        if (!dialogue.dialogueActive)
        {
            meshRenderer.enabled = false;
        }
        else if (dialogue.dialogueActive && inRange)
        {

            meshRenderer.enabled = true;

        } else { 
            meshRenderer.enabled = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            inRange = true;
            target = other.transform.GetComponent<CharacterBehaviours>().cinematicCam.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            inRange = false;
            meshRenderer.enabled = false;
        }
    }
}

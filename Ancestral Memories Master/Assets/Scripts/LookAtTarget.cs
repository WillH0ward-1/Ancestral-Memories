using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    private Dialogue dialogue;

    public Player player;

    [SerializeField] private bool inRange;


    public Transform godFaceInRange;
    public Transform godFaceOutOfRange;

    [SerializeField] private Renderer inRangeGodRenderer;
    [SerializeField] private Renderer outRangeGodRenderer;

    public float inRangeThreshold = 10;

    private void Start()
    {

        dialogue = transform.root.GetComponent<Dialogue>();

        inRangeGodRenderer = transform.GetComponent<Renderer>();
        outRangeGodRenderer = godFaceInRange.GetComponent<Renderer>();
    }

    void Update()
    {
    
        if (!dialogue.dialogueActive)
        {
            inRangeGodRenderer.enabled = false;
            outRangeGodRenderer.enabled = false;
        }
        else if (dialogue.dialogueActive && InRange())
        {

            inRangeGodRenderer.enabled = true;
            outRangeGodRenderer.enabled = false;

        }
        else if (dialogue.dialogueActive && !InRange())
        {

            inRangeGodRenderer.enabled = false;
            outRangeGodRenderer.enabled = true;

        }
        else {
            inRangeGodRenderer.enabled = false;
            outRangeGodRenderer.enabled = false;
        }
    }

    private bool InRange()
    {
        bool inRange;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= inRangeThreshold)
        {
            inRange = true;
        } else 
        {
            inRange = false;
        }

        return inRange;
    }
}

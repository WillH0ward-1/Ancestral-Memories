using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodRangeController : MonoBehaviour
{
    public Dialogue dialogue;

    public Player player;

    [SerializeField] private bool inRange;


    [SerializeField] private Renderer inRangeGodRenderer;
    [SerializeField] private Renderer outRangeGodRenderer;

    public float inRangeThreshold = 10;

    private void Awake()
    {

        inRangeGodRenderer = inRangeGodRenderer.GetComponent<Renderer>();
        outRangeGodRenderer = outRangeGodRenderer.GetComponent<Renderer>();

        inRangeGodRenderer.enabled = false;
        outRangeGodRenderer.enabled = false;
    }

    public IEnumerator UpdateActiveStates()
    {
        while (true)
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
            else
            {
                inRangeGodRenderer.enabled = false;
                outRangeGodRenderer.enabled = false;
            }

            yield return null;
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

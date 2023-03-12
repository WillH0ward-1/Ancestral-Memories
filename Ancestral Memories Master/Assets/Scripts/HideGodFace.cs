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

    public Transform godFire;

    // Start is called before the first frame update
    private void Start()
    {
        target = mainCam.transform;
        meshRenderer = gameObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        gameObject.transform.LookAt(target.position);

        if (dialogue != null)
        {
            if (dialogue.dialogueActive && outOfRange)
            {
                meshRenderer.enabled = true;
            }
            else if (!dialogue.dialogueActive)
            {

                meshRenderer.enabled = false;

            }
        } else
        {
            return;
        }
    }

}

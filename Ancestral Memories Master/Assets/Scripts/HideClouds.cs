using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideClouds : MonoBehaviour
{
    [SerializeField] private CharacterBehaviours behaviours;
    [SerializeField] private CamControl camControl;

    private Renderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = gameObject.GetComponent<Renderer>();
    }

    public void TurnOffClouds()
    {
        meshRenderer.enabled = false;
    }

    public void TurnOnClouds()
    {
        meshRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (camControl.currentZoom <= 0) {
            meshRenderer.enabled = false;
        } else if (camControl.currentZoom > 0)
        {
            meshRenderer.enabled = true;
        }
    }
}

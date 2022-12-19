using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideClouds : MonoBehaviour
{
    [SerializeField] private CharacterBehaviours behaviours;
    [SerializeField] private Camera cinematicCam;
    private Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
    }

    public void TurnOffClouds()
    {
        renderer.enabled = false;
    }

    public void TurnOnClouds()
    {
        renderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (behaviours.isPsychdelicMode) {
            renderer.enabled = false;
        } else if (!behaviours.isPsychdelicMode)
        {
            renderer.enabled = true;
        }
    }
}

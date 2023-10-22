using System.Collections;
using System.Collections.Generic;
// using FMOD.Studio;
using UnityEngine;


public class FlipbookAudio : MonoBehaviour
{
    // public EventInstance flipBookSFXInstance; 
    public float playbackSpeed = 12f;

    private Material flipbookMaterial; // reference to the flipbook material
    private float timeOffset; // offset used to synchronize audio with flipbook animation

    Renderer renderer;

    private void Start()
    {
        renderer = transform.GetComponent<Renderer>();

        if (renderer != null)
        {
            flipbookMaterial = renderer.material;
        }

        timeOffset = 0f;
    }

    private void Update()
    {
        if (renderer.enabled && flipbookMaterial != null)
        {
            float frame = (Time.time + timeOffset) * playbackSpeed % 1f;
            flipbookMaterial.SetFloat("_animSpeed", frame);

            /*
            flipBookSFXInstance.start();
            flipBookSFXInstance.release();
            */
        }
    }
}

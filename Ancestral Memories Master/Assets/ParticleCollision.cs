using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ParticleCollision : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Player player;

    [SerializeField] private string rainSFX;
    private EventInstance instance;

    [SerializeField] private float stability = 0;
    [SerializeField] private float instability = 1;

    private float harmonicStability;

    private float currentWindStrength = 0;
    private float targetHarmonicStability;

    public bool windIsActive;

    private StudioGlobalParameterTrigger globalParams;

    public LayerMask groundLayerMask;

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle hit ground!");

        var ray = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, groundLayerMask);

        Vector3 hitLocation = hitFloor.point;

        Vector3 screenCoords = cam.WorldToViewportPoint(hitLocation);

        bool onScreen =
            screenCoords.x > 0 &&
            screenCoords.x < 1 &&
            screenCoords.y > 0 &&
            screenCoords.y < 1;

        // Credit for above: ScottsGameSounds

        if (onScreen)
        {
            //EventInstance rainDropInstance = emitter.EventInstance;
            RuntimeManager.PlayOneShot(rainSFX, hitLocation);

            //emitter.EventInstance.start();
            //emitter.EventInstance.release();
           
            RuntimeManager.StudioSystem.setParameterByName("HarmonicStability", targetHarmonicStability);

            //instance = RuntimeManager.CreateInstance(rainSFX);
            //instance.start();

         
        }

        //lightningStrikeEvent.setVolume();
    }

    private bool rainIsActive = false;

    private bool harmonicStabilityActive;

    private void Awake()
    {
        StartCoroutine(HarmonicStability());

    }

    private IEnumerator HarmonicStability()
    {
        harmonicStabilityActive = true;

        while (harmonicStabilityActive)
        {
            if (!harmonicStabilityActive)
            {
                yield break;
            }

            harmonicStability = targetHarmonicStability;

          // RuntimeManager.StudioSystem.setParameterByName("HarmonicStability", targetHarmonicStability);
            yield return null;
        }
  
        yield break;

    }


    private void OnEnable() => player.OnFaithChanged += HarmonicStability;
    private void OnDisable() => player.OnFaithChanged -= HarmonicStability;

    private void HarmonicStability(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float output = Mathf.Lerp(instability, stability, t);

        targetHarmonicStability = output;



    }
}

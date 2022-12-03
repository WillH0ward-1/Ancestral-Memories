using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ParticleCollision : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Player player;

    [SerializeField] private StudioEventEmitter emitter;

    [SerializeField] private EventReference rainSFX;

    private int stability = 0;
    private int instability = 1;

    private float currentWindStrength = 0;
    private float targetHarmonicStability;

    public bool windIsActive;

    private StudioGlobalParameterTrigger globalParams;

    private void Awake()
    {
        emitter.EventInstance.setParameterByName("HarmonicStability", targetHarmonicStability);
    }


    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle hit ground!");

        int groundLayerIndex = LayerMask.NameToLayer("Ground");
        int groundLayerMask = (1 << groundLayerIndex);

        var ray = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, groundLayerMask);

        Vector3 hitLocation = hitFloor.point;

        Vector3 screenCoords = cam.WorldToViewportPoint(hitLocation);

        bool onScreen =
            screenCoords.x > 0 &&
            screenCoords.x < 1 &&
            screenCoords.y > 0 &&
            screenCoords.y < 1;

        if (onScreen)
        {
            //rainDropInstance = RuntimeManager.CreateInstance(rainSFX);
           // rainDropInstance.start();
            emitter.EventInstance.start();
            //emitter.EventInstance.setParameterByName("HarmonicStability", targetHarmonicStability);
        }

        //lightningStrikeEvent.setVolume();
    }

    private void OnEnable() => player.OnFaithChanged += HarmonicStability;
    private void OnDisable() => player.OnFaithChanged -= HarmonicStability;

    private void HarmonicStability(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float output = Mathf.Lerp(stability, instability, t);

        targetHarmonicStability = output;

        emitter.EventInstance.setParameterByName("HarmonicStability", targetHarmonicStability);
    }
}

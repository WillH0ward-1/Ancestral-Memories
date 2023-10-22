using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using FMOD;
//using FMODUnity;

public class RayDetectReverbZone : MonoBehaviour
{
    public ReverbManager reverbManager;

    [SerializeField] private GameObject rayOrigin;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float sphereCastRadius = 1f;

    [SerializeField] private bool inRange = false;
    [SerializeField] private bool castActive = false;

    [SerializeField] float intensityRef;
    [SerializeField] float distanceFromRayOriginRef;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ReverbZone"))
        {
            inRange = true;

            if (!castActive)
            {
                StartCoroutine(CastRay());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ReverbZone"))
        {
            inRange = false;
            castActive = false;
            // reverbManager.emitter.EventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private IEnumerator CastRay()
    {
        castActive = true;

        while (inRange)
        {
            if (Physics.SphereCast(rayOrigin.transform.position, sphereCastRadius, Vector3.forward, out RaycastHit rayHit, layer))
            {
                /*
                reverbManager.emitter.EventInstance.start();
                reverbManager.emitter.EventInstance.release();
                */

                float distance = Vector3.Distance(rayOrigin.transform.position, rayHit.transform.position);
                float intensity = CalculateReverbIntensity(rayOrigin.transform.position, rayHit.transform.position);

                intensityRef = intensity;
                distanceFromRayOriginRef = distance;

                // reverbManager.emitter.EventInstance.setParameterByName("Intensity", intensity);
            }

            yield return null;
        }

        yield break;
    }

    private float CalculateReverbIntensity(Vector3 rayOrigin, Vector3 rayHit)
    {
        float maxDistance = Vector3.Distance(rayOrigin, rayHit);
        float origin = Vector3.Distance(rayOrigin, rayOrigin);
        float distance = Vector3.Distance(rayOrigin, rayHit);
        float t = Mathf.InverseLerp(origin, maxDistance, distance);
        float intensity = Mathf.Lerp(0, 1, t);

        return intensity;
    }
}
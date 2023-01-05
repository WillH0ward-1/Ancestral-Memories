using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class RayDetectReverbZone : MonoBehaviour
{

    private ReverbManager reverbManager;

    private PlayerSoundEffects playerSFX;
    
    [SerializeField] private GameObject attenuationObject;
    [SerializeField] private GameObject reverbReflectorEmitter;

    [SerializeField] private LayerMask layer;
    [SerializeField] private bool inRange;
    [SerializeField] private float distance;

    [SerializeField] private bool raycasting = false;

    float minDistance = 0;
    float maxDistance = 1;

    float reverbIntensityMin = 0;
    float reverbIntensityMax = 1;

    float targetIntensity = 0;

    private void Awake()
    {
        raycasting = false;
        attenuationObject = transform.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("NearReverbZone"))
        {
            inRange = true;

            if (!raycasting)
            {
                StartCoroutine(CastRay());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("NearReverbZone"))
        {
            inRange = false;
            raycasting = false;
        }
    }

    private IEnumerator CastRay()
    {
        raycasting = true;

        while (inRange)
        {
            float x1 = attenuationObject.transform.localPosition.x;
            float z1 = attenuationObject.transform.localPosition.z;

            float x2 = reverbReflectorEmitter.transform.localPosition.x;
            float z2 = reverbReflectorEmitter.transform.localPosition.z;


            distance = Vector2.Distance(new Vector2(x1, z1), new Vector2(x2, z2));

            if (Physics.Raycast(transform.position, Vector3.forward, out RaycastHit rayHit, Mathf.Infinity, layer))
            {
                Vector3 attenuationObjectPos = attenuationObject.transform.position;

                var t = Mathf.InverseLerp(minDistance, maxDistance, distance);
                float output = Mathf.Lerp(reverbIntensityMin, reverbIntensityMax, t);
                targetIntensity = output;

                reverbManager.reverbInstance.setParameterByName("ReverbIntensity", targetIntensity);
            }

            yield return null;
        }

        yield break;
    }
}

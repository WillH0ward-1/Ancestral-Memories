using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class RayDetectReverbZone : MonoBehaviour
{
    private ReverbManager reverbManager;

    private PlayerSoundEffects playerSFX;


    [SerializeField] private GameObject rayOrigin;
    [SerializeField] private GameObject reverbZoneWall;

    [SerializeField] private LayerMask layer;
    [SerializeField] private bool inRange;
    [SerializeField] private float distance;

    [SerializeField] private bool castActive = false;

    float minDistance = 0;
    float maxDistance = 1;

    float reverbIntensityMin = 0;
    float reverbIntensityMax = 1;

    float targetIntensity = 0;

    private void Awake()
    {
        castActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("ReverbZone"))
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
        if (other.transform.CompareTag("ReverbZone"))
        {
            inRange = false;
            castActive = false;
        }
    }

    [SerializeField] private float sphereCastRadius = 1;

    private IEnumerator CastRay()
    {
        castActive = true;

        while (inRange)
        {
            if (Physics.SphereCast(transform.position, sphereCastRadius, Vector3.forward, out RaycastHit rayHit, layer))
            {
                //Gizmos.DrawSp(transform.position, Vector3.forward);

                print(rayHit.transform);

                distance = Vector3.Distance(rayOrigin.transform.position, rayHit.transform.position);

                float rayHitPosX = rayHit.transform.position.x;
                float rayHitPosY = rayHit.transform.position.y;
                float rayHitPosZ = rayHit.transform.position.z;

                float rayOriginX = rayOrigin.transform.position.x;
                float rayOriginY = rayOrigin.transform.position.y;
                float rayOriginZ = rayOrigin.transform.position.z;

                rayHitPosX -= rayOriginX;
                rayHitPosY -= rayOriginY;
                rayHitPosZ -= rayOriginZ;

                Vector3 rayPos = new(rayHitPosX, rayHitPosY, rayHitPosZ);

                var t = Mathf.InverseLerp(0, distance, distance);
                float output = Mathf.Lerp(reverbIntensityMin, reverbIntensityMax, t);
                targetIntensity = output;

                reverbManager.reverbInstance.setParameterByName("ReverbIntensity", targetIntensity);
            }

            yield return null;
        }

        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using System.Linq;

public class PlayFlute : MonoBehaviour
{
    [SerializeField] private float distance;

    [SerializeField] private GameObject attenuationObject;

    [SerializeField] private Player player;
    [SerializeField] private CharacterBehaviours behaviours;
    [SerializeField] private PlayerSoundEffects playerSFX;

    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private bool fluteActive = false;

    [SerializeField] private float targetFluteScale;
    [SerializeField] private float minFluteScale = 0;
    [SerializeField] private float maxFluteScale = 6;

    [SerializeField] private float minDistance = 0;
    [SerializeField] private float maxDistance = 100;

    bool isPlaying = false;

    [SerializeField] private float interval = 0;
    [SerializeField] private float maxInterval = 0.1f;
    /*
    private void Awake()
    {
        //attenuationObject = transform.root.gameObject;
    }
    */

    public void EnableFluteControl()
    {
        StartCoroutine(CastRayToGround());
    }

    public IEnumerator CastRayToGround()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        while (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") != 0 && Input.GetAxis("Mouse Y") != 0)
            {
                interval = 0;

                while (interval <= maxInterval)
                {
                    interval += Time.deltaTime;

                    yield return null;
                }
            }

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (!fluteActive && interval >= maxInterval)
            {
                PlayFluteSound();
            }

            if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, targetLayer))
            {

                Vector3 attenuationObjectPos = attenuationObject.transform.position;

                distance = Vector3.Distance(attenuationObjectPos, rayHit.point);
                var t = Mathf.InverseLerp(minDistance, maxDistance, distance);
                float output = Mathf.Lerp(minFluteScale, maxFluteScale, t);
                targetFluteScale = (int)Mathf.Floor(output);

                playerSFX.fluteEventRef.setParameterByName("FluteScale", targetFluteScale);

            }

            yield return null;
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopFluteSound();
            StartCoroutine(CastRayToGround());
            yield break;
        }

    }

    private IEnumerator NoteDelay()
    {
        interval = 0;

        while (interval <= maxInterval && Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            interval += Time.deltaTime;
            yield return null;
        }

        yield break;
    }

    void PlayFluteSound()
    {
        fluteActive = true;
        playerSFX.PlayFluteEvent();
    }

    void StopFluteSound()
    {
        fluteActive = false;
        playerSFX.fluteEventRef.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StopAll()
    {
        StopFluteSound();
        StopAllCoroutines();
        fluteActive = false;
    }
}
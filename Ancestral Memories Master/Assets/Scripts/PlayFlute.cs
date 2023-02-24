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

    public bool fluteActive = false;

    [SerializeField] private float targetFluteScale;
    [SerializeField] private float minFluteScale = 0;
    [SerializeField] private float maxFluteScale = 6;

    [SerializeField] private float minDistance = 0;
    [SerializeField] private float maxDistance = 100;

    bool isPlaying = false;

    [SerializeField] private float interval = 0;
    private float initInterval = 0;
    private float activeInterval = 0.1f;

    [SerializeField] private float maxInterval = 0.1f;

    [SerializeField] private float faithFactor = 0.25f;

    /*
    private void Awake()
    {
        //attenuationObject = transform.root.gameObject;
    }
    */

    private Vector2 screenCenter;
 
    private void Awake()
    {
        screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    public void EnableFluteControl()
    {
        StartCoroutine(CastRayToGround());
    }


    [SerializeField] float tValRef;
    [SerializeField] float fluteScaleOutputRef;

    public IEnumerator CastRayToGround()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        maxInterval = initInterval;

        while (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") != 0 && Input.GetAxis("Mouse Y") != 0)
            {
                interval = 0;
                maxInterval = activeInterval;

                while (interval <= maxInterval)
                {
                    interval += Time.deltaTime;

                    yield return null;
                }
            }


            if (!fluteActive && interval >= maxInterval)
            {
                PlayFluteSound();
            }

            /*
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, targetLayer))
            {
            */

            // Vector3 attenuationObjectPos = attenuationObject.transform.position;

            distance = Vector2.Distance(screenCenter, Input.mousePosition);


            var t = Mathf.InverseLerp(minDistance, maxDistance, distance);
            tValRef = t;

            float output = Mathf.Lerp(minFluteScale, maxFluteScale, t);
            fluteScaleOutputRef = output;

            targetFluteScale = (int)Mathf.Floor(output);

            if (targetFluteScale >= maxFluteScale)
                targetFluteScale = maxFluteScale;
            if (targetFluteScale <= minFluteScale)
                targetFluteScale = minFluteScale;

            playerSFX.fluteEventRef.setParameterByName("FluteScale", targetFluteScale);
            

            
            //}

            player.GainFaith(faithFactor);

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
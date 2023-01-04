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
    [SerializeField] private float maxFluteScale = 5;

    [SerializeField] private float minDistance = 0;
    [SerializeField] private float maxDistance = 100;

    bool isPlaying = false;

    /*
    private void Awake()
    {
        //attenuationObject = transform.root.gameObject;
    }
    */

    public void EnableFluteControl()
    {
        StartCoroutine(CastRayToGround());

        return;
    }

    public IEnumerator CastRayToGround()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        while (Input.GetMouseButton(0))
        {
            if (!fluteActive)
            {
                PlayFluteSound();
            }

            if (Physics.Raycast(ray, out RaycastHit rayHit, Mathf.Infinity, targetLayer))
            { 
                Vector3 attenuationObjectPos = attenuationObject.transform.position;

                distance = Vector3.Distance(rayHit.point, attenuationObjectPos);
                var t = Mathf.InverseLerp(minDistance, maxDistance, distance);
                float output = (int)Mathf.Lerp(minFluteScale, maxFluteScale, t);
                targetFluteScale = Mathf.CeilToInt(output);

                RuntimeManager.StudioSystem.setParameterByName("FluteScale", targetFluteScale);                    

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

    void StopFluteSound()
    {
        fluteActive = false;
        playerSFX.fluteEventRef.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT); 
    }

    void PlayFluteSound()
    {
        fluteActive = true;
        playerSFX.PlayFluteEvent();
    }
}
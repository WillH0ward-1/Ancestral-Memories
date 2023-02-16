using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class PsychedelicWarp : MonoBehaviour
{
    [SerializeField] private float heatDistortion = 0f;
    [SerializeField] private float maskPower = 0f;
    [SerializeField] private float currentDistortion = 0f;
    [SerializeField] private float targetDistortion = 0f;

    [SerializeField] private float currentMaskPower = 0f;
    [SerializeField] private float targetMaskPower = 0f;

    private CharacterBehaviours behaviours;
    private CamControl camControl;

    private Renderer renderer;
    bool warpFinished = false;

    private float quadHeight;
    private float quadWidth;

    void Start()
    {
        renderer = transform.GetComponent<Renderer>();
        maskPower = renderer.sharedMaterial.GetFloat("_MaskPower");
        heatDistortion = renderer.sharedMaterial.GetFloat("_HeatDistortion");

        maskPower = 0;
        heatDistortion = 0;
        warpFinished = true;
    }

    private void Update()
    {
        quadHeight = (float)(camControl.currentOrthoZoom * 2.0);
        quadWidth = quadHeight * Screen.width / Screen.height;

        transform.parent.localScale = new Vector3(quadWidth, quadHeight, 1);

        if (behaviours.isPsychdelicMode)
        {
            if (warpFinished)
            {
                StartCoroutine(Warp());
            }
            return;
        }  
    }


    private IEnumerator Warp()
    {
        warpFinished = false;

        float time = 0f;
        float duration = camControl.toPsychedelicZoomDuration;

        while (behaviours.isPsychdelicMode)
        {
            targetDistortion = 0.125f;
            targetMaskPower = -16f;

            time += Time.deltaTime / duration;

            maskPower = Mathf.Lerp(maskPower, targetMaskPower, duration);
            heatDistortion = Mathf.Lerp(heatDistortion, targetDistortion, duration);

            yield return null;
        }

        if (!behaviours.isPsychdelicMode)
        {
            targetDistortion = 0;
            targetMaskPower = 0;
            StartCoroutine(StopWarp(duration));
            yield break;
        }
    }

    private IEnumerator StopWarp(float duration)
    {

        currentDistortion = 0;
        currentMaskPower = 0;
        float time = 0;


        while (time <= 1f)
        {
            time += Time.deltaTime / duration;

            maskPower = Mathf.Lerp(maskPower, targetMaskPower, duration);
            heatDistortion = Mathf.Lerp(heatDistortion, targetDistortion, duration);

            yield return null;
        }

        warpFinished = true;
        yield break;
    }
}

/*
 * 
 * Remap output from player faith value 

    float minDistortion = 0;
    float maxDistortion = 250;

    float minMaskPower = 0;
    float maxMaskPower = 250;

    [SerializeField]
    private CharacterClass player;

    private void OnEnable() => player.OnFaithChanged += WarpFactor;
    private void OnDisable() => player.OnFaithChanged -= WarpFactor;

    private void WarpFactor(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);

        float distortOutput = Mathf.Lerp(maxDistortion, minDistortion, t);
        float maskOutput = Mathf.Lerp(minMaskPower, maxMaskPower, t);

        targetDistortion = distortOutput;
        targetMaskPower = maskOutput;
    }

}

*/



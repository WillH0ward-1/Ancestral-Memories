using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GammaControl : MonoBehaviour
{
    public Volume volume;
    private ColorAdjustments colorAdjustments;
    private float minContrast = -14;
    private float maxContrast = 1f;

    private float minSaturation = 78f;
    private float maxSaturation = 100f;

    public Player player;

    private void Start()
    {
        if (!volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.LogError("ColorAdjustments not found in the Volume Profile!");
        }

        colorAdjustments.saturation.value = maxContrast;
    }


    private void OnEnable()
    {
        player.OnPsychChanged += UpdatePsych;
    }

    private void OnDisable()
    {
        player.OnPsychChanged -= UpdatePsych;
    }

    private void UpdatePsych(float psych, float minPsych, float maxPsych)
    {
        float t = Mathf.InverseLerp(minPsych, maxPsych, psych);

        float contrast = Mathf.Lerp(minContrast, maxContrast, t);
        float saturation = Mathf.Lerp(minSaturation, maxSaturation, t);

        colorAdjustments.contrast.value = contrast;
        colorAdjustments.saturation.value = saturation;
    }
}
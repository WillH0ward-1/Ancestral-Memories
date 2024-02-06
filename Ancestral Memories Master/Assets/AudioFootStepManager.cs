using System.Collections;
using UnityEngine;

public class AudioFootStepManager : MonoBehaviour
{
    // Parameter names as constants
    private const string OneStepParam = "OneStep";
    private const string SneakParam = "Sneak";
    private const string AttackParam = "ADSR_Attack";
    private const string DecayParam = "ADSR_Decay";
    private const string SustainParam = "ADSR_Sustain";
    private const string ReleaseParam = "ADSR_Release";
    private const string GrassParam = "Grass";
    private const string WoodParam = "Wood";
    private const string StoneParam = "Stone";
    private const string MudMixParam = "MudMix";
    private const string WaterParam = "Water";
    private const string SnowParam = "Snow";
    private const string PitchParam = "Pitch";
    private const string ForceParam = "Force";
    private const string HeelParam = "Heel";
    private const string HeelMixerParam = "HeelMixer";
    private const string HeelRandParam = "HeelRand";
    private const string MaterialParam = "Material";
    private const string WetnessParam = "Wetness";
    private const string RoughnessParam = "Roughness";
    private const string GrassLengthParam = "GrassLength";
    private const string WaterDepthParam = "WaterDepth";

    private float currentWood;
    private float currentStone;
    private float currentMudMix;
    private float currentWater;
    private float currentSnow;

    private void Start()
    {
        CabbageAudioManager.Instance.SetTrigger(OneStepParam, false); 
        CabbageAudioManager.Instance.SetTrigger(SneakParam, false); 
    }

    public void TriggerFootstep()
    {
        CabbageAudioManager.Instance.SetTrigger(OneStepParam, true);
    }

    public void SetSneak(bool enabled)
    {
        CabbageAudioManager.Instance.SetTrigger(SneakParam, enabled);
    }

    public void SetADSR(float attack, float decay, float sustain, float release)
    {
        CabbageAudioManager.Instance.SetParameter(AttackParam, attack);
        CabbageAudioManager.Instance.SetParameter(DecayParam, decay);
        CabbageAudioManager.Instance.SetParameter(SustainParam, sustain);
        CabbageAudioManager.Instance.SetParameter(ReleaseParam, release);
    }

    private const float textureTransitionTime = 1.0f; // Transition time in seconds


    public void SetTexture(float wood, float stone, float mudMix, float water, float snow)
    {
        StartCoroutine(InterpolateParameter("Wood", wood));
        StartCoroutine(InterpolateParameter("Stone", stone));
        StartCoroutine(InterpolateParameter("MudMix", mudMix));
        StartCoroutine(InterpolateParameter("Water", water));
        StartCoroutine(InterpolateParameter("Snow", snow));
    }

    private float GetCurrentParameterValue(string parameterName)
    {
        switch (parameterName)
        {
            case "Wood": return currentWood;
            case "Stone": return currentStone;
            case "MudMix": return currentMudMix;
            case "Water": return currentWater;
            case "Snow": return currentSnow;
            default: return 0f; // Default value or throw an error
        }
    }

    private void UpdateCurrentParameterValue(string parameterName, float newValue)
    {
        switch (parameterName)
        {
            case "Wood": currentWood = newValue; break;
            case "Stone": currentStone = newValue; break;
            case "MudMix": currentMudMix = newValue; break;
            case "Water": currentWater = newValue; break;
            case "Snow": currentSnow = newValue; break;
                // Add cases for other parameters as necessary
        }
    }


    private IEnumerator InterpolateParameter(string parameterName, float targetValue)
    {
        float timeElapsed = 0f;
        float startValue = GetCurrentParameterValue(parameterName);

        while (timeElapsed < textureTransitionTime)
        {
            // Interpolate the current value towards the target value over the transition time
            float currentValue = Mathf.Lerp(startValue, targetValue, timeElapsed / textureTransitionTime);
            CabbageAudioManager.Instance.SetParameter(parameterName, currentValue);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set
        CabbageAudioManager.Instance.SetParameter(parameterName, targetValue);

        // Update the current value for the parameter
        UpdateCurrentParameterValue(parameterName, targetValue);
    }


    public void SetPitch(float value)
    {
        CabbageAudioManager.Instance.SetParameter(PitchParam, value);
    }

    public void SetForce(float value)
    {
        CabbageAudioManager.Instance.SetParameter(ForceParam, value);
    }

    public void SetMaterial(float value)
    {
        CabbageAudioManager.Instance.SetParameter(MaterialParam, value);
    }

    public void SetGrassLength(float value)
    {
        CabbageAudioManager.Instance.SetParameter(GrassLengthParam, value);
    }

    public void SetWaterDepth(float value)
    {
        CabbageAudioManager.Instance.SetParameter(WaterDepthParam, value);
    }
}

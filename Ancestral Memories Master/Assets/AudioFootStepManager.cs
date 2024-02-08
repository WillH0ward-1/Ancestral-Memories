using System.Collections;
using UnityEngine;

public class AudioFootStepManager : MonoBehaviour
{
    CsoundUnity cSoundObj;

    private const string Instrument = "Footstep";
    private const string OneStepParam = "toggleOneStep";
    private const string continuousStepsParam = "toggleSteps";
    private const string continuousDroneParam = "toggleDrone";
    private const string SneakParam = "toggleSneak";
    private const string AttackParam = "attack";
    private const string DecayParam = "decay";
    private const string SustainParam = "sustain";
    private const string ReleaseParam = "release";
    private const string GrassParam = "grassMix";
    private const string WoodParam = "woodMix";
    private const string StoneParam = "stoneMix";
    private const string MudMixParam = "mudMix";
    private const string WaterParam = "waterMix";
    private const string SnowParam = "snowMix";
    private const string PitchParam = "pitch";
    private const string ForceParam = "stepForce";
    private const string HeelParam = "heel";
    private const string HeelMixerParam = "heelMixer";
    private const string HeelRandParam = "heelRandom";
    private const string WetnessParam = "wetness";
    private const string RoughnessParam = "roughness";
    private const string GrassLengthParam = "grassLength";
    private const string WaterDepthParam = "waterDepth";

    private float currentGrass = 0f;
    private float currentWood = 0f;
    private float currentStone = 0f;
    private float currentMudMix = 0f;
    private float currentWater = 0f;
    private float currentSnow = 0f;

    private int minVal = 0;
    private float midVal = 0.5f;
    private int maxVal = 1;

    private void Awake()
    {
        cSoundObj = GetComponent<CsoundUnity>();
    }

    private void Start()
    {
        SetTextureInstant(maxVal, minVal, minVal, minVal, minVal, minVal);

        CabbageAudioManager.Instance.SetTrigger(cSoundObj, continuousStepsParam, false);
        CabbageAudioManager.Instance.SetTrigger(cSoundObj, continuousDroneParam, false);
        CabbageAudioManager.Instance.SetTrigger(cSoundObj, SneakParam, false); 
    }

    public void TriggerFootstep()
    {
        CabbageAudioManager.Instance.SetTrigger(cSoundObj, Instrument, true);
    }

    public void SetSneak(bool enabled)
    {
        CabbageAudioManager.Instance.SetTrigger(cSoundObj, SneakParam, enabled);
    }

    public void SetADSR(float attack, float decay, float sustain, float release)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, AttackParam, attack);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, DecayParam, decay);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, SustainParam, sustain);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, ReleaseParam, release);
    }

    private const float textureTransitionTime = 1.0f; // Transition time in seconds


    public void SetTextureLerp(float grass, float wood, float stone, float mudMix, float water, float snow)
    {
        StartCoroutine(InterpolateParameter(GrassParam, grass));
        StartCoroutine(InterpolateParameter(WoodParam, wood));
        StartCoroutine(InterpolateParameter(StoneParam, stone));
        StartCoroutine(InterpolateParameter(MudMixParam, mudMix));
        StartCoroutine(InterpolateParameter(WaterParam, water));
        StartCoroutine(InterpolateParameter(SnowParam, snow));
    }

    public void SetTextureInstant(float grass, float wood, float stone, float mudMix, float water, float snow)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, GrassParam, grass);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, WoodParam, wood);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, StoneParam, stone);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, MudMixParam, mudMix);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, WaterParam, water);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, SnowParam, snow);
    }

    private float GetCurrentParameterValue(string parameterName)
    {
        return parameterName switch
        {
            GrassParam => currentGrass,
            WoodParam => currentWood,
            StoneParam => currentStone,
            MudMixParam => currentMudMix,
            WaterParam => currentWater,
            SnowParam => currentSnow,
            _ => 0f, // Default
        };
    }

    private void UpdateCurrentParameterValue(string parameterName, float newValue)
    {
        switch (parameterName)
        {
            case GrassParam: currentGrass = newValue; break;
            case WoodParam: currentWood = newValue; break;
            case StoneParam: currentStone = newValue; break;
            case MudMixParam: currentMudMix = newValue; break;
            case WaterParam: currentWater = newValue; break;
            case SnowParam: currentSnow = newValue; break;
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
            CabbageAudioManager.Instance.SetParameter(cSoundObj, parameterName, currentValue);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set
        CabbageAudioManager.Instance.SetParameter(cSoundObj, parameterName, targetValue);

        // Update the current value for the parameter
        UpdateCurrentParameterValue(parameterName, targetValue);
    }


    public void SetPitch(float value)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, PitchParam, value);
    }

    public void SetForce(float value)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, ForceParam, value);
    }

    public void SetGrassLength(float value)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, GrassLengthParam, value);
    }

    public void SetWaterDepth(float value)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, WaterDepthParam, value);
    }
}

using System.Collections;
using UnityEngine;

public class AudioWindManager : MonoBehaviour
{
    [SerializeField] private CsoundUnity cSoundObj;

    private const string toggleWind = "ONOFF";
    private const string windStrength = "WindStrength";
    private const string windWhistle = "WindWhistle";
    private const string windWhistleResonance = "ResonanceStrength";
    private const string windRumble = "WindRumble";
    private const string lfo = "LFO";
    private const string lfoDepth = "LFODepth";
    private const string windRandomness = "WindRandomness";

    private const string ampAttack = "AmpAttack";
    private const string ampDecay = "AmpDecay";
    private const string ampSustain = "AmpSustain";
    private const string ampRelease = "AmpRelease";

    // Global min and max, assuming all parameters use these bounds
    private const float globalMin = 0f;
    private const float globalMax = 1f;

    [SerializeField] private float currentWindStrength = globalMin;
    [SerializeField] private float currentWindWhistle = globalMin;
    [SerializeField] private float currentWindWhistleResonance = globalMin;
    [SerializeField] private float currentWindRumble = globalMin;
    [SerializeField] private float currentLFO = globalMin;
    [SerializeField] private float currentLFODepth = globalMin;
    [SerializeField] private float currentWindRandomness = globalMin;

    private float currentAmpAttack;
    private float currentAmpDecay;
    private float currentAmpSustain;
    private float currentAmpRelease;

    private AICharacterStats stats;

    public Player player;

    public bool windIsActive;

    public void InitCsoundObj()
    {
       cSoundObj = GetComponent<CsoundUnity>();
    }

    public enum ADSRStage
    {
        Attack,
        Decay,
        Sustain,
        Release
    }


    public float GetADSR(ADSRStage stage)
    {
        switch (stage)
        {
            case ADSRStage.Attack:
                return (float)cSoundObj.GetChannel(ampAttack);
            case ADSRStage.Decay:
                return (float)cSoundObj.GetChannel(ampDecay);
            case ADSRStage.Sustain:
                return (float)cSoundObj.GetChannel(ampSustain);
            case ADSRStage.Release:
                return (float)cSoundObj.GetChannel(ampRelease);
            default:
                return 0f; // Return a default value if the stage is not recognized
        }
    }

    private void OnDisable()
    {
        if (windIsActive)
        {
            windIsActive = false;
        }
        //StopAllCoroutines(); // Ensure all ongoing coroutines are stopped
        // ResetWindParameters(); // Reset or turn off the wind sound
    }



    public IEnumerator AdjustWindParametersBasedOnFaith()
    {
        windIsActive = true;

        while (windIsActive)
        {
            if (player != null)
            {
                float faithLevel = player.faith; // Assuming Player script has a 'faith' float variable

//                Debug.Log("FAITH: " + player.faith + "/" + faithLevel);

                float targetValue = Mathf.Lerp(globalMax, globalMin, faithLevel);

//                Debug.Log("TARGETVAL " + targetValue);

                SetAllWindParameters(targetValue);

            }

            yield return null;
        }


    }


    public void SetAllWindParameters(float targetValue)
    {
        // Directly apply the target value to all wind parameters
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windStrength, targetValue);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windWhistle, targetValue);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windWhistleResonance, targetValue);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windRumble, targetValue);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, lfo, targetValue);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, lfoDepth, targetValue);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windRandomness, targetValue);
    }

    private void ResetWindParameters()
    {
        // Reset wind parameters to initial or silent values
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windStrength, 0);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windWhistle, 0);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windWhistleResonance, 0);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windRumble, 0);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, lfo, 0);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, lfoDepth, 0);
        CabbageAudioManager.Instance.SetParameter(cSoundObj, windRandomness, 0);
        // Make sure to turn off the wind if needed
        CabbageAudioManager.Instance.SetTrigger(cSoundObj, toggleWind, false);
    }

    private IEnumerator SetAllWindParametersContinuous(float targetValue, float duration)
    {
        ApplyWindParameters();

        windIsActive = true; // Assuming this is a class member variable to control the loop

        // Capture initial values at the start of the interpolation
        float initialStrength = currentWindStrength;
        float initialWhistle = currentWindWhistle;
        float initialWhistleResonance = currentWindWhistleResonance;
        float initialRumble = currentWindRumble;
        float initialLFO = currentLFO;
        float initialLFODepth = currentLFODepth;
        float initialRandomness = currentWindRandomness;

        float elapsedTime = 0f;
        while (elapsedTime < duration && windIsActive)
        {
            // Calculate interpolation factor
            float t = elapsedTime / duration;
            float interpolatedValue = Mathf.Lerp(0f, targetValue, t);

            // Update current values based on interpolation
            currentWindStrength = interpolatedValue;
            currentWindWhistle = interpolatedValue;
            currentWindWhistleResonance = interpolatedValue;
            currentWindRumble = interpolatedValue;
            currentLFO = interpolatedValue;
            currentLFODepth = interpolatedValue;
            currentWindRandomness = interpolatedValue;

            // Apply the interpolated values
            ApplyWindParameters();

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Make sure all parameters are set to the target value when done
        SetWindParametersToTarget(targetValue);
    }

    private void ApplyWindParameters()
    {
        cSoundObj.SetChannel(windStrength, currentWindStrength);
        cSoundObj.SetChannel(windWhistle, currentWindWhistle);
        cSoundObj.SetChannel(windWhistleResonance, currentWindWhistleResonance);
        cSoundObj.SetChannel(windRumble, currentWindRumble);
        cSoundObj.SetChannel(lfo, currentLFO);
        cSoundObj.SetChannel(lfoDepth, currentLFODepth);
        cSoundObj.SetChannel(windRandomness, currentWindRandomness);
    }

    private void SetWindParametersToTarget(float targetValue)
    {
        currentWindStrength = targetValue;
        currentWindWhistle = targetValue;
        currentWindWhistleResonance = targetValue;
        currentWindRumble = targetValue;
        currentLFO = targetValue;
        currentLFODepth = targetValue;
        currentWindRandomness = targetValue;

        // Apply the final values
        ApplyWindParameters();
    }




    public void SetPlayState(bool isOn)
    {
        Debug.Log("Setting Play State To: " + isOn);
        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, toggleWind, isOn));
    }

    public void SetWindStrength(float targetValue, float duration)
    {
        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, windStrength, currentWindStrength, targetValue, duration));
        currentWindStrength = targetValue;
    }

    // Implement similar methods for other wind parameters
    public void SetWindWhistle(float targetValue, float duration)
    {
        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, windWhistle, currentWindWhistle, targetValue, duration));
        currentWindWhistle = targetValue;
    }

    public void SetWindWhistleResonance(float targetValue, float duration)
    {
        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, windWhistleResonance, currentWindWhistleResonance, targetValue, duration));
        currentWindWhistleResonance = targetValue;
    }

    public void SetWindRumble(float targetValue, float duration)
    {
        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, windRumble, currentWindRumble, targetValue, duration));
        currentWindRumble = targetValue;
    }

    public void SetLFO(float targetValue, float duration)
    {
        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, lfo, currentLFO, targetValue, duration));
        currentLFO = targetValue;
    }

    public void SetLFODepth(float targetValue, float duration)
    {
        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, lfoDepth, currentLFODepth, targetValue, duration));
        currentLFODepth = targetValue;
    }

    public void SetWindRandomness(float targetValue, float duration)
    {
        StartCoroutine(CabbageAudioManager.Instance.SetParameterWithLerp(cSoundObj, windRandomness, currentWindRandomness, targetValue, duration));
        currentWindRandomness = targetValue;
    }
}

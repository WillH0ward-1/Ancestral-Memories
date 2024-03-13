using System;
using System.Collections;
using UnityEngine;

public class CabbageAudioManager : MonoBehaviour
{
    public static CabbageAudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetParameter(CsoundUnity cSound, string parameterName, float value)
    {
        if (cSound.GetChannel(parameterName) != value)
        {
            cSound.SetChannel(parameterName, value);
        }
        //Debug.Log($"Parameter {parameterName} set to: {value}");
    }

    public void SetTrigger(CsoundUnity cSound, string parameterName, bool state)
    {
        string stateValue = state ? "1" : "0";
        string triggerCommand = $"i\"{parameterName}\" 0 {stateValue}";
        cSound.SendScoreEvent(triggerCommand);
        //Debug.Log($"Trigger {parameterName} set to: {stateValue}");
    }

    public IEnumerator TriggerOneShot(CsoundUnity cSound, string parameterName, bool turnOn)
    {
        if (turnOn)
        {
            cSound.SetChannel(parameterName, 0);
        }
        else
        {
            cSound.SetChannel(parameterName, 1);
        }
        yield return null;

        if (turnOn)
        {
            cSound.SetChannel(parameterName, 1);
        }
        else
        {
            cSound.SetChannel(parameterName, 0);
        }
        yield return null;

        yield break;
    }


    public IEnumerator SetParameterWithLerp(CsoundUnity cSound, string parameterName, float startValue, float endValue, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            cSound.SetChannel(parameterName, currentValue);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }

        // Ensure the parameter is set to the final value.
        cSound.SetChannel(parameterName, endValue);

        yield break;
    }


}

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

    internal void SetParameter(float growTime)
    {
        throw new NotImplementedException();
    }

    public IEnumerator TriggerOneShot(CsoundUnity cSound, string parameterName, bool turnOn)
    {
        // First, set the parameter to the opposite state to ensure a change occurs
        cSound.SetChannel(parameterName, turnOn ? 0 : 1);
        yield return null; // Wait for one frame to ensure the command has been processed

        // Then, set it to the desired state
        cSound.SetChannel(parameterName, turnOn ? 1 : 0);
        yield return null; // Wait for another frame to ensure the command has been processed
    }

    public IEnumerator SetParameterWithLerp(CsoundUnity cSound, string parameterName, float startValue, float endValue, float duration)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);
            cSound.SetChannel(parameterName, currentValue);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        // Ensure the parameter is set to the final value.
        cSound.SetChannel(parameterName, endValue);
    }


}

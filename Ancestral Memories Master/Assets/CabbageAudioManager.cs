using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csoundcsharp;

public class CabbageAudioManager : MonoBehaviour
{
    // Static instance property
    public static CabbageAudioManager Instance { get; private set; }

    // Reference to the CsoundUnity component
    private CsoundUnity csoundUnity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            csoundUnity = GetComponent<CsoundUnity>();

            DontDestroyOnLoad(gameObject);
        }
    }

    // Method to handle changing parameters
    public void SetParameter(string parameterName, float value)
    {
        string parameterCommand = $"i \"{parameterName}\" 0 {value}";
        csoundUnity.SendScoreEvent(parameterCommand);
        Debug.Log($"Parameter {parameterName} set to: {value}");
    }

    // Method to handle triggering events
    public void SetTrigger(string parameterName, bool state)
    {
        string stateValue = state ? "1" : "0";
        string triggerCommand = $"i \"{parameterName}\" 0 {stateValue}";
        csoundUnity.SendScoreEvent(triggerCommand);
        Debug.Log($"Trigger {parameterName} set to: {stateValue}");
    }
}

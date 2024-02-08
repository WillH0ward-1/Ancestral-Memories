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
        cSound.SetChannel(parameterName, value);
        //Debug.Log($"Parameter {parameterName} set to: {value}");
    }

    public void SetTrigger(CsoundUnity cSound, string parameterName, bool state)
    {
        string stateValue = state ? "1" : "0";
        string triggerCommand = $"i\"{parameterName}\" 0 {stateValue}";
        cSound.SendScoreEvent(triggerCommand);
        //Debug.Log($"Trigger {parameterName} set to: {stateValue}");
    }
}

using System.Collections;
using System.Collections.Generic;
using ProceduralModeling;
using UnityEngine;

public class AudioTreeLeavesManager : MonoBehaviour
{
    CsoundUnity cSoundObj;

    private string ONOFF = "ONOFF";
    private string GrowthParam = "GrowthStage";

    [SerializeField] private float currentGrowth;
    private float windStrength;

    public PTGrowing pTGrowing;

    public WeatherControl weatherControl;

    private void Awake()
    {
        cSoundObj = GetComponent<CsoundUnity>();
    }

    public void SetPlayState(bool isOn)
    {
        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, ONOFF, isOn));
    }

    public void SetLeafGrowthFX(float growTime)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, GrowthParam, growTime);
        currentGrowth = growTime;
    }

    public void SetLeafRustlingFX(float wind)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, GrowthParam, wind);
        windStrength = wind;
    }

}

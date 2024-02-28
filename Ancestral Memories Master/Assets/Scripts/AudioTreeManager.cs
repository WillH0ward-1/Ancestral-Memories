using System.Collections;
using System.Collections.Generic;
using ProceduralModeling;
using UnityEngine;

public class AudioTreeManager : MonoBehaviour
{
    CsoundUnity cSoundObj;

    private string ONOFF = "ONOFF";
    private string GrowthParam = "GrowthStage";

    [SerializeField] private float currentGrowth;

    public PTGrowing pTGrowing;

    private void Awake()
    {
        cSoundObj = GetComponent<CsoundUnity>();
    }

    public void SetPlayState(bool isOn)
    {
        StartCoroutine(CabbageAudioManager.Instance.TriggerOneShot(cSoundObj, ONOFF, isOn));
    }

    public void SetGrowthFX(float growTime)
    {
        CabbageAudioManager.Instance.SetParameter(cSoundObj, GrowthParam, growTime);
        currentGrowth = growTime;
    }

}


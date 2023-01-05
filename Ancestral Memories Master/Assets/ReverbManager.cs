using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;

public class ReverbManager : MonoBehaviour
{
    public EventInstance reverbInstance;
    public StudioEventEmitter emitter;

    private void Awake()
    {
        emitter = transform.GetComponent<StudioEventEmitter>();
    }

}

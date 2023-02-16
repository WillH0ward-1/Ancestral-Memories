// Code starts here
using UnityEngine;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;

public class AmbientSoundScript : MonoBehaviour
{
    private Light light;
    private float lightIntensity;
    private float lightRange;

    [SerializeField] private List<GameObject> sounds;
    [SerializeField] private List<StudioEventEmitter> emitters;
    List<EventInstance> events;
    List<ChannelGroup> channelGroups;
    List<DSP> dspList;
    [SerializeField] private FMOD.Studio.System system;
    private StudioEventEmitter eventEmitter;

    void Start()
    {
        light = GetComponent<Light>();
        lightIntensity = light.intensity;
        lightRange = light.range;

        output = new DSP_METERING_INFO();
        events = new List<EventInstance>();
        channelGroups = new List<ChannelGroup>();
        dspList = new List<DSP>();

        system.update();
        system.flushCommands();

        ChannelGroup group;
        EventInstance evt1;
        EventDescription evtDesrcription;

        foreach (GameObject s in sounds)
        {
            emitters.Add(s.GetComponent<StudioEventEmitter>());
        }

        foreach (StudioEventEmitter emitter in emitters)
        {
            system.getEvent(emitter.EventReference.Path, out evtDesrcription);
            evtDesrcription.createInstance(out evt1);
            evt1.start();
            events.Add(evt1);
        }
        system.flushCommands();
        system.update();

        foreach (FMOD.Studio.EventInstance evt in events)
        {
            evt.getChannelGroup(out group);
            system.flushCommands();
            system.update();
            channelGroups.Add(group);
        }

        FMOD.DSP dsp;
        foreach (FMOD.ChannelGroup cg in channelGroups)
        {
            cg.getDSP(0, out dsp);
            dsp.setMeteringEnabled(true, true);
            dspList.Add(dsp);
            system.flushCommands();
            system.update();
        }
    }

    float sum = 0;
    DSP_METERING_INFO output;

    void Update()
    {
        // Sum all amplitudes

        foreach (FMOD.DSP dsp in dspList)
        {
            dsp.getMeteringInfo(IntPtr.Zero, out output);
            float outpeaks = output.peaklevel[0] + output.peaklevel[1];
            float rms = output.rmslevel[0] + output.rmslevel[1];
            sum += outpeaks + rms;
        }

        // Modify the lighting according to meter readings
        light.range = lightRange + sum * 3;
        light.intensity = lightIntensity + sum * 3;
        sum = 0;
    }
}

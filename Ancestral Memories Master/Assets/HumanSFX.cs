using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using FMODUnity;
//using FMOD.Studio;
using System;

public class HumanSFX : MonoBehaviour
{
    private Rigidbody rigidBody;

    /*
    [SerializeField] private EventReference WalkEventPath;
    [SerializeField] private EventReference HitTreeEventPath;
    [SerializeField] private EventReference WhooshEventPath;
    [SerializeField] private EventReference UprootPlantEventPath;
    [SerializeField] private EventReference EatEventPath;
    [SerializeField] private EventReference VomitEventPath;
    [SerializeField] private EventReference FluteEventPath;
    [SerializeField] private EventReference PrayerEventPath;
    */

    public AudioSFXManager audioManager;

    private void Awake()
    {

        /*
        WalkEventPath = playerSFX.WalkEventPath;
        HitTreeEventPath = playerSFX.HitTreeEventPath;
        WhooshEventPath = playerSFX.WhooshEventPath;
        UprootPlantEventPath = playerSFX.UprootPlantEventPath;
        EatEventPath = playerSFX.EatEventPath;
        VomitEventPath = playerSFX.VomitEventPath;
        FluteEventPath = playerSFX.FluteEventPath;
        PrayerEventPath = playerSFX.PrayerEventPath;
        */

        rigidBody = transform.GetComponentInChildren<Rigidbody>();

    }

    void PlayFootstep()
    {
        /*
        EventInstance footStepInstance = RuntimeManager.CreateInstance(WalkEventPath);
        RuntimeManager.AttachInstanceToGameObject(footStepInstance, transform, rigidBody);

        footStepInstance.start();
        footStepInstance.release();
        */
    }

    void HitTree()
    {
        /*
        EventInstance hitTreeInstance = RuntimeManager.CreateInstance(HitTreeEventPath);
        RuntimeManager.AttachInstanceToGameObject(hitTreeInstance, transform, rigidBody);

        hitTreeInstance.start();
        hitTreeInstance.release();
        */
    }

    public void PlayPrayerAudioLoop()
    {
        /*
        EventInstance prayerInstance = RuntimeManager.CreateInstance(PrayerEventPath);
        RuntimeManager.AttachInstanceToGameObject(prayerInstance, transform, rigidBody);

        prayerInstance.start();
        prayerInstance.release();
        */
    }

    public void PlayFluteEvent()
    {
        /*
        EventInstance fluteEvent = RuntimeManager.CreateInstance(FluteEventPath);
        RuntimeManager.AttachInstanceToGameObject(fluteEvent, transform, rigidBody);

        fluteEvent.start();
        fluteEvent.release();
        */
    }

    public void UprootPlantEvent()
    {
        /*
        EventInstance pickPlantEvent = RuntimeManager.CreateInstance(UprootPlantEventPath);
        RuntimeManager.AttachInstanceToGameObject(pickPlantEvent, transform, rigidBody);

        pickPlantEvent.start();
        pickPlantEvent.release();
        */
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

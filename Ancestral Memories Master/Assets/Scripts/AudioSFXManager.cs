using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class AudioSFXManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    public Rigidbody rigidBody;
    private AreaManager areaManager;
    [SerializeField] private PlayerWalk playerWalk;

    private Player player;

    public EventReference WalkEventPath;
    public EventReference HitTreeEventPath;
    public EventReference WhooshEventPath;
    public EventReference UprootPlantEventPath;
    public EventReference EatEventPath;
    public EventReference VomitEventPath;
    public EventReference FluteEventPath;
    public EventReference PrayerEventPath;

    //[SerializeField] private EventReference PlayerScreamEventPath;
    //[SerializeField] private EventReference DrownEventPath;

    private EventInstance walkEvent;

    private EventInstance rightFootStepInstance;
    private EventInstance leftFootStepInstance;

    private EventInstance fluteEventInstance;

    private EventInstance prayerInstance;

    public MusicManager musicManager;
    
    private void Awake()
    {
        rigidBody = transform.parent.GetComponent<Rigidbody>();
        player = transform.parent.GetComponent<Player>();
    }

    public void AreaUpdateGroundType(int index)
    {
    // For area-based method, the 'TerrainType' parameter should be set to 'global'

        RuntimeManager.StudioSystem.setParameterByName("TerrainType", index);
    }

    // For raycasting method, the 'TerrainType' parameter should be set to 'local'

    public void RayUpdateGroundType(Transform raySource, int index)
    {
        if (raySource.CompareTag("LeftFoot"))
        {
            leftFootStepInstance.setParameterByName("TerrainType", index);
        }

        if (raySource.CompareTag("RightFoot"))
        {
            rightFootStepInstance.setParameterByName("TerrainType", index);
        }
    }

    public void UpdateWaterDepth(Transform raySource, float depth)
    {
            leftFootStepInstance.setParameterByName("WaterDepth", depth);

            rightFootStepInstance.setParameterByName("WaterDepth", depth);
    }

    public void CheckGroundType()
    {
        Debug.Log("Footstep Event Triggered");

        //  This code does not work! Currently in a state that may not make sense due to attempting to fix.
        //playerWalk.StartCoroutine(playerWalk.DetectGroundType());

    }

    // These are triggered by animation events. See Human to get access to the Animator and it's contained Animation sheet.

    public CharacterBehaviours behaviours;

    void TriggerInstruments()
    {
        if (behaviours.isPsychdelicMode)
        {
            musicManager.PlayOneShot(MusicManager.Instruments.HangDrum.ToString(), transform.gameObject, true);
            musicManager.PlayOneShot(MusicManager.Instruments.JawHarp.ToString(), transform.gameObject, false);
        }
    }

    void PlayLeftFootStep()
    {

        TriggerInstruments();

        CheckGroundType();

        leftFootStepInstance = RuntimeManager.CreateInstance(WalkEventPath);
        RuntimeManager.AttachInstanceToGameObject(leftFootStepInstance, transform, rigidBody);

        leftFootStepInstance.start();
        leftFootStepInstance.release();
    }

    void PlayRightFootStep()
    {
        TriggerInstruments();

        CheckGroundType();

        rightFootStepInstance = RuntimeManager.CreateInstance(WalkEventPath);
        RuntimeManager.AttachInstanceToGameObject(rightFootStepInstance, transform, rigidBody);

        rightFootStepInstance.start();
        rightFootStepInstance.release();
    }

    public void PlayPrayerAudioLoop()
    {
        prayerInstance = RuntimeManager.CreateInstance(PrayerEventPath);
        RuntimeManager.AttachInstanceToGameObject(prayerInstance, transform, rigidBody);

        prayerInstance.start();
        prayerInstance.release();
    }

    public void StopPrayerAudio()
    {
        prayerInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void PlayWalkEvent()
    {
        walkEvent = RuntimeManager.CreateInstance(WalkEventPath);
        RuntimeManager.AttachInstanceToGameObject(walkEvent, transform, rigidBody);

        walkEvent.start();
        walkEvent.release();
    }

    [NonSerialized] public EventInstance fluteEventRef;

    public void PlayFluteEvent()
    {
        EventInstance fluteEvent = RuntimeManager.CreateInstance(FluteEventPath);
        RuntimeManager.AttachInstanceToGameObject(fluteEvent, transform, rigidBody);

        fluteEventRef = fluteEvent;

        fluteEventRef.start();
        fluteEventRef.release();
    }

    void Vomit()
    {
        EventInstance vomitEvent = RuntimeManager.CreateInstance(VomitEventPath);
        RuntimeManager.AttachInstanceToGameObject(vomitEvent, transform, rigidBody);

        vomitEvent.start();
        vomitEvent.release();
    }

    public void UprootPlantEvent()
    {
        EventInstance pickPlantEvent = RuntimeManager.CreateInstance(UprootPlantEventPath);
        RuntimeManager.AttachInstanceToGameObject(pickPlantEvent, transform, rigidBody);

        pickPlantEvent.start();
        pickPlantEvent.release();
    }


    public void EatEvent()
    {
        EventInstance eatEvent = RuntimeManager.CreateInstance(EatEventPath);
        RuntimeManager.AttachInstanceToGameObject(eatEvent, transform, rigidBody);

        eatEvent.start();
        eatEvent.release();
    }

    void HitGround()
    {

        EventInstance walkEvent = RuntimeManager.CreateInstance(WalkEventPath);
        RuntimeManager.AttachInstanceToGameObject(walkEvent, transform, rigidBody);

        walkEvent.start();
        walkEvent.release();
    }

    float shakeMultiplier = 1;

    void WhooshEvent()
    {
        EventInstance whooshEvent = RuntimeManager.CreateInstance(WhooshEventPath);
        RuntimeManager.AttachInstanceToGameObject(whooshEvent, transform, rigidBody);

        whooshEvent.start();
        whooshEvent.release();
    }

    void HitTree()
    {
        EventInstance hitTreeEvent = RuntimeManager.CreateInstance(HitTreeEventPath);
        RuntimeManager.AttachInstanceToGameObject(hitTreeEvent, transform, rigidBody);

        hitTreeEvent.start();
        hitTreeEvent.release();

        float minShakeDuration = 1;
        float maxShakeDuration = 2;

        float duration = UnityEngine.Random.Range(minShakeDuration, maxShakeDuration);

        shakeMultiplier = UnityEngine.Random.Range(shakeMultiplier, shakeMultiplier);
        Shake camShake = cam.GetComponent<Shake>();
        StartCoroutine(camShake.ScreenShake(duration, shakeMultiplier));

        //shakeMultiplier = Random.Range(1, 1.1);
    }

    private int killTreeThreshold = 6;
    public int numberOfHits = 0;

    public GameObject targetTree;

    public bool tree;

    public void HitCount()
    {
        killTreeThreshold = UnityEngine.Random.Range(4, 9);

        numberOfHits++;

        if (player.isBlessed)
        {
            player.isBlessed = false;
        }

        if (numberOfHits >= killTreeThreshold)
        {
            targetTree.transform.GetComponentInChildren<TreeDeathManager>().Fall();
            numberOfHits = 0;
        }
    }


    /*
    void DrownEvent()
    {
    EventInstance drownEvent = RuntimeManager.CreateInstance(DrownEventPath);
    RuntimeManager.AttachInstanceToGameObject(drownEvent, transform, rigidBody);

    drownEvent.start();
    drownEvent.release();
    }
    */

    /*
    void ScreamingPainEvent()
    {
    EventInstance screamingPainEvent = RuntimeManager.CreateInstance(PlayerScreamEventPath);
    RuntimeManager.AttachInstanceToGameObject(screamingPainEvent, transform, rigidBody);

    screamingPainEvent.start();
    screamingPainEvent.release();

    }
    */

}

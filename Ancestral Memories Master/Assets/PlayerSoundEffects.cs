using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerSoundEffects : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Rigidbody rigidBody;
    private AreaManager areaManager;
    [SerializeField] private PlayerWalk playerWalk;

    [SerializeField] private string WalkEventPath;
    [SerializeField] private string HitTreeEventPath;
    [SerializeField] private string WhooshEventPath;
    [SerializeField] private string PlayerScreamEventPath;
    [SerializeField] private string DrownEventPath;
    [SerializeField] private string UprootPlantEventPath;
    [SerializeField] private string EatEventPath;
    [SerializeField] private string VomitEventPath;
    [SerializeField] private string FluteEventPath;

    private EventInstance walkEvent;
    private EventInstance rightFootStepEvent;
    private EventInstance leftFootStepEvent;

    private EventInstance fluteEventInstance;

    private void Awake()
    {
        rigidBody = transform.parent.GetComponent<Rigidbody>();
    }

    public void UpdateGroundType(Transform raySource, int index)
    {
        if (raySource.CompareTag("LeftFoot"))
        {
            leftFootStepEvent.setParameterByName("TerrainType", index);

        } else if (raySource.CompareTag("RightFoot"))
        {
            rightFootStepEvent.setParameterByName("TerrainType", index);

        }
    }

    public void UpdateWaterDepth(Transform raySource, float depth)
    {
        leftFootStepEvent.setParameterByName("WaterDepth", depth);
        rightFootStepEvent.setParameterByName("WaterDepth", depth);

    }

    void CheckGroundType()
    {
        
    }

    void PlayLeftFootStep()
    {
        playerWalk.StartCoroutine(playerWalk.DetectGroundType());

        leftFootStepEvent = RuntimeManager.CreateInstance(WalkEventPath);
        RuntimeManager.AttachInstanceToGameObject(leftFootStepEvent, transform, rigidBody);

        leftFootStepEvent.start();
        leftFootStepEvent.release();
    }

    void PlayRightFootStep()
    {
        playerWalk.StartCoroutine(playerWalk.DetectGroundType());

        rightFootStepEvent = RuntimeManager.CreateInstance(WalkEventPath);
        RuntimeManager.AttachInstanceToGameObject(rightFootStepEvent, transform, rigidBody);

        rightFootStepEvent.start();
        rightFootStepEvent.release();
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

    void DrownEvent()
    {
        EventInstance drownEvent = RuntimeManager.CreateInstance(DrownEventPath);
        RuntimeManager.AttachInstanceToGameObject(drownEvent, transform, rigidBody);

        drownEvent.start();
        drownEvent.release();
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


    void EatEvent()
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

    void HitTree()
    {
        EventInstance hitTreeEvent = RuntimeManager.CreateInstance(HitTreeEventPath);
        RuntimeManager.AttachInstanceToGameObject(hitTreeEvent, transform, rigidBody);

        hitTreeEvent.start();
        hitTreeEvent.release();

        Shake camShake = cam.GetComponent<Shake>();
        shakeMultiplier = UnityEngine.Random.Range(shakeMultiplier, shakeMultiplier);

        float minShakeDuration = 1;
        float maxShakeDuration = 2;

        float duration = UnityEngine.Random.Range(minShakeDuration, maxShakeDuration);

        //float minShakeMultiplier = 1;
        //float maxShakeMultiplier = 2;

        StartCoroutine(camShake.ScreenShake(duration, shakeMultiplier));
        //shakeMultiplier = Random.Range(1, 1.1);
    }

    private int killThreshold = 6;
    public int numberOfHits = 0;

    public GameObject targetTree;

    public bool tree;
    public void HitCount()
    {
        numberOfHits++;

        if (numberOfHits >= killThreshold)
        {
            targetTree.transform.GetComponentInChildren<TreeDeathManager>().Fall();

            numberOfHits = 0;
            return;
        }
    }


    void ScreamingPainEvent()
    {
        EventInstance screamingPainEvent = RuntimeManager.CreateInstance(PlayerScreamEventPath);
        RuntimeManager.AttachInstanceToGameObject(screamingPainEvent, transform, rigidBody);

        screamingPainEvent.start();
        screamingPainEvent.release();

    }

    void WhooshEvent()
    {
        EventInstance whooshEvent = RuntimeManager.CreateInstance(WhooshEventPath);
        RuntimeManager.AttachInstanceToGameObject(whooshEvent, transform, rigidBody);

        whooshEvent.start();
        whooshEvent.release();
    }
}

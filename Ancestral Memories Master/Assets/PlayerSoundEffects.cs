using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;

public class PlayerSoundEffects : MonoBehaviour
{
    private PARAMETER_ID ParamID;

    [SerializeField] private EventReference WalkEventPath;
    [SerializeField] private EventReference HitTreeEventPath;
    [SerializeField] private EventReference WhooshEventPath;
    [SerializeField] private EventReference PlayerScreamEventPath;
    [SerializeField] private EventReference DrownEventPath;
    [SerializeField] private EventReference UprootPlantEventPath;
    [SerializeField] private EventReference EatEventPath;

    [SerializeField] private Camera cam;

    [SerializeField] private PlayerWalk playerWalk;

    private AreaManager areaManager;

    private Rigidbody rigidBody;

    private EventInstance walkEvent;
    private EventInstance rightFootStepEvent;
    private EventInstance leftFootStepEvent;

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

    void DrownEvent()
    {
        EventInstance drownEvent = RuntimeManager.CreateInstance(DrownEventPath);
        RuntimeManager.AttachInstanceToGameObject(drownEvent, transform, rigidBody);

        drownEvent.start();
        drownEvent.release();
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

    float duration = 0;
    [SerializeField] float minShakeDuration = 1;
    [SerializeField] float maxShakeDuration = 2;

    [SerializeField] float minShakeMultiplier = 1;
    [SerializeField] float maxShakeMultiplier = 2;

    float shakeMultiplier = 1;

    void HitTree()
    {
        EventInstance hitTreeEvent = RuntimeManager.CreateInstance(HitTreeEventPath);
        RuntimeManager.AttachInstanceToGameObject(hitTreeEvent, transform, rigidBody);

        hitTreeEvent.start();
        hitTreeEvent.release();

        Shake camShake = cam.GetComponent<Shake>();
        shakeMultiplier = Random.Range(shakeMultiplier, shakeMultiplier);
        duration = Random.Range(minShakeDuration, maxShakeDuration);

        StartCoroutine(camShake.ScreenShake(duration, shakeMultiplier));
        //shakeMultiplier = Random.Range(1, 1.1);
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

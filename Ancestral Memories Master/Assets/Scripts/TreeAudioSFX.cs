using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;

public class TreeAudioSFX : MonoBehaviour
{

    private ScaleControl scaleControl;

    public float treeGrowTime;


    private Rigidbody rigidBody;

    [SerializeField] private EventReference TreeGrowEventPath;
    [SerializeField] private EventReference BirdChirpEventPath;

    //private EventInstance treeGrowSFXInstance;
    private EventInstance birdChirpInstance;
    private Interactable interactable;
    public TreeDeathManager treeFallManager;

    public TimeCycleManager timeManager;

    void Awake()
    {
        scaleControl = transform.GetComponent<ScaleControl>();
        rigidBody = transform.GetComponent<Rigidbody>();

        interactable = transform.GetComponent<Interactable>();
    }

    [SerializeField] private float newMin = 0;
    [SerializeField] private float newMax = 1;

    [SerializeField] private float enableInteractThreshold = 0.7f;

    public IEnumerator StartTreeGrowthSFX()
    {
        EventInstance treeGrowSFXInstance = RuntimeManager.CreateInstance(TreeGrowEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeGrowSFXInstance, transform, rigidBody);

        treeGrowSFXInstance.start();
        treeGrowSFXInstance.release();

        while (!scaleControl.isFullyGrown)
        {
            float output = scaleControl.growthPercent;

            treeGrowTime = output;

            treeGrowSFXInstance.setParameterByName("TreeGrowTime", treeGrowTime);

            //var t = Mathf.InverseLerp(0, 1, output);
            //float newOutput = Mathf.Lerp(1, 0, t);

            //            birdChirpInstance.setParameterByName("HarmonicStability", newOutput);

            if (scaleControl.growthPercent >= 0.8 && !timeManager.isNightTime)
            {
                StartTreeBirds();
            } else 
            {
                StopTreeBirds();
            }

            yield return null;
        }

        StopTreeGrowthSFX(treeGrowSFXInstance);

        yield break;

    }

    PLAYBACK_STATE PlaybackState(EventInstance instance) 
    {
        instance.getPlaybackState(out PLAYBACK_STATE state);
        return state;
    }

    void StopTreeGrowthSFX(EventInstance treeGrowSFXInstance)
    {
        if (PlaybackState(treeGrowSFXInstance) != PLAYBACK_STATE.STOPPED)
        {
            treeGrowSFXInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void StartTreeBirds()
    {
        EventInstance birdChirpInstance = RuntimeManager.CreateInstance(BirdChirpEventPath);
        RuntimeManager.AttachInstanceToGameObject(birdChirpInstance, transform, rigidBody);

        birdChirpInstance.start();
        birdChirpInstance.release();
    }

    void StopTreeBirds()
    {
        if (PlaybackState(birdChirpInstance) != PLAYBACK_STATE.STOPPED)
        {
            birdChirpInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            birdChirpInstance.release();
        }
    }

}

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

    private EventInstance treeGrowSFXInstance;
    private EventInstance birdChirpInstance;

    public TreeDeathManager treeFallManager;

    void Awake()
    {
        scaleControl = transform.GetComponentInChildren<ScaleControl>();
        rigidBody = transform.GetComponent<Rigidbody>();
    }

    [SerializeField] private float newMin = 0;
    [SerializeField] private float newMax = 1;

    public IEnumerator StartTreeGrowthSFX()
    {
        if (!scaleControl.isFullyGrown)
        {
            PlayTreeGrowthSFX();
        }

        while (!scaleControl.isFullyGrown && !treeFallManager.treeDead)
        {
            float output = scaleControl.growthPercent;

            treeGrowTime = output;

            treeGrowSFXInstance.setParameterByName("TreeGrowTime", output);

            var t = Mathf.InverseLerp(0, 1, output);
            float newOutput = Mathf.Lerp(1, 0, t);

            birdChirpInstance.setParameterByName("HarmonicStability", newOutput);

            if (scaleControl.growthPercent >= 0.7)
            {
                StartTreeBirds();
            }

            yield return null;
        }

        if (treeFallManager.treeDead)
        {
            StopTreeGrowthSFX();
            StopBirdSFX();

            yield break;
        }

        yield break;

    }

    void PlayTreeGrowthSFX()
    {
        treeGrowSFXInstance = RuntimeManager.CreateInstance(TreeGrowEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeGrowSFXInstance, transform, rigidBody);

        treeGrowSFXInstance.start();
        treeGrowSFXInstance.release();
    }

    PLAYBACK_STATE PlaybackState(EventInstance instance) 
    {
        instance.getPlaybackState(out PLAYBACK_STATE state);
        return state;
    }

    void StopTreeGrowthSFX()
    {
        if (PlaybackState(treeGrowSFXInstance) != PLAYBACK_STATE.STOPPED)
        {
            treeGrowSFXInstance.stop(FMODUnity.STOP_MODE.AllowFadeout);
            treeGrowSFXInstance.release();
        }

        return;
    }

    private void StartTreeBirds()
    {
        birdChirpInstance = RuntimeManager.CreateInstance(BirdChirpEventPath);
        RuntimeManager.AttachInstanceToGameObject(birdChirpInstance, transform, rigidBody);

        birdChirpInstance.start();
        birdChirpInstance.release();
    }

    void StopBirdSFX()
    {
        if (PlaybackState(birdChirpInstance) != PLAYBACK_STATE.STOPPED)
        {
            birdChirpInstance.stop(FMODUnity.STOP_MODE.AllowFadeout);
            birdChirpInstance.release();
        }

        return;
    }

}

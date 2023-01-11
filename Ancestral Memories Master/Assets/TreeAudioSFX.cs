using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;


public class TreeAudioSFX : MonoBehaviour
{

    private ScaleControl scaleControl;
    bool isPlaying = true;

    [SerializeField] private Player player;

    public float treeGrowTime;


    private Rigidbody rigidBody;


    [SerializeField] private EventReference TreeGrowEventPath;
    [SerializeField] private EventReference BirdChirpEventPath;

    private EventInstance treeGrowSFXInstance;
    private EventInstance birdChirpInstance;


    void Awake()
    {

        scaleControl = transform.GetComponentInChildren<ScaleControl>();
        rigidBody = transform.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        StartCoroutine(WaitForStart());
    }

    private IEnumerator WaitForStart()
    {
        yield return new WaitUntil(() => scaleControl.isGrowing);

        StartCoroutine(StartTreeGrowthSFX());
    }

    private void StartTreeBirds()
    {
        birdChirpInstance = RuntimeManager.CreateInstance(BirdChirpEventPath);
        RuntimeManager.AttachInstanceToGameObject(birdChirpInstance, transform, rigidBody);

        birdChirpInstance.start();
        birdChirpInstance.release();
    }

    [SerializeField] private float newMin = 0;
    [SerializeField] private float newMax = 1;

    private IEnumerator StartTreeGrowthSFX()
    {
        if (!scaleControl.isFullyGrown)
        {
            PlayTreeGrowthSFX();
        }

        while (scaleControl.isGrowing && !scaleControl.isFullyGrown)
        {
            float output = scaleControl.growthPercent;

            treeGrowTime = output;

            treeGrowSFXInstance.setParameterByName("TreeGrowTime", output);

            var t = Mathf.InverseLerp(0, 1, output);
            float newOutput = Mathf.Lerp(1, 0, t);

            birdChirpInstance.setParameterByName("HarmonicStability", newOutput);

            if (scaleControl.growthPercent >= 0.5)
            {
                StartTreeBirds();
            }

            yield return null;
        }

        if (scaleControl.isFullyGrown && !scaleControl.isGrowing)
        {
            StopTreeGrowthSFX();
        }

        yield break;

    }

    void PlayTreeGrowthSFX()
    {
        treeGrowSFXInstance = RuntimeManager.CreateInstance(TreeGrowEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeGrowSFXInstance, transform, rigidBody);

        treeGrowSFXInstance.start();
  
    }

    void StopTreeGrowthSFX()
    {
        treeGrowSFXInstance.stop(FMODUnity.STOP_MODE.AllowFadeout);
        treeGrowSFXInstance.release();
    }

}

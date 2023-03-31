using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class TreeAudioSFX : MonoBehaviour
{
    public TimeCycleManager timeManager;
    public WeatherControl weatherManager;
    private ScaleControl scaleControl;
    public TreeDeathManager treeFallManager;

    public float treeGrowTime;

    private Rigidbody rigidBody;

    [SerializeField] private EventReference TreeGrowEventPath;
    [SerializeField] private EventReference TreeLeavesEventPath;
    [SerializeField] private EventReference TreeSproutEventPath;
    [SerializeField] private EventReference BirdChirpEventPath;
    [SerializeField] private EventReference TreeHitGroundEventPath;

    [SerializeField] private EventReference TreeGrowMusicPath;

    private Interactable interactable;

    void Awake()
    {
        scaleControl = transform.GetComponent<ScaleControl>();
        rigidBody = transform.GetComponent<Rigidbody>();
        treeFallManager = transform.GetComponent<TreeDeathManager>();
        interactable = transform.GetComponent<Interactable>();
    }


    [SerializeField] private float newMin = 0;
    [SerializeField] private float newMax = 1;

    [SerializeField] private float enableInteractThreshold = 0.7f;

    public IEnumerator StartTreeGrowthSFX()
    {
        EventInstance treeGrowInstance = RuntimeManager.CreateInstance(TreeGrowEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeGrowInstance, transform, rigidBody);

        treeGrowInstance.start();
        treeGrowInstance.release();


        EventInstance treeLeavesSFXInstance = RuntimeManager.CreateInstance(TreeLeavesEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeLeavesSFXInstance, transform, rigidBody);

        treeLeavesSFXInstance.start();

        while (!scaleControl.isFullyGrown)
        {
            float output = scaleControl.growthPercent;

            treeGrowTime = output;

            treeGrowInstance.setParameterByName("TreeGrowTime", treeGrowTime);

            //var t = Mathf.InverseLerp(0, 1, output);
            //float newOutput = Mathf.Lerp(1, 0, t);
            //birdChirpInstance.setParameterByName("HarmonicStability", newOutput);

            yield return null;
        }

        treeLeavesSFXInstance.release();

        while (!treeFallManager.treeDead)
        {
            treeLeavesSFXInstance.setParameterByName("WindStrength", weatherManager.windStrength);

            yield return null;
        }

        StartTreeHitGroundSFX();
        StopInstance(treeGrowInstance);
        StopInstance(treeLeavesSFXInstance);

        yield break;

    }

    public void PlayTreeSproutSFX()
    {
        EventInstance treeSproutInstance = RuntimeManager.CreateInstance(TreeSproutEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeSproutInstance, transform, rigidBody);

        treeSproutInstance.start();
        treeSproutInstance.release();
    }

    public void StartTreeHitGroundSFX()
    {
        EventInstance treeHitFloorInstance = RuntimeManager.CreateInstance(TreeHitGroundEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeHitFloorInstance, transform, rigidBody);

        treeHitFloorInstance.start();
        treeHitFloorInstance.release();
    }

    PLAYBACK_STATE PlaybackState(EventInstance instance) 
    {
        instance.getPlaybackState(out PLAYBACK_STATE state);
        return state;
    }

    public void StopInstance(EventInstance instance)
    {
        if (PlaybackState(instance) != PLAYBACK_STATE.STOPPED)
        {
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private EventInstance birdInstance;

    private void StartTreeBirdsSFX()
    {
        EventInstance birdChirpInstance = RuntimeManager.CreateInstance(BirdChirpEventPath);
        RuntimeManager.AttachInstanceToGameObject(birdChirpInstance, transform, rigidBody);

        birdInstance = birdChirpInstance;

        birdChirpInstance.start();
        birdChirpInstance.release();
    }

    void StopTreeBirdsSFX()
    {
        birdInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        birdInstance.release();
    }

}

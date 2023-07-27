using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using ProceduralModeling;

public class TreeAudioManager : MonoBehaviour
{
    public TimeCycleManager timeManager;
    public WeatherControl weatherManager;
    private PTGrowing ptGrowing;

    private Rigidbody rigidBody;
    private LeafScaler leafScaler;

    [SerializeField] private EventReference TreeGrowEventPath;
    [SerializeField] private EventReference TreeLeavesEventPath;
    [SerializeField] private EventReference TreeSproutEventPath;
    [SerializeField] private EventReference BirdChirpEventPath;
    [SerializeField] private EventReference TreeHitGroundEventPath;

    [SerializeField] private EventReference TreeGrowMusicPath;

    private Interactable interactable;

    void Awake()
    {
        ptGrowing = transform.GetComponent<PTGrowing>();
        rigidBody = transform.GetComponent<Rigidbody>();
        interactable = transform.GetComponent<Interactable>();
        leafScaler = transform.GetComponent<LeafScaler>();
    }

    // Add this to your class members
    private EventInstance treeLeavesSFXInstance;

    public IEnumerator LeafRustleSFX()
    {
        treeLeavesSFXInstance = RuntimeManager.CreateInstance(TreeLeavesEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeLeavesSFXInstance, transform, rigidBody);

        treeLeavesSFXInstance.start();
        treeLeavesSFXInstance.release();

        while (!ptGrowing.isDead)
        {
            float leafDensity = leafScaler.NormalizeScale(leafScaler.CurrentScale);
            treeLeavesSFXInstance.setParameterByName("LeafDensity", leafDensity);
            yield return null;
        }

        StopInstance(treeLeavesSFXInstance);
        yield break;
    }

    private EventInstance treeGrowInstance;

    public void StartTreeGrowthSFX(PTGrowing.State state)
    {
        treeGrowInstance = RuntimeManager.CreateInstance(TreeGrowEventPath);
        RuntimeManager.AttachInstanceToGameObject(treeGrowInstance, transform, rigidBody);

        if (PlaybackState(treeGrowInstance) == PLAYBACK_STATE.STOPPED)
        {
            treeGrowInstance.start();
        }

        StartCoroutine(UpdateTreeGrowTime(state));
    }


    private IEnumerator UpdateTreeGrowTime(PTGrowing.State state)
    {
        if (state == PTGrowing.State.Growing)
        {
            while (!ptGrowing.isFullyGrown)
            {
                if (ptGrowing.isDead) // check if the tree is dead
                {
                    StopTreeGrowthSFX(); // stop the instance
                    yield break; // exit the coroutine
                }

                float growTime = ptGrowing.time / ptGrowing.growDuration; // this will give a value between 0 and 1
                treeGrowInstance.setParameterByName("TreeGrowTime", growTime);
                yield return null;
            }
        }
        else if (state == PTGrowing.State.Dying)
        {
            while (ptGrowing.isDead)
            {
                float dieTime = (ptGrowing.deathDuration - ptGrowing.time) / ptGrowing.deathDuration; // This will give a value decreasing from 1 to 0
                treeGrowInstance.setParameterByName("TreeGrowTime", dieTime);
                yield return null;
            }
        }

        // If the coroutine finishes naturally, stop the tree growth SFX
        StopTreeGrowthSFX();
    }



    public void StopTreeGrowthSFX()
    {
        // Stops the coroutine in case it's still running
        StopCoroutine(UpdateTreeGrowTime(PTGrowing.State.Buffering));
        StopInstance(treeGrowInstance);
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
    }

    void StopTreeBirdsSFX()
    {
        birdInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}

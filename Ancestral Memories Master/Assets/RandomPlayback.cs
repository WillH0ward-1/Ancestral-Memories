using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using FMODUnity;
using FMOD.Studio;

public class RandomPlayback : MonoBehaviour
{
    [SerializeField] private EventReference randomPlaybackEvent;

    int length;

    float minRetriggerTime = 3;
    float maxRetriggerTime = 7;

    private void Awake()
    {
        StartCoroutine(PlaybackRandom());
    }

    private IEnumerator PlaybackRandom() { 

        yield return new WaitForSeconds(Random.Range(minRetriggerTime, maxRetriggerTime));

        EventInstance eventInstance = RuntimeManager.CreateInstance(randomPlaybackEvent);
        RuntimeManager.AttachInstanceToGameObject(eventInstance, transform, GetComponent<Rigidbody>());

        eventInstance.start();
        eventInstance.release();

        yield return new WaitUntil(() => PlaybackState(eventInstance) != PLAYBACK_STATE.PLAYING);

        StartCoroutine(PlaybackRandom());
    }


    PLAYBACK_STATE PlaybackState(EventInstance instance)
    {
        instance.getPlaybackState(out PLAYBACK_STATE playBackState);
        return playBackState;
    }

    void Update()
    {
        
    }
}

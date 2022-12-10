using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using FMODUnity;
using FMOD.Studio;

public class RandomPlayback : MonoBehaviour
{
    [SerializeField] private EventReference SeagullPlaybackEvent;

    int length;

   float minRetriggerTime = 5;
   float maxRetriggerTime = 15;

    private void Awake()
    {
        StartCoroutine(PlaySeagullFX());
    }

    private IEnumerator PlaySeagullFX()
    {
        EventInstance seaGullEvent = RuntimeManager.CreateInstance(SeagullPlaybackEvent);
        RuntimeManager.AttachInstanceToGameObject(seaGullEvent, transform, GetComponent<Rigidbody>());

        seaGullEvent.start();
        seaGullEvent.release();

        yield return new WaitUntil(() => PlaybackState(seaGullEvent) != FMOD.Studio.PLAYBACK_STATE.PLAYING);
        yield return new WaitForSeconds(Random.Range(minRetriggerTime, maxRetriggerTime));
    }

    FMOD.Studio.PLAYBACK_STATE PlaybackState(EventInstance instance)
    {
        instance.getPlaybackState(out PLAYBACK_STATE playBackState);
        return playBackState;
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
//using FMOD.Studio;
//using FMODUnity;
using UnityEngine;

public class GodAudioSFX : MonoBehaviour
{
    /*
    [SerializeField] private EventReference godActiveAmbienceRef;
    [SerializeField] private EventInstance godActiveAmbienceFX;
    */

    public void StartGodAmbienceFX()
    {
        /*
        godActiveAmbienceFX = RuntimeManager.CreateInstance(godActiveAmbienceRef);
        RuntimeManager.AttachInstanceToGameObject(godActiveAmbienceFX, transform);
        godActiveAmbienceFX.start();
        godActiveAmbienceFX.release();
        */
    }

    public void StopGodAmbienceFX()
    {
        /*
        if (godActiveAmbienceFX.isValid())
        {
            godActiveAmbienceFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        */
    }
}
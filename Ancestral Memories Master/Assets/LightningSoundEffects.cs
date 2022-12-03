using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class LightningSoundEffects : MonoBehaviour
{
    private PARAMETER_ID ParamID;

    [SerializeField] private EventReference LightningStrikeEventPath;

    public void PlayLightningStrike(Transform lightningTransform)
    {

        EventInstance lightningStrikeEvent = RuntimeManager.CreateInstance(LightningStrikeEventPath);

        RuntimeManager.PlayOneShot(LightningStrikeEventPath, lightningTransform.position);

        lightningStrikeEvent.start();
        lightningStrikeEvent.release();
    }

}

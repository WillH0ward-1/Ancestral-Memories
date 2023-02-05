using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class LightningSoundEffects : MonoBehaviour
{
    private PARAMETER_ID ParamID;

    [SerializeField] private EventReference LightningStrikeEventPath;
    [SerializeField] private EventReference LightningBuzzEventPath;

    public void PlayLightningStrike(GameObject lightning)
    {

        EventInstance lightningStrikeEvent = RuntimeManager.CreateInstance(LightningStrikeEventPath);

        RuntimeManager.PlayOneShot(LightningStrikeEventPath, lightning.transform.position);

        //PlayBuzz(lightning);

        lightningStrikeEvent.start();
        lightningStrikeEvent.release();
    }

    public void PlayBuzz(GameObject lightning)
    {

        EventInstance electricBuzzEvent = RuntimeManager.CreateInstance(LightningBuzzEventPath);

        RuntimeManager.PlayOneShot(LightningBuzzEventPath, lightning.transform.position);

        electricBuzzEvent.start();
        electricBuzzEvent.release();
    }


}

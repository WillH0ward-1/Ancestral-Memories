using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerSoundEffects : MonoBehaviour
{
    private PARAMETER_ID ParamID;

    [SerializeField] private EventReference WalkEventPath;

    void PlayWalkEvent()
    {
        EventInstance walkEvent = RuntimeManager.CreateInstance(WalkEventPath);
        walkEvent.setParameterByNameWithLabel("TerrainType", "Grass");
        RuntimeManager.AttachInstanceToGameObject(walkEvent, transform, GetComponent<Rigidbody>());

        walkEvent.start();
        walkEvent.release();
    }
}

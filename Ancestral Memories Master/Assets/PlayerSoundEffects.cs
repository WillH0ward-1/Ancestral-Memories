using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerSoundEffects : MonoBehaviour
{
    private PARAMETER_ID ParamID;

    [SerializeField] private EventReference WalkEventPath;
    [SerializeField] private EventReference HitTreeEventPath;
    [SerializeField] private EventReference WhooshEventPath;

    void PlayWalkEvent()
    {
        EventInstance walkEvent = RuntimeManager.CreateInstance(WalkEventPath);
        walkEvent.setParameterByNameWithLabel("TerrainType", "Grass");
        RuntimeManager.AttachInstanceToGameObject(walkEvent, transform, GetComponent<Rigidbody>());

        walkEvent.start();
        walkEvent.release();
    }

    void HitTree()
    {
        EventInstance hitTreeEvent = RuntimeManager.CreateInstance(HitTreeEventPath);
        RuntimeManager.AttachInstanceToGameObject(hitTreeEvent, transform, GetComponent<Rigidbody>());

        hitTreeEvent.start();
        hitTreeEvent.release();
    }

    void WhooshEvent()
    {
        EventInstance whooshEvent = RuntimeManager.CreateInstance(WhooshEventPath);
        RuntimeManager.AttachInstanceToGameObject(whooshEvent, transform, GetComponent<Rigidbody>());

        whooshEvent.start();
        whooshEvent.release();
    }
}

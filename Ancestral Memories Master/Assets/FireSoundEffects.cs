using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FireSoundEffects : MonoBehaviour
{

    private PARAMETER_ID ParamID;

    [SerializeField] private EventReference FireWooshEvent;
    [SerializeField] private EventReference FireLoopEvent;

    private void Awake()
    {
        FireLoop();
    }

    void FireWoosh()
    {
        EventInstance fireWooshEvent = RuntimeManager.CreateInstance(FireWooshEvent);
        RuntimeManager.AttachInstanceToGameObject(fireWooshEvent, transform, GetComponent<Rigidbody>());

        fireWooshEvent.start();
        fireWooshEvent.release();
    }

    void FireLoop()
    {
        EventInstance fireLoopEvent = RuntimeManager.CreateInstance(FireLoopEvent);
        fireLoopEvent.setParameterByNameWithLabel("TerrainType", "Grass");
        RuntimeManager.AttachInstanceToGameObject(fireLoopEvent, transform, GetComponent<Rigidbody>());

        fireLoopEvent.start();
        //fireEvent.release();
    }
}


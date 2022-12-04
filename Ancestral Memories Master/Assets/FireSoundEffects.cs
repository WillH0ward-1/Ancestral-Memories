using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FireSoundEffects : MonoBehaviour
{

    private PARAMETER_ID ParamID;
    private EventInstance fireLoopEvent;
    private EventInstance fireWooshEvent;

    [SerializeField] private EventReference FireWooshEvent;
    [SerializeField] private EventReference FireLoopEvent;

    private void Awake()
    {
        FireWoosh();
        FireLoop();
    }

    void FireWoosh()
    {
   
        fireWooshEvent = RuntimeManager.CreateInstance(FireWooshEvent);
        
        RuntimeManager.AttachInstanceToGameObject(fireWooshEvent, transform, transform.GetComponent<Rigidbody>());

        fireWooshEvent.start();
        fireWooshEvent.release();
    }

    void FireLoop()
    {
        fireLoopEvent = RuntimeManager.CreateInstance(FireLoopEvent);
        RuntimeManager.AttachInstanceToGameObject(fireLoopEvent, transform, transform.GetComponent<Rigidbody>());

        fireLoopEvent.start();

        //fireEvent.release();
    }

    private void OnDestroy()
    {
        fireLoopEvent.release();
    }
}


using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PlayerSoundEffects : MonoBehaviour
{
    private PARAMETER_ID ParamID;

    [SerializeField] private EventReference WalkEventPath;
    [SerializeField] private EventReference HitTreeEventPath;
    [SerializeField] private EventReference WhooshEventPath;
    [SerializeField] private EventReference PlayerScreamEventPath;
    [SerializeField] private EventReference DrownEventPath;
    [SerializeField]private Camera cam;

    private string terrainType = "";
    public bool waterColliding = false;

    private void Awake()
    {
        waterColliding = false;
    }

    void PlayWalkEvent()
    {

        if (waterColliding)
        {
            terrainType = "Water";
        } else if (!waterColliding)
        {
            terrainType = "Grass";
        }

        EventInstance walkEvent = RuntimeManager.CreateInstance(WalkEventPath);
        walkEvent.setParameterByNameWithLabel("TerrainType", terrainType);
        RuntimeManager.AttachInstanceToGameObject(walkEvent, transform, GetComponent<Rigidbody>());

        walkEvent.start();
        walkEvent.release();
    }

    void DrownEvent()
    {
        EventInstance drownEvent = RuntimeManager.CreateInstance(DrownEventPath);
        RuntimeManager.AttachInstanceToGameObject(drownEvent, transform, GetComponent<Rigidbody>());

        drownEvent.start();
        drownEvent.release();
    }

    void HitGround()
    {

        if (waterColliding)
        {
            terrainType = "Water";
        }
        else if (!waterColliding)
        {
            terrainType = "Grass";
        }

        EventInstance walkEvent = RuntimeManager.CreateInstance(WalkEventPath);
        walkEvent.setParameterByNameWithLabel("TerrainType", terrainType);
        RuntimeManager.AttachInstanceToGameObject(walkEvent, transform, GetComponent<Rigidbody>());

        walkEvent.start();
        walkEvent.release();
    }

    float duration = 0;
    [SerializeField] float minShakeDuration = 1;
    [SerializeField] float maxShakeDuration = 2;
    float shakeMultiplier = 1;

    void HitTree()
    {
        EventInstance hitTreeEvent = RuntimeManager.CreateInstance(HitTreeEventPath);
        RuntimeManager.AttachInstanceToGameObject(hitTreeEvent, transform, GetComponent<Rigidbody>());

        hitTreeEvent.start();
        hitTreeEvent.release();

        Shake camShake = cam.GetComponent<Shake>();
        duration = Random.Range(minShakeDuration, maxShakeDuration);
        StartCoroutine(camShake.ScreenShake(duration, shakeMultiplier));
        //shakeMultiplier = Random.Range(1, 1.1);
    }


    void ScreamingPainEvent()
    {
        EventInstance screamingPainEvent = RuntimeManager.CreateInstance(PlayerScreamEventPath);
        RuntimeManager.AttachInstanceToGameObject(screamingPainEvent, transform, GetComponent<Rigidbody>());

        screamingPainEvent.start();
        screamingPainEvent.release();

    }

    void WhooshEvent()
    {
        EventInstance whooshEvent = RuntimeManager.CreateInstance(WhooshEventPath);
        RuntimeManager.AttachInstanceToGameObject(whooshEvent, transform, GetComponent<Rigidbody>());

        whooshEvent.start();
        whooshEvent.release();
    }
}

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

    void HitTree()
    {
        EventInstance hitTreeEvent = RuntimeManager.CreateInstance(HitTreeEventPath);
        RuntimeManager.AttachInstanceToGameObject(hitTreeEvent, transform, GetComponent<Rigidbody>());

        hitTreeEvent.start();
        hitTreeEvent.release();
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

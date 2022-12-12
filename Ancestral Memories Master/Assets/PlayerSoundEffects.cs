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

    private PlayerWalk playerWalk;

    private string terrainType = "";
    public bool waterColliding = false;

    private AreaManager areaManager;

    private Rigidbody rigidBody;

    private void Awake()
    {
        waterColliding = false;
        rigidBody = transform.parent.GetComponent<Rigidbody>();
    }


    void PlayWalkEvent()
    {
        if (playerWalk.playerInWater)
        {
            terrainType = "Water";
        }

        if (!playerWalk.playerInWater)
        {
            if (areaManager.currentRoom != "Outside")
            {
                terrainType = "Rock";
            }
            else
            {
                terrainType = "Grass";
            }
        } 

        EventInstance walkEvent = RuntimeManager.CreateInstance(WalkEventPath);
        walkEvent.setParameterByNameWithLabel("TerrainType", terrainType);
        RuntimeManager.AttachInstanceToGameObject(walkEvent, transform, rigidBody);

        walkEvent.start();
        walkEvent.release();
    }

    void DrownEvent()
    {
        EventInstance drownEvent = RuntimeManager.CreateInstance(DrownEventPath);
        RuntimeManager.AttachInstanceToGameObject(drownEvent, transform, rigidBody);

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
        RuntimeManager.AttachInstanceToGameObject(walkEvent, transform, rigidBody);

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
        RuntimeManager.AttachInstanceToGameObject(hitTreeEvent, transform, rigidBody);

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
        RuntimeManager.AttachInstanceToGameObject(screamingPainEvent, transform, rigidBody);

        screamingPainEvent.start();
        screamingPainEvent.release();

    }

    void WhooshEvent()
    {
        EventInstance whooshEvent = RuntimeManager.CreateInstance(WhooshEventPath);
        RuntimeManager.AttachInstanceToGameObject(whooshEvent, transform, rigidBody);

        whooshEvent.start();
        whooshEvent.release();
    }
}

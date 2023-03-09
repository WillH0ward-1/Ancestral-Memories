using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class WindObjectPool : MonoBehaviour
{
    public static WindObjectPool windObjectPool;

    [SerializeField] private GameObject windZonePrefab;
    [SerializeField] private int poolSize = 4;
    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private float lifetime = 5f;

    private List<GameObject> windZonePool = new List<GameObject>();
    [SerializeField] private List<Transform> activeWindZones = new List<Transform>();

    public Player player;

    public WeatherControl weather;

    float radiusFactor = 1;
    public float minRadius = 10;
    public float maxRadius = 20;

    private void Awake()
    {
        if (windObjectPool == null)
        {
            windObjectPool = this;
            FillPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FillPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject windZoneObject = Instantiate(windZonePrefab, transform);
            windZoneObject.SetActive(false);
            WindZoneObject windZone = windZoneObject.GetComponent<WindZoneObject>();
            windZone.Initialize(GetRandomPositionAround(player.transform, radiusFactor));
            windZonePool.Add(windZoneObject);
        }
    }

    private IEnumerator SpawnWindZoneBuffer()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            if (activeWindZones.Count < poolSize)
            {
                GameObject windZoneObject = GetWindZoneObject();
                StartCoroutine(ReturnWindZoneBuffer(windZoneObject));
            }

        }
    }

    private IEnumerator ReturnWindZoneBuffer(GameObject windZoneObject)
    {
        EventInstance wind3DInstance = RuntimeManager.CreateInstance(windEventRef);
        RuntimeManager.AttachInstanceToGameObject(wind3DInstance, windZoneObject.transform);

        wind3DInstance.start();
        wind3DInstance.release();

        yield return new WaitForSeconds(lifetime);

        if (activeWindZones.Contains(windZoneObject.transform))
        {
            wind3DInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            ReturnWindZoneObject(windZoneObject);
        }
    }

    public IEnumerator WaitLifeTime(EventInstance wind3DInstance)
    {
        bool active = true;
        float elapsedTime = 0f;

        while (active && elapsedTime < lifetime)
        {
            wind3DInstance.setParameterByName("WindStrength", weather.windStrength);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        active = false;
    }

    public EventReference windEventRef;


    public GameObject GetWindZoneObject()
    {
        if (windZonePool.Count > 0)
        {
            radiusFactor = Random.Range(minRadius, maxRadius);
            GameObject windZoneObject = windZonePool[0];
            WindZoneObject windZone = windZoneObject.GetComponent<WindZoneObject>();
            windZone.Initialize(GetRandomPositionAround(player.transform, radiusFactor));
            activeWindZones.Add(windZone.transform);
            windZonePool.RemoveAt(0);
            return windZoneObject;
        }

        return null;
    }

    public Vector3 GetRandomPositionAround(Transform centerTransform, float radius)
    {
        Vector3 randomPoint = Random.insideUnitSphere * radius;
        Vector3 position = centerTransform.position + randomPoint;

        return position;
    }

    public void ReturnWindZoneObject(GameObject windZoneObject)
    {
        WindZoneObject windZone = windZoneObject.GetComponent<WindZoneObject>();
        windZone.ReturnToPool();
        activeWindZones.Remove(windZone.transform);
        windZonePool.Add(windZoneObject);
    }

    public void RemoveActiveWindZone(Transform windZoneTransform)
    {
        activeWindZones.Remove(windZoneTransform);
    }

    public void AddActiveWindZone(Transform windZoneTransform)
    {
        activeWindZones.Add(windZoneTransform);
    }

    private void Start()
    {
        StartCoroutine(SpawnWindZoneBuffer());
    }
}
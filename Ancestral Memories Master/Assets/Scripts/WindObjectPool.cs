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
        EventInstance windSFXInstance = RuntimeManager.CreateInstance(windEventRef);
        RuntimeManager.AttachInstanceToGameObject(windSFXInstance, windZoneObject.transform);
        windSFXInstance.start();
        weather.StartCoroutine(weather.UpdateWind(windZoneObject, windSFXInstance));

        yield return new WaitForSeconds(lifetime);

        if (activeWindZones.Contains(windZoneObject.transform))
        {
            windSFXInstance.release();
            windSFXInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            ReturnWindZoneObject(windZoneObject);
        }
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
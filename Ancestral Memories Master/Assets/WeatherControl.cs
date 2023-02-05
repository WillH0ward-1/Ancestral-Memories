using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;

public class WeatherControl : MonoBehaviour
{
    public EventReference wind2DEvent;

    public GameObject windZone;

    public float windStrength = 0;
    [SerializeField] private float targetWindStrength;
    [SerializeField] private float targetLeafShakeStrength;

    [SerializeField] private float targetLeafSpeed;
    public bool wind2DActive;

    [SerializeField] private Renderer[] windAffectedRenderers;

    [SerializeField] private Transform windZones;
    [SerializeField] private int maxWindZoneInstances = 4;

    [SerializeField] private Transform parent;
    [SerializeField] private Player player;

    [SerializeField] LerpParams lerpParams;

    //[SerializeField] private List<Renderer> windAffectedRenderers;

    // Start is called before the first frame update

    public List<Transform> windAffectedRendererList = new List<Transform>();
    public List<Transform> activeWindZones = new List<Transform>();

    private void Awake()
    {
        parent = player.transform;
        func = Lerp.GetLerpFunction(lerpParams.lerpType);
    }

    void Start()
    {
        ListCleanup(windAffectedRendererList);

        //
        EventInstance windAudio2DInstance = RuntimeManager.CreateInstance(wind2DEvent);
        //windStrength = 0;

        StartCoroutine(WindStrength(windAudio2DInstance));

        StartCoroutine(SpawnBuffer());
    }

    void ListCleanup(List<Transform> list)
    {
        for (var i = list.Count - 1; i > -1; i--)
        {
            if (list[i] == null)
                list.RemoveAt(i);
        }
    }

    [SerializeField] private float minSpawnBuffer = 5;
    [SerializeField] private float maxSpawnBuffer = 15;

    [SerializeField] private float minLifeTime = 15;
    [SerializeField] private float maxLifeTime = 40;


    private IEnumerator SpawnBuffer()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnBuffer, maxSpawnBuffer));

        if (activeWindZones.Count <= maxWindZoneInstances)
        {
            SpawnWindZone();
        }

        yield break;
    }

    [SerializeField] private float spawnRadius = 5;

    private void SpawnWindZone()
    {
        StartCoroutine(SpawnBuffer());
     
        GameObject windZoneObject = Instantiate(windZone, player.transform.position, Quaternion.identity, windZones);

        activeWindZones.Add(windZoneObject.transform);

        if (activeWindZones.Count > maxWindZoneInstances)
        {
            activeWindZones.Remove(windZoneObject.transform);
            Destroy(windZoneObject);
        }
        else
        {

            Vector3 newPosition = (Random.insideUnitSphere * spawnRadius) + player.transform.position;
            windZoneObject.transform.position = newPosition;

            EventInstance wind3DInstance = windZoneObject.transform.GetComponent<StudioEventEmitter>().EventInstance;

            StartCoroutine(UpdateWind(windZoneObject, wind3DInstance));
            StartCoroutine(WindTimeout(windZoneObject));
        }
    }

    private IEnumerator UpdateWind(GameObject windZoneObject, EventInstance instance)
    {
        bool active = true;

        while (active)
        {
            if (windZoneObject == null)
            {
                active = false;
            }

            instance.setParameterByName("WindStrength", windStrength);
            yield return null;
        }

        yield break;
    }

    private void Update()
    {
        windStrength = targetWindStrength;
    }

    private IEnumerator WindTimeout(GameObject instance)
    {
        yield return new WaitForSeconds(Random.Range(minLifeTime, maxLifeTime));

        Destroy(instance);
        ListCleanup(activeWindZones);
        //StartCoroutine(SpawnBuffer());
        yield break;
    }

    private IEnumerator WindStrength(EventInstance wind2DInstance)
    {
        wind2DActive = true;

        wind2DInstance.start();

        while (wind2DActive)
        {

            wind2DInstance.setParameterByName("WindStrength", windStrength);

            Debug.Log("WindStrength:" + windStrength);

            foreach (Transform t in windAffectedRendererList)
            {
                foreach (Material m in t.GetComponentInChildren<Renderer>().sharedMaterials)
                {
                    m.SetFloat("_NoiseFactor", targetLeafShakeStrength);
                    m.SetFloat("_WindSpeed", targetLeafSpeed);
                }
            }

            yield return null;
        }

        if (!wind2DActive)
        {
            StartCoroutine(StopWind(wind2DInstance));
            yield break;
        }
    }

    private IEnumerator StopWind(EventInstance windSFX)
    {
        windSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        wind2DActive = false;
        yield break;
    }

    [SerializeField] float newMin = 0;
    [SerializeField] float newMax = 1;

    [SerializeField] float leafSpeedMin = 0;
    [SerializeField] float leafSpeedMax = 1;

    [SerializeField] float leafShakeMin = 1;
    [SerializeField] float leafShakeMax = 5;

    private void OnEnable() => player.OnFaithChanged += WindStrength;
    private void OnDisable() => player.OnFaithChanged -= WindStrength;

    private System.Func<float, float> func;

    private void WindStrength(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float output = Mathf.Lerp(newMin, newMax, func(t));
        float leafOutput = Mathf.Lerp(leafShakeMin, leafShakeMax, func(t));
        float leafSpeedOutput = Mathf.Lerp(leafShakeMin, leafShakeMax, func(t));

        targetWindStrength = output;
        targetLeafShakeStrength = leafOutput;
        targetLeafSpeed = leafSpeedOutput;
    }

}

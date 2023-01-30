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

    public bool wind2DActive;

    [SerializeField] private Renderer[] windAffectedRenderers;

    [SerializeField] private Transform windZones;

    [SerializeField] private Transform parent;
    [SerializeField] private Player player;

    //[SerializeField] private List<Renderer> windAffectedRenderers;

    // Start is called before the first frame update

    public List<Transform> windAffectedRendererList = new List<Transform>();

    private void Awake()
    {
        parent = player.transform;
    }

    void Start()
    {
        ListCleanup();

        //
        EventInstance windAudio2DInstance = RuntimeManager.CreateInstance(wind2DEvent);
        //windStrength = 0;
        StartCoroutine(WindStrength(windAudio2DInstance));

        StartCoroutine(SpawnBuffer());
    }

    void ListCleanup()
    {
        for (var i = windAffectedRendererList.Count - 1; i > -1; i--)
        {
            if (windAffectedRendererList[i] == null)
                windAffectedRendererList.RemoveAt(i);
        }
    }

    [SerializeField] private float minSpawnBuffer = 5;
    [SerializeField] private float maxSpawnBuffer = 15;

    [SerializeField] private float minLifeTime = 15;
    [SerializeField] private float maxLifeTime = 40;

    private IEnumerator SpawnBuffer()
    {
        yield return new WaitForSeconds(Random.Range(minSpawnBuffer, maxSpawnBuffer));

        SpawnWindZone();

        yield break;
    }

    [SerializeField] private float spawnRadius = 5;

    private void SpawnWindZone()
    {
        StartCoroutine(SpawnBuffer());

     
        GameObject windZoneObject = Instantiate(windZone, player.transform.position, Quaternion.identity, windZones);
        windZoneObject.transform.SetParent(player.transform);

        Vector3 newPosition = Random.insideUnitCircle * spawnRadius;
        windZoneObject.transform.position = newPosition;

        EventInstance wind3DInstance = windZoneObject.transform.GetComponent<StudioEventEmitter>().EventInstance;

        StartCoroutine(UpdateWind(windZoneObject, wind3DInstance));
        StartCoroutine(WindTimeout(windZoneObject));
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

        //StartCoroutine(SpawnBuffer());
        yield break;
    }

    private IEnumerator WindStrength(EventInstance wind2DInstance)
    {
        wind2DActive = true;

        // wind2DInstance.start();

        while (wind2DActive)
        {

            wind2DInstance.setParameterByName("WindStrength", windStrength);

            Debug.Log("WindStrength:" + windStrength);

            foreach (Transform t in windAffectedRendererList)
            {
                foreach (Material m in t.GetComponentInChildren<Renderer>().sharedMaterials)
                {
                    m.SetFloat("_NoiseFactor", targetLeafShakeStrength);
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

    [SerializeField] float leafShakeMin = 0;
    [SerializeField] float leafShakeMax = 100;

    private void OnEnable() => player.OnFaithChanged += WindStrength;
    private void OnDisable() => player.OnFaithChanged -= WindStrength;

    private void WindStrength(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float output = Mathf.Lerp(newMin, newMax, t);
        float leafOutput = Mathf.Lerp(leafShakeMin, leafShakeMax, t);

        targetWindStrength = output;
        targetLeafShakeStrength = leafOutput;
    }

}

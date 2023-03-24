using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;

public class WeatherControl : MonoBehaviour
{
    //public EventReference wind2DEvent;

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

    public List<Transform> windAffectedRendererList = new List<Transform>();
    public List<Transform> activeWindZones = new List<Transform>();

    private void Awake()
    {
        parent = player.transform;
       // func = Lerp.GetLerpFunction(lerpParams.lerpType);
    }

    void Start()
    {
        ListCleanup(windAffectedRendererList);
        //EventInstance windAudio2DInstance = RuntimeManager.CreateInstance(wind2DEvent);

    }

    [SerializeField] private float minSpawnBuffer = 5;
    [SerializeField] private float maxSpawnBuffer = 15;

    [SerializeField] private float minLifeTime = 15;
    [SerializeField] private float maxLifeTime = 40;

    [SerializeField] private float spawnRadius = 5;


    void ListCleanup(List<Transform> list)
    {
        for (var i = list.Count - 1; i > -1; i--)
        {
            if (list[i] == null)
                list.RemoveAt(i);
        }
    }

    public IEnumerator UpdateWind()
    {
        bool active = true;

        while (active)
        {
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

        active = false;

        yield break;
    }

    private void Update()
    {
        windStrength = targetWindStrength;
    }

    private IEnumerator WindTimeout(GameObject instance, EventInstance wind3DInstance)
    {
        yield return new WaitForSeconds(Random.Range(minLifeTime, maxLifeTime));

        wind3DInstance.release();
        Destroy(instance);
        ListCleanup(activeWindZones);
        //StartCoroutine(SpawnBuffer());
        yield break;
    }

    [SerializeField] float windMin = 0f;
    [SerializeField] float windMax = 1f;

    [SerializeField] float leafSpeedMin = 0.1f;
    [SerializeField] float leafSpeedMax = 0.75f;

    [SerializeField] float leafShakeMin = 0.5f;
    [SerializeField] float leafShakeMax = 3f;

    private void OnEnable() => player.OnFaithChanged += WindStrength;
    private void OnDisable() => player.OnFaithChanged -= WindStrength;

    //private System.Func<float, float> func;

    private void WindStrength(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float windOutput = Mathf.Lerp(windMin, windMax, t);
        float leafOutput = Mathf.Lerp(leafShakeMin, leafShakeMax, t);
        float leafSpeedOutput = Mathf.Lerp(leafShakeMin, leafShakeMax, t);

        targetWindStrength = windOutput;
        targetLeafShakeStrength = leafOutput;
        targetLeafSpeed = leafSpeedOutput;

        //targetWindStrength = OscillateWindStrength(windOutput, windMin, windMax);
        //targetLeafShakeStrength = OscillateWindStrength(leafOutput, leafShakeMin, leafShakeMax);
        //targetLeafSpeed = OscillateWindStrength(leafSpeedOutput, leafSpeedMin, leafSpeedMax);
    }

    /*
     * 
     * Unused oscillator 
     * 
     * [SerializeField] private float oscillationSpeed = 0.5f; // adjust this to change the range of oscillation
     * 
    private float OscillateWindStrength(float targetValue, float min, float max)
    {
        float oscillationRange = Random.Range(-min, max);

        float time = Time.time * oscillationSpeed;
        float amplitude = oscillationRange * 0.5f;
        float offset = targetValue + amplitude;

        return Mathf.Sin(time) * amplitude + offset;
    }
    */
}

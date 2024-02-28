using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using FMODUnity;
// using FMOD.Studio;
using System.Linq;

public class WeatherControl : MonoBehaviour
{
    public GameObject windZone;

    public float windStrength = 0;
    [SerializeField] private float targetWindStrength;
    [SerializeField] private float targetLeafShakeStrength;

    [SerializeField] private float targetLeafSpeed;

    [SerializeField] private Renderer[] windAffectedRenderers;

    [SerializeField] private Transform windZones;

                                                                                                                                    
    public Player player;

    [SerializeField] LerpParams lerpParams;

    public List<Transform> windAffectedRendererList = new List<Transform>();
    private Queue<GameObject> windZonePool = new Queue<GameObject>(); // Correctly using Queue here


    [SerializeField] private int maxWindZoneInstances = 4;
    [SerializeField] private float spawnRadius = 20f;
    [SerializeField] private float minLifeTime = 10f;
    [SerializeField] private float maxLifeTime = 30f;

    [SerializeField] private float minSpawnBuffer = 5;
    [SerializeField] private float maxSpawnBuffer = 15;

    private float currentAmpAttack;
    private float currentAmpDecay;
    private float currentAmpSustain;
    private float currentAmpRelease;

    public void InitializeWindZonePool()
    {
        ListCleanup(windAffectedRendererList);

        for (int i = 0; i < maxWindZoneInstances; i++)
        {
            GameObject windZoneInstance = Instantiate(windZone);
            AudioWindManager audioManager = windZoneInstance.GetComponentInChildren<AudioWindManager>();
            audioManager.InitCsoundObj();
            audioManager.player = player;
            windZoneInstance.SetActive(false);
            windZonePool.Enqueue(windZoneInstance);
        }

        StartCoroutine(SpawnWindZones());
    }

    private IEnumerator SpawnWindZones()
    {
        while (true)
        {
            if (windZonePool.Count > 0)
            {
                SpawnWindZone();
            }
            yield return new WaitForSeconds(minLifeTime);
        }
    }

    private void SpawnWindZone()
    {
        if (windZonePool.Count == 0) return;

        GameObject windZoneInstance = windZonePool.Dequeue();
        Vector3 spawnPosition = player.transform.position + Random.insideUnitSphere * spawnRadius;
        spawnPosition.y = player.transform.position.y;
        windZoneInstance.transform.position = spawnPosition;
        windZoneInstance.transform.SetParent(player.transform);
        windZoneInstance.SetActive(true);
        AudioWindManager audioManager = windZoneInstance.GetComponentInChildren<AudioWindManager>();
        audioManager.SetPlayState(true);
        audioManager.StartCoroutine(audioManager.AdjustWindParametersBasedOnFaith());

        StartCoroutine(WindZoneLifetime(windZoneInstance, Random.Range(minLifeTime, maxLifeTime), audioManager));
    }


    private IEnumerator WindZoneLifetime(GameObject windZoneInstance, float lifetime, AudioWindManager audioWindManager)
    {
        yield return new WaitForSeconds(lifetime);

        audioWindManager.SetPlayState(false);
        currentAmpRelease = audioWindManager.GetADSR(AudioWindManager.ADSRStage.Release);
        yield return new WaitForSeconds(currentAmpRelease);

        windZoneInstance.SetActive(false);
        windZonePool.Enqueue(windZoneInstance);
    }


    void ListCleanup(List<Transform> list)
    {
        for (var i = list.Count - 1; i > -1; i--)
        {
            if (list[i] == null)
                list.RemoveAt(i);
        }
    }

    bool active;

    public IEnumerator UpdateWindRenderers()
    {
        active = true;

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

    [SerializeField] float windMin = 0f;
    [SerializeField] float windMax = 1f;

    [SerializeField] float leafSpeedMin = 0.1f;
    [SerializeField] float leafSpeedMax = 0.75f;

    [SerializeField] float leafShakeMin = 0.5f;
    [SerializeField] float leafShakeMax = 3f;

    private void OnEnable() => player.OnFaithChanged += WindStrength;
    private void OnDisable() => player.OnFaithChanged -= WindStrength;

    private void WindStrength(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float windOutput = Mathf.Lerp(windMin, windMax, t);
        float leafOutput = Mathf.Lerp(leafShakeMin, leafShakeMax, t);
        float leafSpeedOutput = Mathf.Lerp(leafShakeMin, leafShakeMax, t);

        targetWindStrength = windOutput;
        targetLeafShakeStrength = leafOutput;
        targetLeafSpeed = leafSpeedOutput;
    }
}

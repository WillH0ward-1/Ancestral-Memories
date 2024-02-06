using System.Collections;
using UnityEngine;

public class RainControl : MonoBehaviour
{
    private MapObjGen mapObjGen;
    [SerializeField] private WeatherControl weather;
    public ParticleSystem rainParticles;
    [SerializeField] private AreaManager areaManager;
    public LerpTerrain lerpTerrain;
    private ParticleSystem.EmissionModule emission;
    public float emissionMultiplier = 128f;
    public SeasonManager seasonManager;
    public Material rainMaterial; // Material with the 'Tint' parameter

    [SerializeField] private float minDroughtDuration = 30f;
    [SerializeField] private float maxDroughtDuration = 60f;

    public float rainDuration;
    public float minRainDuration = 10f;
    public float maxRainDuration = 60f;

    [SerializeField] private CloudControl clouds;
    public float rainDanceMultiplier = 1f;
    public float minRainDanceMultiplier = 1f;
    public float maxRainDanceMultiplier = 5f;

    public bool isRaining = false;
    public bool drought = false;
    public float minWaitForRain = 0f;
    public float maxWaitForRain = 300f;

    private bool isInside;
    [SerializeField] private bool triggerDrought;
    public Player player;
    public CharacterBehaviours behaviours;
    public float maxEmissionOverTime;
    private float droughtThreshold = 0;
    [SerializeField] private float droughtThresholdDivisor = 2;

    private Renderer renderer;
    private MaterialPropertyBlock propBlock;

    private Coroutine rainCheckCoroutine;
    private Coroutine rainCoroutine;
    private Coroutine droughtCoroutine;

    private void Awake()
    {
        transform.SetParent(player.transform);
        behaviours = player.GetComponentInChildren<CharacterBehaviours>();
        mapObjGen = FindObjectOfType<MapObjGen>();
        droughtThreshold = player.maxStat / droughtThresholdDivisor;

        renderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        emission = rainParticles.emission;
        emission.enabled = false;
        rainCheckCoroutine = StartCoroutine(ChanceOfRain());
    }

    public IEnumerator ChanceOfRain()
    {
        while (true) // Repeat indefinitely
        {
            if (!QuestManager.Instance.IsShamanQuestActive())
            {
                if (!isRaining)
                {
                    float time = 0;
                    float rainWaitDuration = Random.Range(minWaitForRain, maxWaitForRain);

                    while (time < rainWaitDuration)
                    {
                        time += Time.deltaTime;
                        yield return null;
                    }

                    if (player.faith >= droughtThreshold && areaManager.currentRoom == "Outside")
                    {
                        if (rainCoroutine != null)
                            StopCoroutine(rainCoroutine);
                        rainCoroutine = StartCoroutine(StartRaining());
                        yield break; // Exit the coroutine after starting the rain
                    }
                    else if (player.faith <= droughtThreshold && triggerDrought && seasonManager._currentSeason != SeasonManager.Season.Winter)
                    {
                        if (droughtCoroutine != null)
                            StopCoroutine(droughtCoroutine);
                        droughtCoroutine = StartCoroutine(Drought());
                        yield break; // Exit the coroutine after starting the drought
                    }
                }
            }
            yield return null; // Wait for the next frame before continuing
        }
    }

    public IEnumerator Drought()
    {
        drought = true;
        float time = 0;
        float droughtDuration = Random.Range(minDroughtDuration, maxDroughtDuration);

        StartCoroutine(lerpTerrain.LerpDrought(true));

        mapObjGen.KillAllTreeProduce();

        while (time < droughtDuration)
        {
            if (seasonManager._currentSeason == SeasonManager.Season.Winter)
            {
                break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        rainCheckCoroutine = StartCoroutine(ChanceOfRain());
        
    }

    public IEnumerator StartRaining()
    {
        clouds.OverrideCloudPower(clouds.cloudPersistanceMax);
        isRaining = true;
        rainDuration = Random.Range(minRainDuration, maxRainDuration);

        mapObjGen.ReviveTreeProduce();

        emission.enabled = true;
        rainParticles.Play();

        if (drought)
        {
            drought = false;
            StartCoroutine(lerpTerrain.LerpDrought(false));
        }

        float time = 0;
        while (time < rainDuration && areaManager.currentRoom == "Outside")
        {
            float rainStrength = Mathf.Lerp(0, maxEmissionOverTime, time / rainDuration);
            emission.rateOverTime = rainStrength * emissionMultiplier;
            time += Time.deltaTime;
            yield return null;
        }

        if (rainCoroutine != null)
            StopCoroutine(rainCoroutine);
        rainCoroutine = StartCoroutine(StopRaining());
    }

    public IEnumerator StopRaining()
    {
        clouds.StopCloudPowerOverride();
        emission.enabled = false;
        isRaining = false;
        rainParticles.Stop();

        rainCheckCoroutine = StartCoroutine(ChanceOfRain());

        yield break;
    }
}

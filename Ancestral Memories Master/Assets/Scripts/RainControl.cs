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
    public SeasonManager seasons;
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
                else if (player.faith <= droughtThreshold && triggerDrought)
                {
                    if (droughtCoroutine != null)
                        StopCoroutine(droughtCoroutine);
                    droughtCoroutine = StartCoroutine(Drought());
                    yield break; // Exit the coroutine after starting the drought
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

        lerpTerrain.ToDesert();
        mapObjGen.KillAllTreeProduce();

        while (time < droughtDuration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        drought = false;
        rainCheckCoroutine = StartCoroutine(ChanceOfRain());
    }

    public IEnumerator StartRaining()
    {
        clouds.OverrideCloudPower(clouds.cloudPersistanceMax);
        isRaining = true;
        rainDuration = Random.Range(minRainDuration, maxRainDuration);

        if (seasons.CurrentSeason == SeasonManager.Season.Winter)
        {
            SnowShape();
            StartCoroutine(LerpMaterialColor(Color.white)); // Lerp to snow color
        }
        else
        {
            RainShape();
            StartCoroutine(LerpMaterialColor(Color.blue)); // Lerp to rain color
        }

        emission.enabled = true;
        rainParticles.Play();

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

    private void SnowShape()
    {
        var main = rainParticles.main;
        main.startSize3D = true;
        main.startSizeX = new ParticleSystem.MinMaxCurve(0.1f, 0.1f);
        main.startSizeY = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
        main.startSizeZ = new ParticleSystem.MinMaxCurve(0.1f, 0.1f);
        // Apply without stopping the particle system
        rainParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
        rainParticles.Play();
    }

    private void RainShape()
    {
        var main = rainParticles.main;
        main.startSize3D = true;
        main.startSizeX = new ParticleSystem.MinMaxCurve(0.1f, 0.1f);
        main.startSizeY = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSizeZ = new ParticleSystem.MinMaxCurve(0.1f, 0.1f);
        // Apply without stopping the particle system
        rainParticles.Stop(withChildren: true, ParticleSystemStopBehavior.StopEmittingAndClear);
        rainParticles.Play();
    }

    private IEnumerator LerpMaterialColor(Color targetColor)
    {
        float lerpDuration = 1f; // Duration to lerp color
        float time = 0f;

        // Get the current properties of the material
        renderer.GetPropertyBlock(propBlock);
        Color startColor = propBlock.GetColor("Tint");

        while (time < lerpDuration)
        {
            Color lerpedColor = Color.Lerp(startColor, targetColor, time / lerpDuration);
            propBlock.SetColor("Tint", lerpedColor);
            renderer.SetPropertyBlock(propBlock);

            time += Time.deltaTime;
            yield return null;
        }

        propBlock.SetColor("Tint", targetColor); // Ensure final color is set
        renderer.SetPropertyBlock(propBlock);
    }
}

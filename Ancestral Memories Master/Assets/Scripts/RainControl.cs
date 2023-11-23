using System.Collections;
using UnityEngine;

public class RainControl : MonoBehaviour
{
    private MapObjGen mapObjGen;
    [SerializeField] private WeatherControl weather;
    public ParticleSystem rainParticles;
    [SerializeField] private AreaManager areaManager;
    public LerpTerrain lerpTerrain;
    public ParticleSystem.EmissionModule emission;
    public float emissionMultiplier = 128f;
    public SeasonManager seasons;
    public Material rainMaterial; // Material with the 'Tint' parameter

    [SerializeField] private float minDroughtDuration = 30f;
    [SerializeField] private float maxDroughtDuration = 60f;

    public float rainDuration;
    public float minRainDuration = 10f;
    public float maxRainDuration = 60f;

    [SerializeField] private float rainStrengthTarget;
    [SerializeField] private float emissionRateOverTime;
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

    private new Renderer renderer;
    private MaterialPropertyBlock propBlock;

    private void Awake()
    {
        transform.parent.SetParent(player.transform);
        behaviours = player.transform.GetComponentInChildren<CharacterBehaviours>();
        mapObjGen = FindObjectOfType<MapObjGen>();
        droughtThreshold = player.maxStat / droughtThresholdDivisor;

        renderer = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
    }

    void Start()
    {
        emission = rainParticles.emission;
        emission.enabled = false;
        StartCoroutine(ChanceOfRain());
    }

    public IEnumerator ChanceOfRain()
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

            if (!isRaining && player.faith >= droughtThreshold && areaManager.currentRoom == "Outside")
            {
                StartCoroutine(StartRaining());
            }
            else if (player.faith <= droughtThreshold && triggerDrought)
            {
                StartCoroutine(Drought());
            }
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
        StartCoroutine(ChanceOfRain());
    }

    public IEnumerator StartRaining()
    {
        clouds.OverrideCloudPower(clouds.cloudPersistanceMax);
        emissionRateOverTime = 0;
        emission.rateOverTime = emissionRateOverTime * rainDanceMultiplier;
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

        StartCoroutine(StopRaining());
    }

    public IEnumerator StopRaining()
    {
        clouds.StopCloudPowerOverride();
        emissionRateOverTime = 0;
        emission.enabled = false;
        isRaining = false;
        rainParticles.Stop();

        StartCoroutine(ChanceOfRain());

        yield break;
    }

    private void SnowShape()
    {
        var main = rainParticles.main;
        main.startSize3D = true;
        main.startSizeX = new ParticleSystem.MinMaxCurve(0.1f, 0.1f);
        main.startSizeY = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
        main.startSizeZ = new ParticleSystem.MinMaxCurve(0.1f, 0.1f);
    }

    private void RainShape()
    {
        var main = rainParticles.main;
        main.startSize3D = true;
        main.startSizeX = new ParticleSystem.MinMaxCurve(0.1f, 0.1f);
        main.startSizeY = new ParticleSystem.MinMaxCurve(1f, 2f);
        main.startSizeZ = new ParticleSystem.MinMaxCurve(0.1f, 0.1f);
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

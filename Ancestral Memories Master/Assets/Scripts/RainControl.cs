using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainControl : MonoBehaviour
{

    [SerializeField] private WeatherControl weather;

    public ParticleSystem rainParticles;

    [SerializeField] private AreaManager areaManager;

    public LerpTerrain lerpTerrain;

    public ParticleSystem.EmissionModule emission;
    public float emissionMultiplier = 128f;

    void Start()
    {
        emission = rainParticles.emission;
        emission.enabled = false;
        isRaining = false;

        StartCoroutine(ChanceOfRain());
    }

    public bool isRaining = false;
    public bool drought = false;

    public float minWaitForRain = 0f;
    public float maxWaitForRain = 300f;

    private bool isInside;
    [SerializeField] private bool triggerDrought;

    public Player player;
    public CharacterBehaviours behaviours;

    public float maxEmissionOverTime;

    private void Awake()
    {

        transform.parent.SetParent(player.transform);

        behaviours = player.transform.GetComponentInChildren<CharacterBehaviours>();

        //weather = transform.GetComponent<WeatherControl>();

    }

    public IEnumerator ChanceOfRain()
    {
        if (!isRaining)
        {
            float time = 0;
            float rainWaitDuration = Random.Range(minWaitForRain, maxWaitForRain);
            float rainDuration = 0;
            float maxRainDuration = 10f; // Set this value to the desired maximum rain duration

            while (time <= rainWaitDuration)
            {
                time += Time.deltaTime;

                // Check if the rain duration has elapsed
                if (isRaining)
                {
                    rainDuration += Time.deltaTime;
                    if (rainDuration >= maxRainDuration)
                    {
                        break;
                    }
                }

                yield return null;
            }

            if (time >= rainWaitDuration)
            {
                if (isRaining || areaManager.currentRoom != "Outside")
                {
                    StartCoroutine(ChanceOfRain());
                    yield break;
                }
                else if (!isRaining && player.faith >= 50)
                {
                    StartCoroutine(StartRaining());
                }
                else if (player.faith <= 50 && triggerDrought)
                {
                    StartCoroutine(Drought());
                    yield break;
                }

            }

            yield break;
        }
    }

    public float minDroughtDuration = 30f;
    public float maxDroughtDuration = 60f;

    public IEnumerator Drought()
    {
        drought = true;

        float time = 0;
        float droughtDuration = Random.Range(minDroughtDuration, maxDroughtDuration);

        StartCoroutine(lerpTerrain.ToDesert(15f));

        while (time < droughtDuration)
        {
            time += Time.deltaTime;

            if (time >= droughtDuration)
            {
                StartCoroutine(ChanceOfRain());
                yield break;
            }

            yield return null;
        }
    
    }

    [SerializeField] private float rainDuration;

    float rainStrength;


    public float minRainDuration = 10f;
    public float maxRainDuration = 60f;

    [SerializeField] private float rainStrengthTarget;
    float emissionRate;
    [SerializeField] public float emissionRateOverTime;

    public CloudControl clouds;

    public float rainDanceMultiplier = 1f;
    public float minRainDanceMultiplier = 1f;
    public float maxRainDanceMultiplier = 5f;

    public IEnumerator StartRaining()
    {

        clouds.OverrideCloudPower(clouds.cloudPersistanceMax);

        emissionRateOverTime = 0;
        emission.rateOverTime = emissionRateOverTime * rainDanceMultiplier;

        isRaining = true;

        if (drought)
        {
            drought = false;
        }

        rainDuration = Random.Range(minRainDuration, maxRainDuration);

        //rainStrength = Random.Range(minRainStrength, maxRainStrength);
        StartCoroutine(lerpTerrain.ToWetOasis(15f));

        float time = 0;

        emission.enabled = true;
        rainParticles.Play();

        while (time <= 1f && areaManager.currentRoom == "Outside")
        {
            time += Time.deltaTime / rainDuration;

            rainStrengthTarget = maxEmissionOverTime;
            emission.rateOverTime = Mathf.Lerp(emissionRateOverTime, rainStrengthTarget, time);
            maxEmissionOverTime = weather.windStrength * emissionMultiplier + 1; // (Yields a range of 0 - 1 * emissionMultiplier 

            yield return null;
        }

        if (time >= 1f || areaManager.currentRoom != "Outside")
        {
            StartCoroutine(StopRaining(true));
            yield break;
        }
    }

    private bool retrigger = false;

    private float grassDryTime = 30;

    public IEnumerator StopRaining(bool retrigger)
    {
        clouds.StopCloudPowerOverride();

        emissionRateOverTime = 0;
        emission.enabled = false;
        isRaining = false;
        rainParticles.Stop();
        StartCoroutine(lerpTerrain.ToOasis(15f));

        if (retrigger)
        {
            StartCoroutine(ChanceOfRain());
            yield break;
        } else
        {
            yield break;
        }
    }
}

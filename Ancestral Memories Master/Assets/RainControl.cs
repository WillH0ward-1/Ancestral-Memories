using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainControl : MonoBehaviour
{

    [SerializeField] private WeatherControl weather;

    [SerializeField] private ParticleSystem rainParticles;

    [SerializeField] AreaManager areaManager;

    public LerpTerrain lerpTerrain;

    private ParticleSystem.EmissionModule emission;

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
    private bool triggerDrought;


    private void Awake()
    {
        triggerDrought = false;

        //weather = transform.GetComponent<WeatherControl>();

    }
    
    public IEnumerator ChanceOfRain()
    {
        triggerDrought = false;

        if (isRaining || areaManager.currentRoom != "Outside")
        {
            yield break;
        }

        if (!isRaining)
        {
            float time = 0;
            float rainWaitDuration = Random.Range(minWaitForRain, maxWaitForRain);

            int trigger = Random.Range(0, 1);

            if (trigger == 1)
            {
                //triggerDrought = true;
            }

            while (time <= rainWaitDuration)
            {
                time += Time.deltaTime;
                yield return null;
            }

            if (time >= rainWaitDuration)
            {
                if (triggerDrought)
                {
                    //StartCoroutine(lerpTerrain.ToDesert(15f));
                    yield break;
                }
                else if (!triggerDrought)
                {
                    StartCoroutine(StartRaining());
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

        StartCoroutine(lerpTerrain.ToDesert(15));

        while (time < droughtDuration)
        {
            time += Time.deltaTime;

            if (time >= droughtDuration)
            {
                StartCoroutine(StartRaining());
                yield break;
            }

            yield return null;
        }
    
    }

    [SerializeField] private float rainDuration;

    float rainStrength;


    public float minRainDuration = 10f;
    public float maxRainDuration = 60f;

    float minRainStrength = 1;
    float maxRainStrength = 100;

    [SerializeField] private float rainStrengthTarget;
    float emissionRate;
    [SerializeField] float emissionRateOverTime;

    public IEnumerator StartRaining()
    {
        if (areaManager.currentRoom == "Outside")
        {
            emissionRateOverTime = 0;

            emission.rateOverTime = emissionRateOverTime;

            emission.enabled = true;
            isRaining = true;
            drought = false;

            rainDuration = Random.Range(minRainDuration, maxRainDuration);


            //rainStrength = Random.Range(minRainStrength, maxRainStrength);
            //StartCoroutine(lerpTerrain.ToWetOasis(15));

            float time = 0;

            while (time <= 1f && areaManager.currentRoom == "Outside")
            {
                time += Time.deltaTime / rainDuration;

                rainStrengthTarget = weather.windStrength * 200;
                emission.rateOverTime = Mathf.Lerp(emissionRateOverTime, rainStrengthTarget, time);

                yield return null;
            }

            if (time >= 1f || areaManager.currentRoom != "Outside")
            {
                StartCoroutine(StopRaining(true));
                yield break;
            }
        }
        else if (areaManager.currentRoom != "Outside")
        {
            yield break;
        }
    }

    private bool retrigger = false;

    private float grassDryTime = 30;

    public IEnumerator StopRaining(bool retrigger)
    {
        emissionRateOverTime = 0;
        emission.enabled = false;
        isRaining = false;
        //StartCoroutine(lerpTerrain.ToOasis(15f));

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

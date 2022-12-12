using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainControl : MonoBehaviour
{
    [SerializeField] private ParticleSystem rainParticles;

    [SerializeField] AreaManager areaManager;

    public LerpTerrain lerpTerrain;

    void Start()
    {
        ParticleSystem.EmissionModule emission = rainParticles.emission;
        emission.enabled = false;

        isRaining = false;

        StartCoroutine(ChanceOfRain(rainParticles.emission));
    }

    public bool isRaining = false;
    public bool drought = false;

    public float minRainDuration = 10f;
    public float maxRainDuration = 60f;

    public float minWaitForRain = 0f;
    public float maxWaitForRain = 300f;

    private bool isInside;
    private bool triggerDrought;

    private void Awake()
    {
        triggerDrought = false;
    }
    
    public IEnumerator ChanceOfRain(ParticleSystem.EmissionModule emission)
    {
        triggerDrought = false;
        if (isRaining)
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
                triggerDrought = true;
            }

            while (time < rainWaitDuration)
            {
                time += Time.deltaTime;
                yield return null;
            }

            if (time >= rainWaitDuration)
            {
                if (triggerDrought)
                {
                    StartCoroutine(lerpTerrain.ToDesert(15f));
                    yield break;
                }
                else if (!triggerDrought)
                {
                    StartCoroutine(StartRaining(emission));
                    yield break;
                }
            }
        } 
    }

    public IEnumerator RainStrength()
    {
        yield return null;

    }

    public float minDroughtDuration = 30f;
    public float maxDroughtDuration = 60f;

    public IEnumerator Drought(ParticleSystem.EmissionModule emission)
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
                StartCoroutine(StartRaining(emission));
                yield break;
            }

            yield return null;
        }
    
    }

    float rainDuration;

    public IEnumerator StartRaining(ParticleSystem.EmissionModule emission)
    {
        emission.enabled = true;
        isRaining = true;
        drought = false;

        float time = 0;

        rainDuration = Random.Range(minRainDuration, maxRainDuration);

        StartCoroutine(lerpTerrain.ToWetOasis(15));

        while (time < rainDuration)
        {
            if (isRaining && areaManager.currentRoom == "InsideCave" || drought)
            {

                StartCoroutine(StopRaining(emission, false));
                
                yield break;
            }

            time += Time.deltaTime;
            yield return null;
        }

        if (time >= rainDuration && areaManager.currentRoom != "InsideCave")
        {
            StartCoroutine(StopRaining(emission, true));
            yield break;
        } else
        {
            StartCoroutine(StopRaining(emission, false));
        }
    }

    private bool retrigger = false;

    private float grassDryTime = 30;

    public IEnumerator StopRaining(ParticleSystem.EmissionModule emission, bool retrigger)
    {
        emission.enabled = false;
        isRaining = false;
        StartCoroutine(lerpTerrain.ToOasis(15f));

        if (retrigger)
        {

            StartCoroutine(ChanceOfRain(emission));
            yield break;
        } else
        {
            yield break;
        }
    }
}

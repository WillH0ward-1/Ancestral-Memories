using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainControl : MonoBehaviour
{
    [SerializeField] private ParticleSystem rainParticles;

    void Start()
    {
        ParticleSystem.EmissionModule emission = rainParticles.emission;
        emission.enabled = false;

        isRaining = false;

        StartCoroutine(ChanceOfRain(rainParticles.emission));
    }

    public bool isRaining = false;
    public bool drought = false;

    public float droughtThreshold;

    public float minRainDuration = 10f;
    public float maxRainDuration = 60f;

    public float minWaitForRain = 0f;
    public float maxWaitForRain = 300f;

    private void Awake()
    {
        droughtThreshold = maxWaitForRain;
    }

    public IEnumerator ChanceOfRain(ParticleSystem.EmissionModule emission)
    {
        if (!isRaining && !drought)
        {
            float time = 0;
            float duration = Random.Range(minWaitForRain, maxWaitForRain);

            if (duration >= droughtThreshold)
            {
                drought = true;
                StartCoroutine(Drought(emission));
                yield break;
            }

            while (time < duration)
            {
                time += Time.deltaTime;
                yield return null;
            }

            if (time >= duration)
            {
                StartCoroutine(StartRaining(emission));
            }
            
        } else
        {
            yield break;
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
        float time = 0;
        float duration = Random.Range(minDroughtDuration, maxDroughtDuration);

        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        if (time >= duration)
        {
            drought = false;
            StartCoroutine(StartRaining(emission));
        }
    }


    public IEnumerator StartRaining(ParticleSystem.EmissionModule emission)
    {
        emission.enabled = true;
        isRaining = true;

        float time = 0;

        float duration = Random.Range(minRainDuration, maxRainDuration);

        while (time < duration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        if (time >= duration)
        {
            StartCoroutine(StopRaining(emission));
            yield break;
        }
    }

    public IEnumerator StopRaining(ParticleSystem.EmissionModule emission)
    {
        if (isRaining)
        {
            emission.enabled = false;
            isRaining = false;
            StartCoroutine(ChanceOfRain(emission));

            yield break;
        }
        else
        {
            yield break;
        }
    }
}

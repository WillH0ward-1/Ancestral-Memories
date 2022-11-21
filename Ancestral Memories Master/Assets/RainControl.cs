using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainControl : MonoBehaviour
{
    [SerializeField] private ParticleSystem rainParticles;
    // Start is called before the first frame update
    void Start()
    {
        var emission = rainParticles.emission;

        emission.enabled = false;
        isRaining = false;

        StartCoroutine(ChanceOfRain(rainParticles.emission)
);
    }
    public bool isRaining;

    public IEnumerator ChanceOfRain(ParticleSystem.EmissionModule emission)
    {
        if (!isRaining)
        {
            float time = 0;
            float duration = Random.Range(60, 300);

            while (time < duration)
            {
                time += Time.deltaTime;
            }

            if (time >= duration)
            {
                StartCoroutine(StartRaining(emission));
            }

            yield break;
        }
    }

    public IEnumerator StartRaining(ParticleSystem.EmissionModule emission) {
        emission.enabled = true;
        isRaining = true;
        StartCoroutine(RainDuration(emission));
        yield break;
    }

    public IEnumerator StopRaining(ParticleSystem.EmissionModule emission)
    {
        emission.enabled = false;
        isRaining = false;
        StartCoroutine(ChanceOfRain(emission));

        yield break;
    }

    public IEnumerator RainDuration(ParticleSystem.EmissionModule emission)
    {
        if (isRaining)
        {
            float time = 0;
            float duration = Random.Range(60, 300);

            while (time < duration)
            {
                time += Time.deltaTime;
            }

            if (time >= duration)
            {
                StartCoroutine(StopRaining(emission));
            }

            yield break;
        }

        yield return null;
    }

}

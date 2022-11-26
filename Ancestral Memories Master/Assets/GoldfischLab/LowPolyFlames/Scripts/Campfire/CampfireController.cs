using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CampfireController : MonoBehaviour
{
     private float targetEmissionRate;
     private float targetLightIntensity;

     [SerializeField] private float delayStart;
    [SerializeField] private float duration;

    private Transform fireRoot;
    private ParticleSystem fireParticles;
    private Light fireLight;
    private Collider triggerCollider;

    private float fireLightIntensity;
    private float emissionRate;

    void StartFire()
    {
        StartCoroutine(FireControl(fireLightIntensity, targetLightIntensity, targetEmissionRate, duration, delayStart));
    }

    void StopFire()
    {

        targetLightIntensity = 0;
        targetEmissionRate = 0;
        StartCoroutine(FireControl(fireLightIntensity, targetLightIntensity, targetEmissionRate, duration, delayStart));
    }

    private EmissionModule emission;

    void Awake()
    {
        fireRoot = gameObject.transform;

        fireParticles = fireRoot.transform.GetComponentInChildren<ParticleSystem>();

        emission = fireParticles.emission;
        emission.rateOverTime = 0;
        triggerCollider = fireParticles.transform.GetComponent<Collider>();
        triggerCollider.enabled = false;

        fireLight = fireParticles.transform.GetComponentInChildren<Light>();
        fireLight.intensity = 0;

        StartFire();

    }

    public IEnumerator FireControl(float fireLightIntensity, float targetLightIntensity, float targetEmissionRate, float duration, float delayStart)
    {
        yield return new WaitForSeconds(delayStart);

        float time = 0;

        while (time <= duration)
        {
            time += Time.deltaTime;

            fireLight.intensity = Mathf.Lerp(fireLightIntensity, targetLightIntensity, time / duration);

            emission.rateOverTime = Mathf.Lerp(emissionRate, targetEmissionRate, time / duration);

            yield return null;
        }

        if (time >= duration)
        {
            triggerCollider.enabled = true;

            yield break;
        }

    }
}

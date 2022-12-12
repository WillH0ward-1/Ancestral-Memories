using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class FireController : MonoBehaviour
{
    [SerializeField] private float startFireLightIntensityTarget;
    [SerializeField] private float startFireEmissionRateTarget;
    [SerializeField] private float startFireDuration;
    //[SerializeField] private float startFireDelay;

    [SerializeField] private float endFireDuration;
    //[SerializeField] private float endFireDelay;

    private ParticleSystem fireParticles;
    private Light fireLight;
    private Collider triggerCollider;

    private float fireLightIntensity;
    private float emissionRate;

    [SerializeField] private GameObject fire;

    private Vector3 target;
    private Transform targetTransform;

    public void StartFire(Transform targetTransform, Vector3 target)
    {
        float startFireLightIntensityTarget = 20;
        float startFireEmissionRateTarget = 20;
        float startFireDuration = 10f;
        float startFireDelay = Random.Range(1, 2);

        StartCoroutine(FireControl(targetTransform, target, startFireLightIntensityTarget, startFireEmissionRateTarget, startFireDuration, startFireDelay));
    }

    void KillFire(GameObject newFire)
    {
        float endFireLightIntensityTarget = 0;
        float endFireEmissionRateTarget = 0;
        float endFireDuration = 5f;
        float endFireDelay = Random.Range(1, 2);

        StartCoroutine(FireControl(targetTransform, target, endFireLightIntensityTarget, endFireEmissionRateTarget, endFireDuration, endFireDelay));

        triggerCollider.enabled = false;
        Destroy(newFire);
    }

    private EmissionModule emission;

    void Awake()
    {
        /*
        GameObject fireInstance = Instantiate(fire, new Vector3(0, 1, 0), Quaternion.identity);
        fireRoot = fireInstance.transform;
        fireInstance.transform.SetParent(transform, false);
        */
    }

    float minFireDuration = 3;
    float maxFireDuration = 6;

    public IEnumerator FireControl(Transform targetTransform, Vector3 target, float targetLightIntensity, float targetEmissionRate, float duration, float delayStart)
    {
        GameObject newFire = Instantiate(fire, new Vector3(target.x, target.y, target.z), Quaternion.identity);

        if (targetTransform.CompareTag("Trees") || targetTransform.CompareTag("AppleTree"))
        {
            newFire.transform.SetParent(targetTransform, true);
        }
        //newFire.transform.SetParent(targetTransform, true);

        fireParticles = newFire.transform.GetComponentInChildren<ParticleSystem>();

        emission = fireParticles.emission;
        emission.rateOverTime = 0;
        triggerCollider = fireParticles.transform.GetComponent<Collider>();
        triggerCollider.enabled = false;
        fireLight = fireParticles.transform.GetComponentInChildren<Light>();
        fireLight.intensity = 0;
        fireLightIntensity = fireLight.intensity;

        yield return new WaitForSeconds(delayStart);

        float time = 0;

        while (time <= duration)
        {
            time += Time.deltaTime;

            fireLightIntensity = Mathf.Lerp(fireLightIntensity, targetLightIntensity, time / duration);
            fireLight.intensity = fireLightIntensity;
            emission.rateOverTime = Mathf.Lerp(emissionRate, targetEmissionRate, time / duration);

            yield return null;
        }

        if (time >= duration)
        {
            float fireLength = Random.Range(minFireDuration, maxFireDuration);
            yield return new WaitForSeconds(fireLength);

            KillFire(newFire);

            yield break;
        }
    }
}

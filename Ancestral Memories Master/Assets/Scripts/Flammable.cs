using System.Collections;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Flammable : MonoBehaviour
{
    [SerializeField] private GameObject firePrefab;
    private Mesh mesh;
    private Vector3[] vertices;
    int sampleDensity;

    private ScaleControl scaleControl;

    [SerializeField] int vertSampleFactor;

    [SerializeField] private bool invertSpreadOrigin = false;

    [SerializeField] private int minFireSpreadDelay = 0;
    [SerializeField] private int maxFireSpreadDelay = 5;

    [SerializeField] private float startFireLightIntensityTarget;
    [SerializeField] private float startFireEmissionRateTarget;
    [SerializeField] private float startFireDuration;
    //[SerializeField] private float startFireDelay;

    [SerializeField] private float endFireDuration;
    //[SerializeField] private float endFireDelay;

    private float minFireDuration = 3;
    private float maxFireDuration = 6;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            StartCoroutine(CatchFire());
            return;
        }
    }

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        scaleControl = GetComponent<ScaleControl>();
    }

    private IEnumerator CatchFire()
    {
        int sampleDensity = vertices.Length / vertSampleFactor;

        int fireSpreadDelay = Random.Range(minFireSpreadDelay, maxFireSpreadDelay);

        Debug.Log(transform.gameObject.name + " has caught fire!" + "Sample Density: " + sampleDensity);

        if (!invertSpreadOrigin)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                if (i >= vertices.Length)
                {
                    yield break;
                }

                StartFire(transform, transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]));

                i += sampleDensity;

                yield return new WaitForSeconds(fireSpreadDelay);

                yield return null;
            }

            StartCoroutine(BurnToGround());

        }
        else if (invertSpreadOrigin)
        {
            for (int i = vertices.Length; i-- > 0;)
            {
                if (i <= 0)
                {
                    yield break;
                }

                StartFire(transform, transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]));

                i -= sampleDensity;

                yield return new WaitForSeconds(fireSpreadDelay);

                yield return null;
            }

            StartCoroutine(BurnToGround());
        }

        yield return null;
    }

    [SerializeField] private float fallDuration = 2;

    private IEnumerator BurnToGround()
    {
        StartCoroutine(scaleControl.LerpScale(gameObject, transform.localScale, new Vector3(transform.localScale.x, 0, transform.localScale.y), fallDuration, 0f));

        yield return new WaitForSeconds(fallDuration);

        Destroy(gameObject);
        yield return null;

    }

    public void StartFire(Transform targetTransform, Vector3 target)
    {
        float startFireLightIntensityTarget = 20;
        float startFireEmissionRateTarget = 20;
        float startFireDuration = 10f;
        float startFireDelay = Random.Range(1, 2);

        StartCoroutine(FireControl(targetTransform, target, startFireLightIntensityTarget, startFireEmissionRateTarget, startFireDuration, startFireDelay));
    }

    public IEnumerator FireControl(Transform targetTransform, Vector3 target, float targetLightIntensity, float targetEmissionRate, float duration, float delayStart)
    {
        GameObject newFire = Instantiate(firePrefab, new Vector3(target.x, target.y, target.z), Quaternion.identity);

        if (targetTransform.CompareTag("Trees") || targetTransform.CompareTag("AppleTree"))
        {
            newFire.transform.SetParent(targetTransform, true);
        }

        ParticleSystem fireParticles = newFire.transform.GetComponentInChildren<ParticleSystem>();

        EmissionModule emission = fireParticles.emission;
        emission.rateOverTime = 0;
        Collider triggerCollider = fireParticles.transform.GetComponent<Collider>();
        triggerCollider.enabled = false;
        Light fireLight = fireParticles.transform.GetComponentInChildren<Light>();
        fireLight.intensity = 0;
        float fireLightIntensity = fireLight.intensity;

        yield return new WaitForSeconds(delayStart);

        float time = 0;

        while (time <= 1f)
        {
            time += Time.deltaTime / duration;

            fireLightIntensity = Mathf.Lerp(fireLightIntensity, targetLightIntensity, time);
            fireLight.intensity = fireLightIntensity;
            emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, targetEmissionRate, time);

            yield return null;
        }

        if (time >= 1f)
        {
            float fireLength = Random.Range(minFireDuration, maxFireDuration);
            yield return new WaitForSeconds(fireLength);

            KillFire(newFire, targetTransform, target, 0, 0, endFireDuration, Random.Range(1, 2));

            yield break;
        }
    }

    void KillFire(GameObject newFire, Transform targetTransform, Vector3 target, float targetLightIntensity, float targetEmissionRate, float duration, float delayStart)
    {
        StartCoroutine(FireControl(targetTransform, target, targetLightIntensity, targetEmissionRate, duration, delayStart));

        Destroy(newFire);
    }
}

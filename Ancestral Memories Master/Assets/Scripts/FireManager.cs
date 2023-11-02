using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance;

    [Header("Fire Prefab")]
    [SerializeField] private GameObject firePrefab;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 20;

    [Header("Fire Settings")]
    [SerializeField] private float startFireLightIntensityTarget = 20;
    [SerializeField] private float startFireEmissionRateTarget = 20;
    [SerializeField] private float startFireDuration = 10f;
    [SerializeField] private float minFireDuration = 8f;
    [SerializeField] private float maxFireDuration = 16f;
    [SerializeField] private float endFireDuration = 5f;
    private int vertSampleFactor;
    [SerializeField] private float minFireSpreadDelay = 0.5f;
    [SerializeField] private float maxFireSpreadDelay = 1f;

    [SerializeField] private bool invertSpreadOrigin = false;

    [SerializeField] private const int MaxVertices = 500; // Maximum number of vertices to use

    [SerializeField] private float vertexDensityDivider = 10f; // Adjust this value in the Inspector

    private Queue<GameObject> firePool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple instances of FireManager detected!");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(firePrefab);
            obj.SetActive(false);
            firePool.Enqueue(obj);
        }
    }

    private GameObject GetFireFromPool()
    {
        if (firePool.Count > 0)
        {
            GameObject obj = firePool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        // Optionally create a new object if the pool is empty
        GameObject newObj = Instantiate(firePrefab);
        return newObj;
    }

    private void ReturnFireToPool(GameObject fire)
    {
        fire.SetActive(false);
        firePool.Enqueue(fire);
    }

    public void StartFireOnObject(GameObject targetObject)
    {
        List<Vector3> allVertices = new List<Vector3>();

        // Add vertices from MeshFilter
        MeshFilter[] meshFilters = targetObject.GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            if (meshFilter.sharedMesh != null)
            {
                AddMeshVertices(meshFilter.sharedMesh, meshFilter.gameObject, ref allVertices);
            }
        }

        // Add vertices from SkinnedMeshRenderer
        SkinnedMeshRenderer[] skinnedMeshRenderers = targetObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            if (skinnedMeshRenderer.sharedMesh != null)
            {
                Mesh skinnedMesh = new Mesh();
                skinnedMeshRenderer.BakeMesh(skinnedMesh);
                AddMeshVertices(skinnedMesh, skinnedMeshRenderer.gameObject, ref allVertices);
            }
        }

        if (allVertices.Count > 0)
        {
            StartCoroutine(SpreadFire(targetObject, allVertices.ToArray(), invertSpreadOrigin, minFireSpreadDelay, maxFireSpreadDelay));
        }
        else
        {
            Debug.LogWarning("No vertices found for starting fire on object: " + targetObject.name);
        }
    }

    // ... [Previous code in FireManager] ...

    public void StartFireOnSegments(List<GameObject> segmentObjects)
    {
        if (segmentObjects.Count == 0)
        {
            Debug.LogWarning("No segments found for starting fire.");
            return;
        }

        StartCoroutine(SpreadFireAcrossSegments(segmentObjects));
    }

    private IEnumerator SpreadFireAcrossSegments(List<GameObject> segmentObjects)
    {
        foreach (GameObject segment in segmentObjects)
        {
            float delay = Random.Range(minFireSpreadDelay, maxFireSpreadDelay);
            yield return new WaitForSeconds(delay);

            StartCoroutine(ControlFire(segment, segment.transform.position));
        }
    }

    // ... [Rest of the FireManager code] ...


    private void AddMeshVertices(Mesh mesh, GameObject gameObject, ref List<Vector3> allVertices)
    {
        Vector3[] vertices = mesh.vertices;

        vertSampleFactor = Mathf.Max(1, (int)(vertices.Length / vertexDensityDivider));

        for (int i = 0; i < vertices.Length; i += vertSampleFactor)
        {
            allVertices.Add(gameObject.transform.TransformPoint(vertices[i]));
        }
    }



    private IEnumerator SpreadFire(GameObject targetObject, Vector3[] vertices, bool invertSpreadOrigin, float minFireSpreadDelay, float maxFireSpreadDelay)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            float delay = Random.Range(minFireSpreadDelay, maxFireSpreadDelay);
            yield return new WaitForSeconds(delay);

            Vector3 spreadPoint = vertices[invertSpreadOrigin ? vertices.Length - 1 - i : i];
            StartCoroutine(ControlFire(targetObject, spreadPoint));
        }
    }

    public void StartFireAtPosition(Vector3 position)
    {
        StartCoroutine(ControlFire(null, position));
    }


    private IEnumerator ControlFire(GameObject targetObject, Vector3 target)
    {
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.position = target;

        // If targetObject is not null, set firePoint as its child
        if (targetObject != null)
        {
            firePoint.transform.SetParent(targetObject.transform);
            firePoint.transform.localPosition = Vector3.one;
        } 

        GameObject newFire = GetFireFromPool();
        newFire.transform.position = firePoint.transform.position;

        // If targetObject is null, don't set the newFire as a child of firePoint
        // This is to allow fires to be created independently of a target object
        newFire.transform.SetParent(targetObject != null ? firePoint.transform : null);

        // Setup fire effect, light, and particle system
        ParticleSystem fireParticles = newFire.GetComponent<ParticleSystem>();
        Light fireLight = newFire.GetComponent<Light>();

        float time = 0;
        float fireLightIntensity = 0;

        // Start increasing the intensity of fire and light
        while (time < startFireDuration)
        {
            time += Time.deltaTime;
            fireLightIntensity = Mathf.Lerp(0, startFireLightIntensityTarget, time / startFireDuration);

            // Update light intensity and particle emission rate here
            if (fireParticles)
            {
                ParticleSystem.EmissionModule emissionModule = fireParticles.emission;
                emissionModule.rateOverTime = Mathf.Lerp(0, startFireEmissionRateTarget, time / startFireDuration);
            }
            if (fireLight)
                fireLight.intensity = fireLightIntensity;

            yield return null;
        }

        // Fire at full intensity
        yield return new WaitForSeconds(Random.Range(minFireDuration, maxFireDuration));

        // Begin to reduce the intensity of fire and light
        time = 0;
        while (time < endFireDuration)
        {
            time += Time.deltaTime;
            fireLightIntensity = Mathf.Lerp(startFireLightIntensityTarget, 0, time / endFireDuration);

            // Gradually decrease light intensity and particle emission rate
            if (fireParticles)
            {
                ParticleSystem.EmissionModule emissionModule = fireParticles.emission;
                emissionModule.rateOverTime = Mathf.Lerp(startFireEmissionRateTarget, 0, time / endFireDuration);
            }
            if (fireLight)
                fireLight.intensity = fireLightIntensity;

            yield return null;
        }

        // Once the fire is out, return the fire object to the pool and remove the firePoint
        ReturnFireToPool(newFire);
        Destroy(firePoint);
    }

}

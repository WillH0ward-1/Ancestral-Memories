using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ProceduralModeling;
using UnityEngine;


public class FireManager : MonoBehaviour
{
    public static FireManager Instance;

    private LayerMask flammableLayerMask;

    [Header("Fire Prefab")]
    [SerializeField] private GameObject firePrefab;

    [Header("Pool Settings")]
    [SerializeField] private int poolSize = 20;

    [Header("Fire Settings")]
    [SerializeField] private float startFireEmissionRateTarget = 20;
    [SerializeField] private float startFireDuration = 10f;
    [SerializeField] private float minFireDuration = 8f;
    [SerializeField] private float maxNpcBurnTime = 20f;
    [SerializeField] private float maxTreeBurnTime = 30f;
    [SerializeField] private float endFireDuration = 5f;
    [SerializeField] private float minFireSpreadDelay = 2;
    [SerializeField] private float maxFireSpreadDelay = 3f;

    [Header("Fire Spread Settings")]
    [SerializeField] private float checkInterval = 0.5f;  // How often to check for flammable objects
    [SerializeField] private float checkRadius = 10f;     // The radius for checking for flammable objects

    private Queue<GameObject> firePool = new Queue<GameObject>();
    public MapObjGen mapObjGen;
    private RainControl rainControl;

    [Header("Objects On Fire")]
    [SerializeField] private HashSet<GameObject> objectsOnFire = new HashSet<GameObject>();


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

        flammableLayerMask = LayerMask.GetMask("Trees", "Human", "Animals");
        InitializePool();
    }

    private Dictionary<GameObject, List<GameObject>> flammableObjectsToFirePoints = new Dictionary<GameObject, List<GameObject>>();

    private int firePointIndex;

    public void GenerateFirePoints()
    {
        firePointIndex = 0; // Reset index at the start of generating fire points

        foreach (GameObject flammableObject in mapObjGen.flammableObjectList)
        {
            if (flammableObject != null)
            {
                // Check if the GameObject is on one of the flammable layers
                if (((1 << flammableObject.layer) & flammableLayerMask) != 0)
                {
                    if (flammableObject.layer == LayerMask.NameToLayer("Human") || flammableObject.layer == LayerMask.NameToLayer("Animals"))
                    {
                        Transform rigTransform = FindRigInChildren(flammableObject.transform);

                        if (rigTransform != null)
                        {
                            List<GameObject> firePoints = new List<GameObject>();
                            CreateFirePoints(rigTransform.gameObject, firePoints); // Call the function directly
                            flammableObjectsToFirePoints[flammableObject] = firePoints;
                        }
                        else
                        {
                            //   Debug.LogWarning($"'Rig' tagged object not found in children of: {flammableObject.name}");
                        }
                    }
                    else
                    {
                        List<GameObject> firePoints = new List<GameObject>();
                        CreateFirePoints(flammableObject, firePoints); // Call the function directly
                        flammableObjectsToFirePoints[flammableObject] = firePoints;
                    }
                }
                else
                {
                    //  Debug.LogWarning($"Object layer is not flammable: {flammableObject.name}");
                }
            }
        }
    }

    [SerializeField] private int desiredNumberOfFirePoints = 10; // The target number of fire points you want

    public Vector3 defaultMaxScale = new Vector3(35,35,35);
    public Vector3 treeMaxScale = new Vector3(55, 55, 55);
   
    private void CreateFirePoints(GameObject rigObject, List<GameObject> firePoints)
    {
        Transform[] allChildren = rigObject.GetComponentsInChildren<Transform>();

        int dynamicResolution = Mathf.Max(1, allChildren.Length / desiredNumberOfFirePoints);

        for (int i = 1; i < allChildren.Length; i += dynamicResolution)
        {
            if (allChildren[i] == rigObject.transform) continue;

            if (allChildren[i].GetComponent<NonFlammable>() != null) continue;

            GameObject firePoint = new GameObject($"FirePoint_{firePointIndex++}");
            firePoint.transform.SetParent(allChildren[i], false);
            firePoint.transform.localPosition = Vector3.zero;
            firePoints.Add(firePoint);
        }
    }

    public IEnumerator StartFireOnObject(GameObject target)
    {
        if (coolDownDictionary.TryGetValue(target, out bool isCoolingDown) && isCoolingDown)
        {
            yield break;
        }

        ObjectOnFire(target, true);

        if (!fireCoroutines.TryGetValue(target, out List<Coroutine> coroutinesList))
        {
            coroutinesList = new List<Coroutine>();
            fireCoroutines[target] = coroutinesList;
        }

        ParticleSystem fireParticles = null;

        HumanAI humanAI = null;

        if (target.CompareTag("Human"))
        {
            humanAI = target.GetComponentInChildren<HumanAI>();
            if (humanAI != null)
            {
                if (!humanAI.isElectrocuted || !humanAI.isGettingUp){
                        humanAI.ChangeState(HumanAI.AIState.RunningPanic);
                    }
            }
        }

        StartCoroutine(StartFireStopCountDown(target, humanAI, coroutinesList, maxNpcBurnTime, null));

        if (flammableObjectsToFirePoints.TryGetValue(target, out List<GameObject> firePoints))
        {
            foreach (GameObject firePoint in firePoints)
            {
                GameObject fireInstance = GetFireFromPool();
                if (fireInstance == null)
                {
                    continue;
                }

                fireInstance.transform.position = firePoint.transform.position;
                fireInstance.transform.SetParent(firePoint.transform);
                fireParticles = fireInstance.GetComponent<ParticleSystem>();

                Coroutine controlCoroutine = StartCoroutine(ControlFire(fireInstance, target, humanAI, fireParticles, defaultMaxScale));
                coroutinesList.Add(controlCoroutine);

                yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));
            }
        }

        yield break;
    }

    public IEnumerator StartFireOnSegments(GameObject target, SaturationControl saturation)
    {
        if (coolDownDictionary.TryGetValue(target, out bool isCoolingDown) && isCoolingDown)
        {
            yield break;
        }

        ObjectOnFire(target, true);

        List<GameObject> segmentObjects;
        ProceduralTree proceduralTree = target.GetComponentInChildren<ProceduralTree>();
        PTGrowing ptGrowing = target.GetComponentInChildren<PTGrowing>();

        segmentObjects = proceduralTree.segmentObjects;

        if (segmentObjects.Count == 0)
        {
            Debug.LogWarning("No segments found for starting fire.");
            yield break;
        }

        saturation.LerpSaturationToMin();

        if (proceduralTree.showingSegments)
        {
            proceduralTree.ShowSegments();
        }

        if (!fireCoroutines.TryGetValue(target, out List<Coroutine> coroutinesList))
        {
            coroutinesList = new List<Coroutine>();
            fireCoroutines[target] = coroutinesList;
        }

        // Start both coroutines simultaneously
        Coroutine branchFallCoroutine = StartCoroutine(ApplySmallBursts(segmentObjects, proceduralTree));
        Coroutine fireSetupCoroutine = StartCoroutine(SetUpFires(segmentObjects, coroutinesList, target));

        coroutinesList.Add(branchFallCoroutine);
        coroutinesList.Add(fireSetupCoroutine);

        StartCoroutine(StartFireStopCountDown(target, null, coroutinesList, maxTreeBurnTime, segmentObjects));

        yield break;
    }

    private IEnumerator SetUpFires(List<GameObject> segmentObjects, List<Coroutine> coroutinesList, GameObject target)
    {
        for (int i = segmentObjects.Count - 1; i >= 0; i--)
        {
            GameObject segment = segmentObjects[i];

            if (segment.name == "TreeRoot")
            {
                continue;
            }

            GameObject fireInstance = GetFireFromPool();
            if (fireInstance == null)
            {
                continue;
            }

            MeshRenderer meshRenderer = segment.GetComponent<MeshRenderer>();
            Vector3 centerPosition = meshRenderer != null ? meshRenderer.bounds.center : Vector3.zero;
            fireInstance.transform.position = centerPosition;
            fireInstance.transform.rotation = Quaternion.identity;
            fireInstance.transform.SetParent(segment.transform);
            ParticleSystem fireParticles = fireInstance.GetComponent<ParticleSystem>();

            Vector3 maxFireScale;
            if (meshRenderer != null)
            {
                float largestDimension = Mathf.Max(meshRenderer.bounds.size.x, meshRenderer.bounds.size.y, meshRenderer.bounds.size.z);
                maxFireScale = new Vector3(largestDimension, largestDimension, largestDimension) * 10f;
            }
            else
            {
                maxFireScale = treeMaxScale;
            }

            Coroutine controlCoroutine = StartCoroutine(ControlFire(fireInstance, target, null, fireParticles, maxFireScale));
            coroutinesList.Add(controlCoroutine);

            yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 5f));
        }

 
        yield break;
    }


    private IEnumerator ApplySmallBursts(List<GameObject> segments, ProceduralTree proceduralTree)
    {
        foreach (GameObject segment in segments)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(minFireSpreadDelay, maxFireSpreadDelay));
            proceduralTree.SmallBurst(segment);
        }
    }



    private void CheckForFlammableObjects(GameObject fireInstance, HumanAI humanAI)
    {
        Collider[] hitColliders = Physics.OverlapSphere(fireInstance.transform.position, checkRadius, flammableLayerMask);

        foreach (var hitCollider in hitColliders)
        {
            GameObject hitObj = hitCollider.gameObject;
            Coroutine fireCoroutine = null; // Initialize to null

            if (hitObj != null && IsFlammable(hitObj) && !IsOnFire(hitObj))
            {
                if (hitObj.CompareTag("Trees"))
                {
                    if (!hitObj.GetComponentInChildren<PTGrowing>().ValidateTree()) continue;
                    SaturationControl saturationControl = hitObj.GetComponentInChildren<SaturationControl>();
                    fireCoroutine = StartCoroutine(StartFireOnSegments(hitObj, saturationControl));
                }
                else if (hitObj.CompareTag("Human") && humanAI != null)
                {
                    if (humanAI.isElectrocuted || humanAI.isGettingUp)
                    {
                        return;
                    }
                }

                if (fireCoroutine == null) // Only assign if it wasn't assigned earlier
                {
                    fireCoroutine = StartCoroutine(StartFireOnObject(hitObj));
                }

                objectsOnFire.Add(hitObj);

                Debug.Log(hitObj + " is on fire!");

                if (!fireCoroutines.TryGetValue(hitObj, out List<Coroutine> coroutinesList))
                {
                    coroutinesList = new List<Coroutine>();
                    fireCoroutines[hitObj] = coroutinesList;
                }
                coroutinesList.Add(fireCoroutine);
            }
        }
    }



    private Dictionary<GameObject, List<Coroutine>> fireCoroutines = new Dictionary<GameObject, List<Coroutine>>();

    public LayerMask waterDetectLayer;

    private IEnumerator ControlFire(GameObject fireInstance, GameObject target, HumanAI humanAI, ParticleSystem fireParticles, Vector3 maxScale)
    {
        // Initial scale should be zero
        fireInstance.transform.localScale = Vector3.zero;

        // Start increasing the scale and emission rate of the fire GameObject
        float time = 0;
        while (time < startFireDuration)
        {
            time += Time.deltaTime;
            float lerpFactor = time / startFireDuration;
            Vector3 scale = Vector3.Lerp(Vector3.zero, maxScale, lerpFactor);

            if (fireParticles)
            {
                ParticleSystem.EmissionModule emissionModule = fireParticles.emission;
                emissionModule.rateOverTime = Mathf.Lerp(0, startFireEmissionRateTarget, lerpFactor);
            }

            fireInstance.transform.localScale = scale;
            fireInstance.transform.rotation = Quaternion.identity; // Keep the fire upright

            yield return null;
        }

        // Fire at full intensity for a random duration between min and max fire duration
        float randomFireDuration = UnityEngine.Random.Range(minFireDuration, maxNpcBurnTime);
        float elapsedTime = 0;
        while (elapsedTime < randomFireDuration)
        {
            elapsedTime += checkInterval;

            CheckForFlammableObjects(fireInstance, humanAI);

            fireInstance.transform.rotation = Quaternion.identity; // Keep the fire upright
            yield return new WaitForSeconds(checkInterval);
        }

        StartCoroutine(ShrinkFire(fireInstance));
    }

    private IEnumerator ShrinkFire(GameObject fireInstance)
    {
        float time = 0;
        Vector3 currentScale = fireInstance.transform.localScale;
        ParticleSystem fireParticles = fireInstance.GetComponent<ParticleSystem>();
        while (time < endFireDuration)
        {
            time += Time.deltaTime;
            float lerpFactor = time / endFireDuration;
            Vector3 scale = Vector3.Lerp(currentScale, Vector3.zero, lerpFactor);

            if (fireParticles)
            {
                ParticleSystem.EmissionModule emissionModule = fireParticles.emission;
                emissionModule.rateOverTime = Mathf.Lerp(startFireEmissionRateTarget, 0, lerpFactor);
            }

            fireInstance.transform.localScale = scale;

            yield return null;
        }

        ReturnFireToPool(fireInstance);
    }


    public void StopFireOnObject(GameObject target, HumanAI humanAI, List<GameObject> segments)
    {
        if (target == null)
        {
            Debug.LogError("StopFireOnObject called with a null target.");
            return;
        }

        if (target.CompareTag("Human") && humanAI != null)
        {
            humanAI.stats.isTerrified = false;
        }

        if (fireCoroutines.TryGetValue(target, out List<Coroutine> coroutinesList))
        {
            foreach (Coroutine coroutine in coroutinesList)
            {
                if (coroutine != null) StopCoroutine(coroutine);
            }
            fireCoroutines.Remove(target);
        }

        if (segments != null && target.CompareTag("Trees"))
        {
            foreach (GameObject segment in segments)
            {
                HandleFirePoints(segment);
            }
        }
        else if (flammableObjectsToFirePoints.TryGetValue(target, out List<GameObject> firePoints))
        {
            if (firePoints.Count == 0)
            {
                Debug.LogWarning($"No fire points found for target: {target.name}");
            }

            foreach (GameObject firePoint in firePoints)
            {
                HandleFirePoints(firePoint);
            }
        }

        ObjectOnFire(target, false);
        Debug.Log($"Fire stopped on {target.name}");
    }

    private void HandleFirePoints(GameObject firePoint)
    {
        ParticleSystem[] particleSystems = firePoint.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems)
        {
            GameObject fireInstance = ps.gameObject;
            StartCoroutine(ShrinkFire(fireInstance));
        }
    }



    [SerializeField] private float coolDownTime = 15f;
    private Dictionary<GameObject, bool> coolDownDictionary = new Dictionary<GameObject, bool>();

    public IEnumerator CoolDown(GameObject target)
    {
        // Add target to cooldown dictionary
        coolDownDictionary[target] = true;

        // Wait for the cooldown time
        yield return new WaitForSeconds(coolDownTime);

        // After cooldown, allow target to catch fire again
        coolDownDictionary[target] = false;
    }

    public IEnumerator StartFireStopCountDown(GameObject target, HumanAI humanAI, List<Coroutine> coroutinesList, float maxBurnTime, List<GameObject> segments)
    {
        float startTime = Time.time;

        // Check for human targets and perform continuous checking for revival
        if (target != null && target.CompareTag("Human") && humanAI != null)
        {
            while (Time.time - startTime < maxBurnTime)
            {
                if (humanAI.isReviving)
                {
                    StopFireOnObject(target, humanAI, null);
                    Coroutine coolDownCoroutine = StartCoroutine(CoolDown(target));
                    coroutinesList.Add(coolDownCoroutine);
                    yield break;
                }
                yield return null;
            }
        }
        else
        {
            while (Time.time - startTime < maxBurnTime)
            {
                yield return null;
            }

            StopFireOnObject(target, null, segments);
            Coroutine finalCoolDownCoroutine = StartCoroutine(CoolDown(target));
            coroutinesList.Add(finalCoolDownCoroutine);
            yield break;

        }
    }

    private void StopAndReturnFire(GameObject fireInstance)
    {
        // If the fire instance has a particle system, stop it
        ParticleSystem fireParticles = fireInstance.GetComponent<ParticleSystem>();
        if (fireParticles)
        {
            fireParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Return the fire instance to the pool
        ReturnFireToPool(fireInstance);
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

    [SerializeField] private int maxFireInstances = 100; // Maximum number of fire instances allowed
    private int currentFireCount = 0; // Tracks the current number of active fires

    private List<GameObject> activeFires = new List<GameObject>();

    private GameObject GetFireFromPool()
    {
        GameObject obj = null;

        // Try to get an inactive fire instance from the pool
        while (firePool.Count > 0)
        {
            obj = firePool.Dequeue();
            if (obj != null && !obj.activeInHierarchy)
            {
                break; // Found a valid, inactive object
            }
            // If obj is null or active, continue to the next one in the pool
        }

        // If the pool is empty, try to recycle the oldest active fire instance
        if (obj == null && activeFires.Count > 0)
        {
            Debug.LogWarning("Fire pool is empty. Recycling the oldest active fire instance.");
            obj = activeFires[0];
            activeFires.RemoveAt(0); // Remove it from the active fires list

            if (obj != null)
            {
                // Reset the fire as needed before reusing
                ResetFire(obj);
            }
            else
            {
                Debug.LogError("Encountered a null object in active fires list.");
                return null; // Exit as no valid fire instance is found
            }
        }

        // If no valid fire instance is found
        if (obj == null)
        {
            Debug.LogWarning("No available fires to recycle. All fires are active.");
            return null;
        }

        // Activate the fire instance and add it to the active fires list
        obj.SetActive(true);
        activeFires.Add(obj);
        return obj;
    }


    private void ResetFire(GameObject fire)
    {
        var fireParticles = fire.GetComponent<ParticleSystem>();
        if (fireParticles)
        {
            fireParticles.Clear();
            fireParticles.Play();
        }
    }

    private void ReturnFireToPool(GameObject fire)
    {
        fire.SetActive(false);
        fire.transform.SetParent(null);
        activeFires.Remove(fire); // Remove the instance from the active fires list
        firePool.Enqueue(fire);
        currentFireCount--; // Decrease count of active fires
    }

    public void StartFireAtPosition(Vector3 position)
    {
        GameObject fireInstance = GetFireFromPool();
        ParticleSystem fireParticles = fireInstance.GetComponentInChildren<ParticleSystem>();
        // If the fire instance is null (meaning the pool was empty), exit the method.
        if (fireInstance == null)
        {
            Debug.LogWarning("No fire instances available to start fire at position.");
            return; // Exit the method.
        }

        if (fireInstance.transform.parent != null)
        {
            fireInstance.transform.SetParent(null);
        }

        fireInstance.transform.position = position;

        StartCoroutine(ControlFire(fireInstance, null, null, fireParticles, defaultMaxScale));
    }

    private Transform FindRigInChildren(Transform parent)
    {
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.CompareTag("Rig"))
            {
                return child;
            }
        }
        return null;
    }

    private bool IsFlammable(GameObject obj)
    {
        // Check if the object's layer is in the flammableLayerMask
        return ((flammableLayerMask.value & (1 << obj.layer)) > 0);
    }

    public bool IsOnFire(GameObject obj)
    {
        // Check if the object is already in the set of objects on fire
        return objectsOnFire.Contains(obj);
    }

    public void ObjectOnFire(GameObject obj, bool isOnFire)
    {
        if (isOnFire)
        {
            objectsOnFire.Add(obj);
        } else
        {
            objectsOnFire.Remove(obj);
        }
    }

}

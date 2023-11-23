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
    [SerializeField] private float maxFireDuration = 16f;
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

    [SerializeField] private int desiredNumberOfFirePoints = 10; // The target number of fire points you want

    private void CreateFirePoints(GameObject rigObject, List<GameObject> firePoints)
    {
        Transform[] allChildren = rigObject.GetComponentsInChildren<Transform>();

        // Calculate dynamic resolution based on the desired number of fire points
        int dynamicResolution = Mathf.Max(1, allChildren.Length / desiredNumberOfFirePoints);

        // Starting at 1 to skip the root object itself.
        for (int i = 1; i < allChildren.Length; i += dynamicResolution)
        {
            // Skip the root 'rigObject' transform if you only want fire points on the children.
            if (allChildren[i] == rigObject.transform) continue;

            // Check if the current child has a NonFlammable component.
            if (allChildren[i].GetComponent<NonFlammable>() != null) continue;

            // We create a fire point for the current node.
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
            //Debug.Log($"Object {target.name} is cooling down and cannot catch fire.");
            yield break;
        }

        ObjectOnFire(target, true);

        if (flammableObjectsToFirePoints.TryGetValue(target, out List<GameObject> firePoints))
        {
            foreach (GameObject firePoint in firePoints)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(minFireSpreadDelay, maxFireSpreadDelay));

                GameObject fireInstance = GetFireFromPool();

                if (fireInstance == null)
                {
                    // Debug.LogWarning("Fire instance could not be created due to empty pool.");
                    continue; // Skip this iteration and move on to the next one.
                }

                fireInstance.transform.position = firePoint.transform.position;
                fireInstance.transform.SetParent(firePoint.transform);

                StartCoroutine(ControlFire(fireInstance, target));
            }

            StartCoroutine(CoolDown(target));
        }
        else
        {
            // Debug.LogWarning($"No fire points found for object: {target.name}");
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

    public void StopFireOnObject(GameObject target)
    {

        if (flammableObjectsToFirePoints.TryGetValue(target, out List<GameObject> firePoints))
        {
            foreach (GameObject firePoint in firePoints)
            {
                // Check if the firePoint has an active fire instance as a child
                if (firePoint.transform.childCount > 0)
                {
                    GameObject fireInstance = firePoint.transform.GetChild(0).gameObject;
                    StopAndReturnFire(fireInstance);
                }
            }
        }
        else
        {
            Debug.LogWarning($"No fire points found for object: {target.name}");
        }

        // Mark the object as not on fire
        ObjectOnFire(target, false);
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
        GameObject obj;

        if (firePool.Count > 0)
        {
            obj = firePool.Dequeue();
        }
        else if (activeFires.Count > 0)
        {
            Debug.LogWarning("Fire pool is empty. Recycling the oldest active fire instance.");
            // Take the oldest active fire instance
            obj = activeFires[0];
            activeFires.RemoveAt(0); // Remove it from the active fires list

            // Reset the fire as needed before reusing
            ResetFire(obj);
        }
        else
        {
            Debug.LogWarning("No available fires to recycle. All fires are active.");
            return null;
        }

        obj.SetActive(true);
        activeFires.Add(obj); // Add the instance to the active fires list
        return obj;
    }

    private void ResetFire(GameObject fire)
    {
        // If the fire has any specific state or effects that need to be reset, do it here
        // For example, resetting particle systems, timers, etc.
        // Example for a particle system reset:
        var fireParticles = fire.GetComponent<ParticleSystem>();
        if (fireParticles)
        {
            fireParticles.Clear();
            fireParticles.Play();
        }

        // Any other reset logic would go here
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

        StartCoroutine(ControlFire(fireInstance, null));
    }

    private void FireCooldown()
    {

    }

    private void CheckForFlammableObjects(GameObject fireInstance)
    {
        Collider[] hitColliders = Physics.OverlapSphere(fireInstance.transform.position, checkRadius, flammableLayerMask);

        foreach (var hitCollider in hitColliders)
        {
            GameObject hitObj = hitCollider.gameObject;

            if (IsFlammable(hitObj))
            {

                if (hitObj.CompareTag("Trees"))
                {
                    if (!hitObj.GetComponentInChildren<PTGrowing>().ValidateTree()) continue;
                }

                if (!IsAlreadyOnFire(hitObj))
                {
                    StartCoroutine(Instance.StartFireOnObject(hitObj));
                    objectsOnFire.Add(hitObj);
                }
            }
        }
    }

    private bool IsFlammable(GameObject obj)
    {
        // Check if the object's layer is in the flammableLayerMask
        return ((flammableLayerMask.value & (1 << obj.layer)) > 0);
    }

    private IEnumerator ControlFire(GameObject fireInstance, GameObject target)
    {
        ParticleSystem fireParticles = fireInstance.GetComponent<ParticleSystem>();
        Vector3 maxScale = new Vector3(35, 35, 35); // Adjust max scale if needed

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

            yield return null;
        }

        // Fire at full intensity for a random duration between min and max fire duration
        float randomFireDuration = UnityEngine.Random.Range(minFireDuration, maxFireDuration);
        float elapsedTime = 0;
        while (elapsedTime < randomFireDuration)
        {
            elapsedTime += checkInterval;
            CheckForFlammableObjects(fireInstance);
            yield return new WaitForSeconds(checkInterval);
        }

        // Begin to reduce the emission rate and scale down the GameObject
        time = 0;
        while (time < endFireDuration)
        {
            time += Time.deltaTime;
            float lerpFactor = time / endFireDuration;
            Vector3 scale = Vector3.Lerp(maxScale, Vector3.zero, lerpFactor);

            if (fireParticles)
            {
                ParticleSystem.EmissionModule emissionModule = fireParticles.emission;
                emissionModule.rateOverTime = Mathf.Lerp(startFireEmissionRateTarget, 0, lerpFactor);
            }

            fireInstance.transform.localScale = scale;

            yield return null;
        }

        ReturnFireToPool(fireInstance);
        ObjectOnFire(target, false);
    }

    private bool IsAlreadyOnFire(GameObject obj)
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

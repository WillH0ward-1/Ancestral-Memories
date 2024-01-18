using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class TempleGenerator : MonoBehaviour
{
    public GameObject monolithPrefab;
    public GameObject centerPlatformPrefab;
    public GameObject seatPlatformPrefab;

    public Vector3 monolithSize = Vector3.one;
    public Vector3 centerPlatformSize = Vector3.one;
    public Vector3 seatPlatformSize = Vector3.one;


    public Vector3 defaultMonolithSize = Vector3.one;
    public Vector3 defaultCenterPlatformSize = Vector3.one;
    public Vector3 defaultSeatPlatformSize = Vector3.one;

    public Material monolithMaterial;
    public Material centerPlatformMaterial;
    public Material seatPlatformMaterial;

    public float seatPlatformRadius = 10f;
    public float monolithSpacing = 5f;

    private List<GameObject> monoliths;
    private List<GameObject> seatPlatforms;
    private GameObject centerPlatform;

    [SerializeField] private Dictionary<GameObject, bool> seatAvailability;
    [SerializeField] private bool isCenterPlatformAvailable;

    [SerializeField] private bool isGenerated = false;

    [SerializeField] private float sizeMultiplier = 1f;
    private float raycastOffset = 10f;
    [SerializeField] private LayerMask groundAndWaterLayerMask;
    [SerializeField] private LayerMask groundLayer;
    private string waterTag = "Water";
    private GameObject templeColliderObject;

    [SerializeField] private float distanceInFrontOfSeat = 0.5f; // This can now be adjusted in the editor

    public Player player;
    public AICharacterStats playerStats;

    [SerializeField] private float faithFactor = 0f;

    [SerializeField] private int maxMonoliths = 12;
    [SerializeField] private int minMonoliths = 3;

    private Coroutine faithCheckCoroutine;

    public void GenerateTemple()
    {
        Debug.Log("GenerateTemple called: " + Environment.StackTrace);


        if (!isGenerated)
        {
            monoliths = new List<GameObject>();
            seatPlatforms = new List<GameObject>();
            seatAvailability = new Dictionary<GameObject, bool>();

            // Instantiate and set up the center platform first
            if (centerPlatformPrefab != null)
            {
                centerPlatform = Instantiate(centerPlatformPrefab, transform.position, Quaternion.identity, transform);
                centerPlatform.transform.localScale = centerPlatformSize * sizeMultiplier;

                // Apply material to the centerPlatform
                Renderer centerPlatformRenderer = centerPlatform.GetComponentInChildren<Renderer>();
                if (centerPlatformRenderer != null && centerPlatformMaterial != null)
                {
                    centerPlatformRenderer.material = centerPlatformMaterial;
                }
            }


            for (int i = 0; i < maxMonoliths; i++)
            {
                GameObject monolith = CreateMonolithOrSeat(monolithPrefab, monolithSize, monolithMaterial, i, true);
                GameObject seat = CreateMonolithOrSeat(seatPlatformPrefab, seatPlatformSize, seatPlatformMaterial, i, false);

                if (monolith == null || seat == null)
                {
                    Debug.LogError("Monolith or Seat is null in GenerateTemple");
                    continue; // Skip this iteration as we can't proceed with null objects
                }

                // Ensure the center platform is created before calling this
                CreateNPCApproachPosition(seat);

                monoliths.Add(monolith);
                seatPlatforms.Add(seat);
                seatAvailability[seat] = false;

                if (i >= GetCurrentMonolithCount())
                {
                    monolith.SetActive(false);
                    seat.SetActive(false);
                }
            }

            isGenerated = true;

            if (faithCheckCoroutine != null)
            {
                StopCoroutine(faithCheckCoroutine);
            }

            InitializeTempleState();
            faithCheckCoroutine = StartCoroutine(CheckFaithChanges());
        }
        else
        {
            UpdateMonolithsAndSeats();
        }
    }

    private void InitializeTempleState()
    {
        int currentCount = GetCurrentMonolithCount();

        for (int i = 0; i < monoliths.Count; i++)
        {
            if (i < currentCount)
            {
                if (monoliths[i] != null)
                {
                    monoliths[i].SetActive(true);
                }

                if (seatPlatforms[i] != null)
                {
                    seatPlatforms[i].SetActive(true);
                }
            }
            else
            {
                if (monoliths[i] != null)
                {
                    monoliths[i].SetActive(false);
                }

                if (seatPlatforms[i] != null)
                {
                    seatPlatforms[i].SetActive(false);
                }
            }
        }

        // Immediately reposition the monoliths and seats without animation
        for (int i = 0; i < monoliths.Count; i++)
        {
            RepositionMonolithAndSeatInstant(i);
        }
    }

    private void RepositionMonolithAndSeatInstant(int index)
    {
        float angle = index * Mathf.PI * 2 / GetCurrentMonolithCount();
        float monolithRadius = seatPlatformRadius * sizeMultiplier + monolithSpacing * sizeMultiplier;
        float seatRadius = seatPlatformRadius * sizeMultiplier;

        Vector3 monolithPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * monolithRadius;
        Vector3 seatPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * seatRadius;

        Vector3 worldMonolithPosition = transform.position + monolithPosition;
        Vector3 worldSeatPosition = transform.position + seatPosition;

        if (index < monoliths.Count && monoliths[index] != null)
        {
            monoliths[index].transform.position = worldMonolithPosition;
        }

        if (index < seatPlatforms.Count && seatPlatforms[index] != null)
        {
            seatPlatforms[index].transform.position = worldSeatPosition;
        }
    }

    private IEnumerator CheckFaithChanges()
    {
        int previousMonolithCount = GetCurrentMonolithCount();
        while (true)
        {
            yield return new WaitForSeconds(1f); // Check interval
            int currentMonolithCount = GetCurrentMonolithCount();

            // Only log and access player faith if in game mode
            if (Application.isPlaying)
            {
                // Debugging: Log the current faith and monolith count
                Debug.Log($"Current Faith: {player.faith}, Current Monolith Count: {currentMonolithCount}");
            }

            if (currentMonolithCount != previousMonolithCount)
            {
                Debug.Log("Updating Monoliths and Seats due to faith change.");
                UpdateMonolithsAndSeats();
                previousMonolithCount = currentMonolithCount;
            }
        }
    }

    private GameObject CreateMonolithOrSeat(GameObject prefab, Vector3 size, Material material, int index, bool isMonolith)
    {
        // Check if the prefab or material is null
        if (prefab == null)
        {
            Debug.LogError("Prefab is null in CreateMonolithOrSeat");
            return null;
        }

        if (material == null)
        {
            Debug.LogError("Material is null in CreateMonolithOrSeat");
            return null;
        }

        float angle = index * Mathf.PI * 2 / maxMonoliths;
        float radius = isMonolith ? seatPlatformRadius * sizeMultiplier + monolithSpacing * sizeMultiplier : seatPlatformRadius * sizeMultiplier;
        Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
        Vector3 worldPosition = transform.position + position;

        GameObject obj = Instantiate(prefab, worldPosition, Quaternion.identity, transform);

        // Check if the object is null after instantiation
        if (obj == null)
        {
            //Debug.LogWarning("Instantiated GameObject is null in CreateMonolithOrSeat");
            return null;
        }

        // Set scale
        obj.transform.localScale = size * sizeMultiplier;

        // Set material
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }
        else
        {
            Debug.LogWarning("Renderer not found on the instantiated GameObject in CreateMonolithOrSeat");
        }

        // Ground check and position adjustment
        if (Application.isPlaying)
        {
            GroundCheck(obj);
        }
        else
        {
            Vector3 editorPosition = obj.transform.position;
            editorPosition.y = 0; // Adjust height in editor mode
            obj.transform.position = editorPosition;
        }

        // Rotate the monolith/seat to face the center, ensuring obj is not destroyed
        if (isMonolith && obj != null)
        {
            Vector3 directionToCenter = (transform.position - obj.transform.position).normalized;
            obj.transform.rotation = Quaternion.LookRotation(directionToCenter);
        }

        return obj;
    }

    public float lerpScaleTime = 2f;

    private Dictionary<GameObject, Coroutine> descendCoroutines = new Dictionary<GameObject, Coroutine>();

    private void AnimateRising(GameObject obj)
    {
        Vector3 defaultSize = obj.transform.localScale;

        Vector3 startScale = new Vector3(obj.transform.localScale.x, 0, obj.transform.localScale.z);
        Vector3 endScale = new Vector3(obj.transform.localScale.x, defaultSize.y, obj.transform.localScale.z);

        StartCoroutine(ScaleObject(obj, startScale, endScale, lerpScaleTime));
    }

    private void AnimateDescend(GameObject obj)
    {
        Vector3 currentScale = obj.transform.localScale;
        Vector3 endScale = new Vector3(currentScale.x, 0, currentScale.z);

        // Check if there's an ongoing animation and stop it
        if (movementCoroutines.ContainsKey(obj) && movementCoroutines[obj] != null)
        {
            StopCoroutine(movementCoroutines[obj]);
        }

        Coroutine coroutine = StartCoroutine(ScaleObject(obj, currentScale, endScale, lerpScaleTime));
        movementCoroutines[obj] = coroutine; // Keep track of the coroutine
    }

    private IEnumerator AnimateDescendAndDisable(GameObject obj)
    {
        AnimateDescend(obj);
        yield return new WaitForSeconds(lerpScaleTime);
        obj.SetActive(false);
    }

    IEnumerator ScaleObject(GameObject obj, Vector3 startScale, Vector3 endScale, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            float newYScale = Mathf.Lerp(startScale.y, endScale.y, time / duration);
            obj.transform.localScale = new Vector3(startScale.x, newYScale, startScale.z);
            time += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = endScale;
    }

    public float repositionLerpTime = 2f;

    private Dictionary<GameObject, Coroutine> movementCoroutines = new Dictionary<GameObject, Coroutine>();

    private void RepositionMonolithAndSeat(int index)
    {
        float angle = index * Mathf.PI * 2 / GetCurrentMonolithCount();
        float monolithRadius = seatPlatformRadius * sizeMultiplier + monolithSpacing * sizeMultiplier;
        float seatRadius = seatPlatformRadius * sizeMultiplier;

        Vector3 monolithPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * monolithRadius;
        Vector3 seatPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * seatRadius;

        Vector3 worldMonolithPosition = transform.position + monolithPosition;
        Vector3 worldSeatPosition = transform.position + seatPosition;

        if (index < monoliths.Count)
        {
            StartMoveObjectCoroutine(monoliths[index], worldMonolithPosition, repositionLerpTime);
        }

        if (index < seatPlatforms.Count)
        {
            StartMoveObjectCoroutine(seatPlatforms[index], worldSeatPosition, repositionLerpTime);
        }
    }

    private void StartMoveObjectCoroutine(GameObject obj, Vector3 targetPosition, float duration)
    {
        if (movementCoroutines.ContainsKey(obj) && movementCoroutines[obj] != null)
        {
            StopCoroutine(movementCoroutines[obj]);
        }

        Coroutine coroutine = StartCoroutine(MoveObject(obj, targetPosition, duration));
        movementCoroutines[obj] = coroutine;
    }

    IEnumerator MoveObject(GameObject obj, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = obj.transform.position;
        float time = 0;

        while (time < duration)
        {
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = targetPosition;
    }


    private int GetCurrentMonolithCount()
    {
        float currentFaith;

        float minStat, maxStat;

        if (Application.isPlaying)
        {
            currentFaith = player.faith;
            minStat = playerStats.minStat;
            maxStat = playerStats.maxStat;
        }
        else
        {
            currentFaith = faithFactor;
            minStat = 0f; // Default minimum stat
            maxStat = 1f; // Default maximum stat
        }

        float faithRatio = Mathf.Clamp((currentFaith - minStat) / (maxStat - minStat), 0f, 1f);
        return Mathf.RoundToInt(minMonoliths + (maxMonoliths - minMonoliths) * faithRatio);
    }

    private void UpdateMonolithsAndSeats()
    {
        int currentCount = GetCurrentMonolithCount();

        // Check for null or empty lists
        if (monoliths == null || seatPlatforms == null)
            return;

        for (int i = 0; i < monoliths.Count; i++)
        {
            if (i < currentCount)
            {
                if (monoliths[i] != null && !monoliths[i].activeSelf)
                {
                    monoliths[i].SetActive(true);
                    seatPlatforms[i].SetActive(true);
                    AnimateRising(monoliths[i]);
                    AnimateRising(seatPlatforms[i]);
                }

                // Reposition monoliths and seats
                RepositionMonolithAndSeat(i);
            }
            else if (monoliths[i] != null && monoliths[i].activeSelf)
            {
                StartCoroutine(AnimateDescendAndDisable(monoliths[i]));
                StartCoroutine(AnimateDescendAndDisable(seatPlatforms[i]));
            }
        }
    }

    private void CreateNPCApproachPosition(GameObject seat)
    {
        if (seat == null) return;

        GameObject approachPosition = new GameObject("SeatTarget");
        approachPosition.transform.SetParent(seat.transform);
        // Calculate the direction towards the center from the seat
        Vector3 directionToCenter = (centerPlatform.transform.position - seat.transform.position).normalized;

        // Rotate the seat to face the center
        seat.transform.rotation = Quaternion.LookRotation(directionToCenter);

        // Position the approach point a bit in front of the seat, towards the center platform.
        approachPosition.transform.localPosition = Vector3.forward * distanceInFrontOfSeat;

        // Ensure the target is on the ground in play mode.
        if (Application.isPlaying)
        {
            // Move the approach position up by the raycast offset before casting the ray down.
            Vector3 worldPosition = approachPosition.transform.position + Vector3.up * raycastOffset;
            if (Physics.Raycast(worldPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundAndWaterLayerMask))
            {
                // Place the approach position directly on the ground at the hit point.
                approachPosition.transform.position = hit.point;
            }
        }
        else
        {
            // In editor mode, simply adjust the height of the approach position as needed.
            approachPosition.transform.localPosition += Vector3.down * 0.1f;
        }

        // Debugging: Draw a line in the editor to visualize the direction and distance.
        Debug.DrawLine(seat.transform.position, approachPosition.transform.position, Color.red, 5f);
    }

    void GroundCheck(GameObject obj)
    {
        obj.transform.position += Vector3.up * raycastOffset;

        if (obj != null && Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, groundAndWaterLayerMask))
        {
            if (hit.collider.CompareTag(waterTag) || hit.collider == null)
            {
                DestroyObject(obj);
            }
            else
            {
                AnchorToGround(obj);
            }
        }
    }

    private void DestroyObject(GameObject obj)
    {
        if (Application.isEditor)
        {
            DestroyImmediate(obj);
        }
        else
        {
            Destroy(obj);
        }
    }

    void AnchorToGround(GameObject obj)
    {
        if (obj != null && Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, groundLayer))
        {
            float distance = hitFloor.distance;
            Vector3 newPosition = obj.transform.position - Vector3.up * distance;
            obj.transform.position = newPosition;
        }
    }

    private void AddOrUpdateSphereCollider(float radius, float spacing)
    {
        if (templeColliderObject == null)
        {
            templeColliderObject = new GameObject("TempleCollider");
            templeColliderObject.transform.SetParent(transform);
            templeColliderObject.layer = LayerMask.NameToLayer("DeadZone");
            templeColliderObject.AddComponent<SphereCollider>();
        }

        SphereCollider collider = templeColliderObject.GetComponent<SphereCollider>();
        collider.isTrigger = false;

        float colliderRadius = radius + spacing + (seatPlatformSize.x * sizeMultiplier) / 2;
        collider.center = new Vector3(0, -raycastOffset, 0);
        collider.radius = colliderRadius * colliderRadius / 2;
        templeColliderObject.transform.position = transform.position;
    }

    public void ClearTemple()
    {
        // Ensure seatPlatforms is initialized and not null
        if (seatPlatforms != null)
        {
            // Destroy child objects of each seat first
            foreach (GameObject seat in seatPlatforms)
            {
                if (seat != null)
                {
                    foreach (Transform child in seat.transform)
                    {
                        if (Application.isPlaying)
                        {
                            Destroy(child.gameObject);
                        }
                        else
                        {
                            DestroyImmediate(child.gameObject);
                        }
                    }
                }
            }
        }

        // Now destroy the parent objects
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        // Clear lists and dictionary
        if (monoliths != null) monoliths.Clear();
        if (seatPlatforms != null) seatPlatforms.Clear();
        if (seatAvailability != null) seatAvailability.Clear();

        isGenerated = false;
    }

    private void OnDisable()
    {
        // Stop coroutine if it's running
        if (faithCheckCoroutine != null)
        {
            StopCoroutine(faithCheckCoroutine);
            faithCheckCoroutine = null;
        }

        // Clear generated temples
        ClearTemple();
    }

    public bool CheckSeatAvailability(GameObject seat)
    {
        return seatAvailability.TryGetValue(seat, out bool isAvailable) && isAvailable;
    }

    public bool CheckCenterPlatformAvailability()
    {
        return isCenterPlatformAvailable;
    }

    public void SetSeatAvailability(GameObject seat, bool available)
    {
        if (seatAvailability.ContainsKey(seat))
        {
            seatAvailability[seat] = available;
        }
    }

    public void SetCenterPlatformAvailability(bool available)
    {
        isCenterPlatformAvailable = available;
    }

    public void LerpMonolithScale(float scaleFactor)
    {
        foreach (var monolith in monoliths)
        {
            monolith.transform.localScale = Vector3.Lerp(monolith.transform.localScale, monolithSize * scaleFactor, Time.deltaTime);
        }
    }

    public void LerpSeatPlatformScale(float scaleFactor)
    {
        foreach (var seatPlatform in seatPlatforms)
        {
            seatPlatform.transform.localScale = Vector3.Lerp(seatPlatform.transform.localScale, seatPlatformSize * scaleFactor, Time.deltaTime);
        }
    }

    public void LerpCenterPlatformScale(float scaleFactor)
    {
        if (centerPlatform != null)
        {
            centerPlatform.transform.localScale = Vector3.Lerp(centerPlatform.transform.localScale, centerPlatformSize * scaleFactor, Time.deltaTime);
        }
    }

}

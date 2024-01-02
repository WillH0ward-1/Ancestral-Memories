using UnityEngine;
using System.Collections.Generic;

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

    public int monolithCount = 8;
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


    public void GenerateTemple()
    {
        if (isGenerated)
        {
            ClearTemple();
        }

        monoliths = new List<GameObject>();
        seatPlatforms = new List<GameObject>();

        seatAvailability = new Dictionary<GameObject, bool>();

        float appliedMonolithRadius = seatPlatformRadius * sizeMultiplier;
        float appliedMonolithSpacing = monolithSpacing * sizeMultiplier;
        AddOrUpdateSphereCollider(appliedMonolithRadius, appliedMonolithSpacing);

        Vector3 appliedMonolithSize = monolithSize * sizeMultiplier;
        Vector3 appliedCenterPlatformSize = centerPlatformSize * sizeMultiplier;
        Vector3 appliedSeatPlatformSize = seatPlatformSize * sizeMultiplier;

        defaultMonolithSize = appliedMonolithSize;
        defaultCenterPlatformSize = appliedCenterPlatformSize;
        defaultSeatPlatformSize = appliedSeatPlatformSize;

        centerPlatform = CreateObject(centerPlatformPrefab, transform.position, appliedCenterPlatformSize, centerPlatformMaterial);
        isCenterPlatformAvailable = false; // Center platform is not available initially

        for (int i = 0; i < monolithCount; i++)
        {
            float angle = i * Mathf.PI * 2 / monolithCount;
            Vector3 monolithPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (appliedMonolithRadius + appliedMonolithSpacing);
            Vector3 worldMonolithPosition = transform.position + monolithPosition;
            monoliths.Add(CreateObject(monolithPrefab, worldMonolithPosition, appliedMonolithSize, monolithMaterial));
        }

        for (int i = 0; i < monolithCount; i++)
        {
            float angle = i * Mathf.PI * 2 / monolithCount;
            Vector3 seatPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * appliedMonolithRadius;
            Vector3 worldSeatPosition = transform.position + seatPosition;
            GameObject seat = CreateObject(seatPlatformPrefab, worldSeatPosition, appliedSeatPlatformSize, seatPlatformMaterial);
            if (seat != null)
            {
                seatPlatforms.Add(seat);
                seatAvailability[seat] = false; // Seats are not available initially

                CreateNPCApproachPosition(seat);
            }
        }

        isGenerated = true;
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


    private GameObject CreateObject(GameObject prefab, Vector3 position, Vector3 size, Material material)
    {
        GameObject obj = Instantiate(prefab, position, Quaternion.identity, transform);
        obj.transform.localScale = size;

        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        if (Application.isPlaying)
        {
            GroundCheck(obj);
        }
        else
        {
            Vector3 editorPosition = obj.transform.position;
            editorPosition.y = 0;
            obj.transform.position = editorPosition;
        }

        return obj;
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

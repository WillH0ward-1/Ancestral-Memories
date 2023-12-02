using UnityEngine;

public class TempleGenerator : MonoBehaviour
{
    public GameObject monolithPrefab;
    public GameObject centerPlatformPrefab;
    public GameObject smallPlatformPrefab;

    public Vector3 monolithSize = Vector3.one;
    public Vector3 centerPlatformSize = Vector3.one;
    public Vector3 smallPlatformSize = Vector3.one;

    public Material monolithMaterial;
    public Material centerPlatformMaterial;
    public Material smallPlatformMaterial;

    public int monolithCount = 8;
    public float baseCircleRadius = 10f;
    public float basePlatformSpacing = 5f;
    [SerializeField] private float sizeMultiplier = 1f; // Serialized property for size multiplier

    [SerializeField] private bool isGenerated = false;
    private float raycastOffset = 10f;

    [SerializeField] private LayerMask groundAndWaterLayerMask;
    [SerializeField] private LayerMask groundLayer;

    private string waterTag = "Water";

    private GameObject templeColliderObject;

    float appliedCircleRadius;
    float appliedPlatformSpacing;

    private void Awake()
    {
        appliedCircleRadius = baseCircleRadius * sizeMultiplier;
        appliedPlatformSpacing = basePlatformSpacing * sizeMultiplier;
        AddOrUpdateSphereCollider(appliedCircleRadius, appliedPlatformSpacing);
    }

    public void GenerateTemple()
    {
        if (isGenerated)
        {
            ClearTemple();
        }

        Vector3 appliedMonolithSize = monolithSize * sizeMultiplier;
        Vector3 appliedCenterPlatformSize = centerPlatformSize * sizeMultiplier;
        Vector3 appliedSmallPlatformSize = smallPlatformSize * sizeMultiplier;

        for (int i = 0; i < monolithCount; i++)
        {
            float angle = i * Mathf.PI * 2 / monolithCount;
            Vector3 localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * appliedCircleRadius;
            Vector3 worldPosition = transform.position + localPosition;
            CreateObject(monolithPrefab, worldPosition, appliedMonolithSize, monolithMaterial);
        }

        CreateObject(centerPlatformPrefab, transform.position, appliedCenterPlatformSize, centerPlatformMaterial);

        for (int i = 0; i < monolithCount; i++)
        {
            float angle = i * Mathf.PI * 2 / monolithCount;
            Vector3 localPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (appliedCircleRadius + appliedPlatformSpacing);
            Vector3 worldPosition = transform.position + localPosition;
            CreateObject(smallPlatformPrefab, worldPosition, appliedSmallPlatformSize, smallPlatformMaterial);
        }

        isGenerated = true;
    }

    private void CreateObject(GameObject prefab, Vector3 position, Vector3 size, Material material)
    {
        // Instantiate the object at the given position
        GameObject obj = Instantiate(prefab, position + Vector3.up * 50, Quaternion.identity, transform);
        obj.transform.localScale = size;

        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        // Add ShaderLightColor component to each first-level child of the instantiated object
        foreach (Transform child in obj.transform)
        {
            if (child.gameObject.GetComponent<ShaderLightColor>() == null)
            {
                child.gameObject.AddComponent<ShaderLightColor>();
            }
        }

        // Perform ground check and adjustment only if in play mode
        if (Application.isPlaying)
        {
            GroundCheck(obj);
        }
    }



    void GroundCheck(GameObject obj)
    {
        obj.transform.position += Vector3.up * 50;

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
        if (obj != null)
        {
            if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, groundLayer))
            {
                float distance = hitFloor.distance;

                float x = obj.transform.position.x;
                float y = obj.transform.position.y - distance;
                float z = obj.transform.position.z;

                Vector3 newPosition = new Vector3(x, y, z);

                obj.transform.position = newPosition;

                //Debug.Log("Clamped to Ground!");
                //Debug.Log("Distance: " + distance);
            }
        }

    }

    private void AddOrUpdateSphereCollider(float circleRadius, float platformSpacing)
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

        // Calculate the radius to cover the small platforms, which are the furthest from the center
        float smallPlatformOuterEdge = circleRadius + platformSpacing + (smallPlatformSize.x * sizeMultiplier) / 2;
        float colliderRadius = Mathf.Max(smallPlatformOuterEdge, circleRadius + platformSpacing) + 1; // Adding an extra unit for buffer

        collider.center = new Vector3(0, -50f, 0); // Adjust Y offset as needed
        collider.radius = colliderRadius * colliderRadius / 2; // Adjust radius based on the farthest extent of the temple
        templeColliderObject.transform.position = transform.position;
    }



    public void ClearTemple()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
        isGenerated = false;
    }
}

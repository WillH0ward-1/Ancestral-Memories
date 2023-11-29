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
    public float circleRadius = 10f;
    public float platformSpacing = 5f;

    private bool isGenerated = false;
    private float raycastOffset = 10f;
    private LayerMask groundLayer;

    private GameObject templeColliderObject;

    private void Awake()
    {
        groundLayer = LayerMask.NameToLayer("Ground");

        GenerateTemple();
    }

    public void GenerateTemple()
    {
        if (isGenerated)
        {
            ClearTemple();
        }

        for (int i = 0; i < monolithCount; i++)
        {
            float angle = i * Mathf.PI * 2 / monolithCount;
            Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * circleRadius;
            CreateObject(monolithPrefab, position, monolithSize, monolithMaterial);
        }

        CreateObject(centerPlatformPrefab, Vector3.zero, centerPlatformSize, centerPlatformMaterial);

        for (int i = 0; i < monolithCount; i++)
        {
            float angle = i * Mathf.PI * 2 / monolithCount;
            Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * (circleRadius + platformSpacing);
            CreateObject(smallPlatformPrefab, position, smallPlatformSize, smallPlatformMaterial);
        }

        AddOrUpdateSphereCollider();

        isGenerated = true;
    }

    private void CreateObject(GameObject prefab, Vector3 position, Vector3 size, Material material)
    {
        GameObject obj = Instantiate(prefab, position + Vector3.up * raycastOffset, Quaternion.identity, transform);
        obj.transform.localScale = size;
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        if (Application.isPlaying)
        {
            if (!GroundCheck(obj))
            {
                Destroy(obj);
            }
        }
        else
        {
            obj.transform.position = position;
        }
    }

    private bool GroundCheck(GameObject obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            float heightOffset = obj.transform.localScale.y / 2.0f;
            obj.transform.position = hit.point + Vector3.up * heightOffset;
            return true;
        }
        return false;
    }

    private void AddOrUpdateSphereCollider()
    {
        if (templeColliderObject == null)
        {
            templeColliderObject = new GameObject("TempleCollider");
            templeColliderObject.transform.SetParent(transform);
            templeColliderObject.layer = LayerMask.NameToLayer("DeadZone");
            templeColliderObject.AddComponent<SphereCollider>();
        }

        SphereCollider collider = templeColliderObject.GetComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.center = new Vector3(0, -25f, 0); // Adjust Y offset as needed
        collider.radius = circleRadius + platformSpacing + 3; // Adjust radius as needed

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

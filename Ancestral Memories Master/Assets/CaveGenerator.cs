using System.Collections.Generic;
using UnityEngine;

public class CaveGenerator : MonoBehaviour
{
    public bool useScaledRocksOnSphere = true;
    public bool useScaledRocksOnCylinder = true;

    public float sphereRadius = 25f;
    public int rockCount = 100;
    public int cylinderRockCount = 50;

    [Header("Sphere Rock Settings")]
    [SerializeField] private Vector3 minSphereRockScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 maxSphereRockScale = new Vector3(2f, 2f, 2f);

    [Header("Cylinder Rock Settings")]
    [SerializeField] private Vector3 minCylinderRockScale = new Vector3(0.3f, 0.3f, 0.3f);
    [SerializeField] private Vector3 maxCylinderRockScale = new Vector3(1.5f, 1.5f, 1.5f);

    [Header("Door Settings")]
    [SerializeField] private Vector3 doorScale = new Vector3(5, 5, 5);
    [SerializeField] private float cylinderYPosition = 0f;

    [Header("Door Rotation Settings")]
    [SerializeField] private Vector3 rotationAdjustment = new Vector3(90, 180, 0);

    [SerializeField] private Material sphereMaterial;
    [SerializeField] private Material duplicateSphereMaterial;
    [SerializeField] private Material rockMaterial;
    [SerializeField] private GameObject cylinderPrefab;

    [Header("Scaled Cylinder Rock Settings")]
    [SerializeField] private Vector3 minCylinderScaleAtHighest = new Vector3(0.3f, 0.3f, 0.3f);
    [SerializeField] private Vector3 maxCylinderScaleAtLowest = new Vector3(1.5f, 1.5f, 1.5f);

    private List<GameObject> generatedElements = new List<GameObject>();

    [SerializeField] private LayerMask groundLayerMask;

    [Header("Duplicate Sphere Settings")]
    [SerializeField] private float duplicateSphereScaleMultiplier = 1f; // Added line

    private void Awake()
    {
        GenerateCave();
    }

    public void GenerateCave()
    {
        ClearExistingCave();
        GameObject cave = GeneratePrimitive(PrimitiveType.Sphere, Vector3.zero, new Vector3(sphereRadius * 2, sphereRadius * 2, sphereRadius * 2));
        if (sphereMaterial != null)
        {
            cave.GetComponent<Renderer>().material = sphereMaterial;
        }

        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 doorPosition = randomDirection * sphereRadius;
        doorPosition.y = cylinderYPosition;
        Quaternion orientation = Quaternion.LookRotation(Vector3.zero - doorPosition);
        GameObject door = Instantiate(cylinderPrefab, doorPosition, orientation);
        door.transform.Rotate(rotationAdjustment, Space.Self);
        door.transform.SetParent(transform, true);
        door.transform.localScale = doorScale;
        door.transform.localRotation = Quaternion.Euler(90, door.transform.localRotation.eulerAngles.y, door.transform.localRotation.eulerAngles.z);

        if (rockMaterial != null)
        {
            door.GetComponent<Renderer>().material = rockMaterial;
        }

        // Create a duplicate sphere at the position of the cylinder (door)
        GameObject duplicateSphere = GeneratePrimitive(PrimitiveType.Sphere, doorPosition, new Vector3(sphereRadius * 2 * duplicateSphereScaleMultiplier, sphereRadius * 2 * duplicateSphereScaleMultiplier, sphereRadius * 2 * duplicateSphereScaleMultiplier));
        if (sphereMaterial != null)
        {
            duplicateSphere.GetComponent<Renderer>().material = duplicateSphereMaterial;
        }
        for (int i = 0; i < rockCount; i++)
        {
            GenerateRockOnSphere(cave.transform.position, sphereRadius, door.transform.position, door.transform.localScale, useScaledRocksOnSphere);
        }
        for (int i = 0; i < cylinderRockCount; i++)
        {
            GenerateRockOnCylinder(door, minCylinderRockScale, maxCylinderRockScale, useScaledRocksOnCylinder);
        }
        generatedElements.Add(door);
    }

    private GameObject GeneratePrimitive(PrimitiveType type, Vector3 position, Vector3 scale)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.transform.SetParent(transform);

        if (rockMaterial != null)
        {
            obj.GetComponent<Renderer>().material = rockMaterial;
        }

        obj.transform.gameObject.AddComponent<ShaderLightColor>();

        generatedElements.Add(obj);
        return obj;
    }

    private void GenerateRockOnSphere(Vector3 sphereCenter, float sphereRadius, Vector3 cylinderCenter, Vector3 cylinderScale, bool scaleBasedOnHeight)
    {
        Vector3 randomDirection = Random.onUnitSphere;
        Vector3 position = sphereCenter + randomDirection * sphereRadius;
        Vector3 randomScale = new Vector3(
            Random.Range(minSphereRockScale.x, maxSphereRockScale.x),
            Random.Range(minSphereRockScale.y, maxSphereRockScale.y),
            Random.Range(minSphereRockScale.z, maxSphereRockScale.z)
        );
        float distanceToCylinderCenter = Vector3.Distance(cylinderCenter, position);
        float cylinderRadius = cylinderScale.x * 0.5f;
        if (distanceToCylinderCenter >= cylinderRadius)
        {
            if (scaleBasedOnHeight)
            {
                float normalizedY = Mathf.InverseLerp(-sphereRadius, sphereRadius, position.y);
                randomScale = Vector3.Lerp(maxSphereRockScale, minSphereRockScale, normalizedY);
            }

            GameObject rock = GeneratePrimitive(PrimitiveType.Sphere, position, randomScale);
            CheckIfUnderGround(rock);
        }
    }

    private void GenerateRockOnCylinder(GameObject cylinderPrefab, Vector3 minScale, Vector3 maxScale, bool scaleBasedOnHeight)
    {
        Transform cylinderTransform = cylinderPrefab.transform;
        Vector3 cylinderLocalScale = cylinderPrefab.transform.localScale;
        Vector3 cylinderWorldScale = Vector3.Scale(cylinderTransform.parent.lossyScale, cylinderLocalScale);

        float theta = Random.Range(0f, 2 * Mathf.PI);
        float y = Random.Range(-cylinderWorldScale.y * 0.5f, cylinderWorldScale.y * 0.5f);
        float radius = cylinderWorldScale.x * 0.5f;

        Vector3 position = new Vector3(
            Mathf.Cos(theta) * radius,
            y,
            Mathf.Sin(theta) * radius
        );

        position = cylinderTransform.position + cylinderTransform.rotation * position;

        Vector3 randomScale = new Vector3(
            Random.Range(minScale.x, maxScale.x),
            Random.Range(minScale.y, maxScale.y),
            Random.Range(minScale.z, maxScale.z)
        );

        if (scaleBasedOnHeight)
        {
            float normalizedY = Mathf.InverseLerp(-cylinderWorldScale.y * 0.5f, cylinderWorldScale.y * 0.5f, y);
            randomScale = Vector3.Lerp(maxCylinderScaleAtLowest, minCylinderScaleAtHighest, normalizedY);
        }

        GameObject rock = GeneratePrimitive(PrimitiveType.Sphere, position, randomScale);
        CheckIfUnderGround(rock);

    }
    private bool CheckIfUnderGround(GameObject obj)
    {
        Vector3 position = obj.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.up, out hit, Mathf.Infinity, groundLayerMask))
        {
            if (Application.isPlaying)
            {
                Destroy(obj);
            }
            else
            {
                DestroyImmediate(obj, false);
            }
            generatedElements.Remove(obj);
            return true;
        }
        return false;
    }


    public void ClearExistingCave()
    {
        foreach (GameObject go in generatedElements)
        {
            if (Application.isPlaying)
            {
                Destroy(go);
            }
            else
            {
                DestroyImmediate(go, false);
            }
        }

        generatedElements.Clear();
    }
}

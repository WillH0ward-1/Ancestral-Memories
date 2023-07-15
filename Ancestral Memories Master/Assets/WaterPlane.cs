using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterPlane : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float waveScale = 1.0f;
    public float timeMultiplier = 1.0f;
    public int subdivisionLevel = 1;

    public Material sidePlaneMaterial;
    public float planeScaleMultiplier = 10.0f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material material;
    private Vector3[] baseVertices;
    [SerializeField] private float yPos = 0;

    private void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.sharedMaterial;
        baseVertices = meshFilter.sharedMesh.vertices;
        //yPos = transform.localPosition.y;
        GenerateSidePlanes();
    }

    private void GenerateSidePlanes()
    {
        Vector3 rootPlanePosition = transform.position;
        Vector3 rootPlaneScale = transform.localScale;
        float offsetX = rootPlaneScale.x * planeScaleMultiplier;
        float offsetZ = rootPlaneScale.z * planeScaleMultiplier;

        // Create a parent object for the side planes
        GameObject sidePlaneParent = new GameObject("SidePlaneParent");
        sidePlaneParent.transform.SetParent(transform);

        // Generate planes at X positions
        CreateSidePlane(rootPlanePosition + Vector3.left * offsetX, yPos, sidePlaneParent);
        CreateSidePlane(rootPlanePosition + Vector3.right * offsetX, yPos, sidePlaneParent);

        // Generate planes at Z positions
        CreateSidePlane(rootPlanePosition + Vector3.forward * offsetZ, yPos, sidePlaneParent);
        CreateSidePlane(rootPlanePosition + Vector3.back * offsetZ, yPos, sidePlaneParent);

        // Generate planes at X+Z positions
        CreateSidePlane(rootPlanePosition + Vector3.right * offsetX + Vector3.back * offsetZ, yPos, sidePlaneParent);
        CreateSidePlane(rootPlanePosition + Vector3.left * offsetX + Vector3.forward * offsetZ, yPos, sidePlaneParent);
        CreateSidePlane(rootPlanePosition + Vector3.left * offsetX + Vector3.back * offsetZ, yPos, sidePlaneParent);
        CreateSidePlane(rootPlanePosition + Vector3.right * offsetX + Vector3.forward * offsetZ, yPos, sidePlaneParent);
    }

    private void CreateSidePlane(Vector3 position, float yPosition, GameObject parent)
    {
        GameObject sidePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sidePlane.name = "SidePlane";
        sidePlane.transform.localScale = transform.localScale; // Use the original scale of the root plane

        Vector3 sidePlanePosition = new Vector3(position.x, yPosition, position.z);
        sidePlane.transform.position = sidePlanePosition;

        sidePlane.transform.SetParent(parent.transform);

        MeshRenderer sidePlaneRenderer = sidePlane.GetComponent<MeshRenderer>();
        sidePlaneRenderer.sharedMaterial = new Material(sidePlaneMaterial); // Create a new material instance

        // Copy the albedo color from the main water plane's material
        sidePlaneRenderer.sharedMaterial.color = meshRenderer.sharedMaterial.color;

        // Copy the texture if needed
        sidePlaneRenderer.sharedMaterial.mainTexture = meshRenderer.sharedMaterial.mainTexture;
    }

}

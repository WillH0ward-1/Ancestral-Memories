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

    private void OnEnable()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.sharedMaterial;
        baseVertices = meshFilter.sharedMesh.vertices;

        GenerateSidePlanes();
    }

    private void GenerateSidePlanes()
    {
        Vector3 rootPlanePosition = transform.position;
        Vector3 rootPlaneScale = transform.localScale;
        float offsetX = rootPlaneScale.x * planeScaleMultiplier;
        float offsetZ = rootPlaneScale.z * planeScaleMultiplier;

        // Generate planes at X positions
        CreateSidePlane(rootPlanePosition + Vector3.left * offsetX);
        CreateSidePlane(rootPlanePosition + Vector3.right * offsetX);

        // Generate planes at Z positions
        CreateSidePlane(rootPlanePosition + Vector3.forward * offsetZ);
        CreateSidePlane(rootPlanePosition + Vector3.back * offsetZ);

        // Generate planes at X+Z positions
        CreateSidePlane(rootPlanePosition + Vector3.right * offsetX + Vector3.back * offsetZ);
        CreateSidePlane(rootPlanePosition + Vector3.left * offsetX + Vector3.forward * offsetZ);
        CreateSidePlane(rootPlanePosition + Vector3.left * offsetX + Vector3.back * offsetZ);
        CreateSidePlane(rootPlanePosition + Vector3.right * offsetX + Vector3.forward * offsetZ);
    }

    private void CreateSidePlane(Vector3 position)
    {
        GameObject sidePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sidePlane.name = "SidePlane";
        sidePlane.transform.position = position;
        sidePlane.transform.localScale = new Vector3(1f, 1f, 1f);
        sidePlane.transform.SetParent(transform);

        MeshRenderer sidePlaneRenderer = sidePlane.GetComponent<MeshRenderer>();
        sidePlaneRenderer.sharedMaterial = sidePlaneMaterial;

        // Assign the color from the main water plane's material
        sidePlaneRenderer.sharedMaterial.color = meshRenderer.sharedMaterial.color;
    }
}

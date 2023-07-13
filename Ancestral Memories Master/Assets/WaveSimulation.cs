using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaveSimulation : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float waveScale = 1.0f;
    public float timeMultiplier = 1.0f;
    public int subdivisionLevel = 1;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Material material;
    private Vector3[] baseVertices;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.sharedMaterial;
        baseVertices = meshFilter.sharedMesh.vertices;
    }

    private void Update()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        Mesh mesh = meshFilter.sharedMesh;
        Vector3[] vertices = new Vector3[baseVertices.Length];

        float time = Time.time * timeMultiplier;

        for (int i = 0; i < baseVertices.Length; i++)
        {
            Vector3 vertex = baseVertices[i];
            float wave = Mathf.Sin(vertex.x * waveScale + time) * amplitude;
            vertices[i] = vertex + Vector3.up * wave;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}

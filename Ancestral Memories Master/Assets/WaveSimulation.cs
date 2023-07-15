using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class WaveSimulation : MonoBehaviour
{
    public float amplitude = 0.1f;
    public float waveScale = 1.0f;
    public float timeMultiplier = 1.0f;
    public float noiseScale = 0.1f;
    public float fresnelPower = 2.0f;
    public Color albedoColor = Color.white;

    private MeshRenderer meshRenderer;
    private Material material;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.sharedMaterial;
    }

    private void Update()
    {
        material.SetFloat("_Amplitude", amplitude);
        material.SetFloat("_WaveScale", waveScale);
        material.SetFloat("_TimeMultiplier", timeMultiplier);
        material.SetFloat("_NoiseScale", noiseScale);
        material.SetFloat("_FresnelPower", fresnelPower);
        material.SetColor("_Albedo", albedoColor);
    }
}

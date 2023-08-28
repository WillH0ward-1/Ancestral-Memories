using System.Collections;
using UnityEngine;

public class TreeInteractions : MonoBehaviour
{
    public float growing = 1.0f; // Corresponds to "_T" in the shader
    public Color lightColor = Color.white; // Corresponds to "_LightColor" in the shader
    public float noiseScaleX = 1.0f, noiseScaleY = 1.0f, noiseScaleZ = 1.0f; // Corresponds to "_NoiseScaleX", "_NoiseScaleY", "_NoiseScaleZ" in the shader
    public float timeScale = 1.0f; // Corresponds to "_TimeScale" in the shader

    private Material treeMaterial;
    private float shakeDuration = 1.0f; // Duration of the shake

    private void Start()
    {
        // Assuming the tree's material is on the same GameObject, otherwise find it and assign it
        treeMaterial = GetComponent<Renderer>().material;

    }

    public void TreeShake()
    {
        StartCoroutine(ShakeTree());
    }

    IEnumerator ShakeTree()
    {
        treeMaterial.SetFloat("_EnableNoise", 1.0f);
        float timeElapsed = 0.0f;
        float lerpValue;

        while (timeElapsed < shakeDuration)
        {
            lerpValue = Mathf.PingPong(timeElapsed / shakeDuration, 0.02f); // Lerp from 0 to 0.5 to 0
            treeMaterial.SetFloat("_NoiseAmount", lerpValue);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        treeMaterial.SetFloat("_NoiseAmount", 0.0f);
        treeMaterial.SetFloat("_EnableNoise", 0.0f);

        yield break;
    }
}

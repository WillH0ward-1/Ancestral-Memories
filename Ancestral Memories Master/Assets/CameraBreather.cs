using UnityEngine;
using System.Collections;

[ExecuteAlways]

public class BreathingEffect : MonoBehaviour
{
    // Breathing effect variables
    private float time = 0.0f;
    public float breathingSpeed = 1.0f;
    public float movementRange = 0.1f;
    Camera cam;
    private Transform camTransform;
    private Vector3 previousRandomOffset;

    void OnEnable()
    {
        cam = Camera.main;
        camTransform = cam.transform;
        StartCoroutine(ApplyBreathingEffect());
    }

    private IEnumerator ApplyBreathingEffect()
    {
        while (true)
        {
            time += Time.deltaTime * breathingSpeed;

            float offsetX = Mathf.PerlinNoise(time, 0.0f) * 2 - 1;
            float offsetY = Mathf.PerlinNoise(0.0f, time) * 2 - 1;
            float offsetZ = Mathf.PerlinNoise(time, time) * 2 - 1; // Added Z-axis offset
            Vector3 randomOffset = new Vector3(offsetX, offsetY, offsetZ) * movementRange;

            // Remove the previous random offset before applying the new one
            camTransform.transform.position -= previousRandomOffset;
            camTransform.transform.position += randomOffset;

            // Store the new random offset for the next frame
            previousRandomOffset = randomOffset;

            yield return new WaitForEndOfFrame();
        }
    }
}
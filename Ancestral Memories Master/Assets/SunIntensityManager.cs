using UnityEngine;

[ExecuteInEditMode]
public class SunIntensityManager : MonoBehaviour
{
    public float sunIntensity;
    public float minSunIntensity = 0.1f; // Minimum sun intensity
    public float maxSunIntensity = 1.0f; // Maximum sun intensity
    private Light directionalLight;
    private TimeCycleManager timeCycleManager;

    void Start()
    {
        // Get the TimeCycleManager component from the same GameObject
        timeCycleManager = GetComponent<TimeCycleManager>();

        // Get the directional light reference from TimeCycleManager
        if (timeCycleManager != null)
        {
            directionalLight = timeCycleManager.DirectionalLight;
        }

        if (directionalLight == null)
        {
            Debug.LogError("SunIntensityManager: Directional light not found.");
        }
    }

    void Update()
    {
        if (timeCycleManager != null && directionalLight != null)
        {
            timeCycleManager.sunIntensity = CalculateSunIntensity(timeCycleManager.TimeOfDay);
            directionalLight.intensity = sunIntensity;
        }
    }

    private float CalculateSunIntensity(float timeOfDay)
    {
        // Convert timeOfDay to a value from 0 to 1
        float normalizedTime = timeOfDay / 24f;

        // Use a sine wave to modulate the intensity
        // The sine wave peaks at 0.25 (noon) and has a trough at 0.75 (midnight)
        float sunIntensity = Mathf.Sin(normalizedTime * Mathf.PI * 2);

        // Normalize intensity to the range of 0 to 1
        sunIntensity = sunIntensity * 0.5f + 0.5f;

        // Scale intensity between min and max values
        return sunIntensity * (maxSunIntensity - minSunIntensity) + minSunIntensity;
    }

}

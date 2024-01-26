using UnityEngine;

[ExecuteInEditMode]
public class SunIntensityManager : MonoBehaviour
{
    public float sunIntensity;
    public float minSunIntensity = 0.1f; // Minimum sun intensity
    public float maxSunIntensity = 0.8f; // Maximum sun intensity
    public Light directionalLight;
    public TimeCycleManager timeCycleManager;

    void Update()
    {
        if (timeCycleManager != null && directionalLight != null)
        {
            sunIntensity = CalculateSunIntensity(timeCycleManager.TimeOfDay);
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

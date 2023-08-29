using System.Collections;
using OccaSoftware.Buto;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogControl : MonoBehaviour
{
    public Volume volume;
    private VolumetricFog fog;

    // Variables to control fog parameters
    public VolumetricFogMode fogMode = VolumetricFogMode.Off;
    public int sampleCount = 64;
    public bool animateSamplePosition = false;
    public bool selfShadowingEnabled = true;
    public int maximumSelfShadowingOctaves = 1;
    public bool horizonShadowingEnabled = true;
    public float maxDistanceVolumetric = 64f;
    public bool analyticFogEnabled = false;
    public float maxDistanceAnalytic = 5000f;
    public bool temporalAntiAliasingEnabled = false;
    public float temporalAntiAliasingIntegrationRate = 0.03f;
    public float fogDensity = 5f;
    public float anisotropy = 0.2f;
    public float lightIntensity = 1f;
    public float shadowIntensity = 1f;
    public float baseHeight = 0f;
    public float attenuationBoundarySize = 10f;
    public Color litColor = Color.white;
    public Color shadowedColor = Color.white;
    public Color emitColor = Color.white;
    public Texture2D colorRamp;
    public float colorInfluence = 0f;
    public int octaves = 1;
    public float lacunarity = 2f;
    public float gain = 0.3f;
    public float noiseTiling = 50f;
    public Vector3 noiseWindSpeed = new Vector3(0, 0, 0);
    public Vector3 defaultNoiseWindSpeed = new Vector3(2, 2, -2);
    public Vector2 noiseMap = new Vector2(0, 1);

    void Start()
    {
        // Get the VolumetricFog component from the Volume
        if (volume.profile.TryGet(out fog))
        {
            UpdateFogParameters();
        }
        else
        {
            Debug.LogWarning("VolumetricFog component not found in the Volume.");
        }

        noiseWindSpeed = defaultNoiseWindSpeed;
    }

    public IEnumerator LerpFogDensity(float lerpDuration, float normalizedDensityTarget)
    {
        float timeElapsed = 0;

        // Clamp the target density and map it to the 0-20 range
        float targetDensity = Mathf.Clamp(normalizedDensityTarget, 0, 1) * 20;

        float startDensity = fog.fogDensity.value;

        while (timeElapsed < lerpDuration)
        {
            fog.fogDensity.value = Mathf.Lerp(startDensity, targetDensity, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the target density is set when the lerp is done
        fog.fogDensity.value = targetDensity;
    }

    public IEnumerator RandomizeFogWindDirection()
    {
        float time = 0f;
        float timeStep = 0.1f; // Controls the speed of change
        float noiseScale = 2f; // Controls the magnitude of change

        // Loop indefinitely to keep changing wind direction
        while (true)
        {
            // Generate a new wind direction using Perlin noise
            Vector3 targetWindDirection = new Vector3(
                Mathf.PerlinNoise(time, 0f) * 2f - 1f,
                Mathf.PerlinNoise(0f, time) * 2f - 1f,
                Mathf.PerlinNoise(time, time) * 2f - 1f
            ) * noiseScale;

            // Start and wait for the LerpFogWindDirection coroutine to finish
            yield return StartCoroutine(LerpFogWindDirection(5f, targetWindDirection));

            // Wait for a semi-random but smooth amount of time before the next change
            time += timeStep;

            yield return new WaitForSeconds(timeStep);
        }
    }


    public IEnumerator LerpFogWindDirection(float lerpDuration, Vector3 targetWindDirection)
    {
        float timeElapsed = 0;

        Vector3 startWindDirection = fog.noiseWindSpeed.value;

        while (timeElapsed < lerpDuration)
        {
            fog.noiseWindSpeed.value = Vector3.Lerp(startWindDirection, targetWindDirection, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the target wind direction is set when the lerp is done
        fog.noiseWindSpeed.value = targetWindDirection;
    }

    void Update()
    {
        UpdateFogParameters();
    }

    void UpdateFogParameters()
    {
        if (fog == null) return;

        // Update fog parameters
        fog.mode.value = fogMode;
        fog.sampleCount.value = sampleCount;
        fog.animateSamplePosition.value = animateSamplePosition;
        fog.selfShadowingEnabled.value = selfShadowingEnabled;
        fog.maximumSelfShadowingOctaves.value = maximumSelfShadowingOctaves;
        fog.horizonShadowingEnabled.value = horizonShadowingEnabled;
        fog.maxDistanceVolumetric.value = maxDistanceVolumetric;
        fog.analyticFogEnabled.value = analyticFogEnabled;
        fog.maxDistanceAnalytic.value = maxDistanceAnalytic;
        fog.temporalAntiAliasingEnabled.value = temporalAntiAliasingEnabled;
        fog.temporalAntiAliasingIntegrationRate.value = temporalAntiAliasingIntegrationRate;
        fog.fogDensity.value = fogDensity;
        fog.anisotropy.value = anisotropy;
        fog.lightIntensity.value = lightIntensity;
        fog.shadowIntensity.value = shadowIntensity;
        fog.baseHeight.value = baseHeight;
        fog.attenuationBoundarySize.value = attenuationBoundarySize;
        fog.litColor.value = litColor;
        fog.shadowedColor.value = shadowedColor;
        fog.emitColor.value = emitColor;
        fog.colorRamp.value = colorRamp;
        fog.colorInfluence.value = colorInfluence;
        fog.octaves.value = octaves;
        fog.lacunarity.value = lacunarity;
        fog.gain.value = gain;
        fog.noiseTiling.value = noiseTiling;
        fog.noiseWindSpeed.value = noiseWindSpeed;
        fog.noiseMap.value = noiseMap;
    }
}

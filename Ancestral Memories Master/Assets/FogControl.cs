using System.Collections;
using OccaSoftware.Buto;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FogControl : MonoBehaviour
{
    public Volume volume;
    private VolumetricFog fog;
    public VolumeComponent volumeComponent;

    // Variables to control fog parameters
    public VolumetricFogMode fogMode = VolumetricFogMode.On;

    public int sampleCount = 32;
    public bool animateSamplePosition = true;
    public bool selfShadowingEnabled = false;
    public int maximumSelfShadowingOctaves = 1;
    public bool horizonShadowingEnabled = false;
    public float maxDistanceVolumetric = 64f;
    public bool analyticFogEnabled = false;
    public float maxDistanceAnalytic = 5000f;
    public bool temporalAntiAliasingEnabled = false;
    public float temporalAntiAliasingIntegrationRate = 0.03f;
    public float fogDensity = 0f;
    public float anisotropy = 0.2f;
    public float lightIntensity = 0f;
    public float shadowIntensity = 0f;
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
    public float noiseTiling = 24;
    public Vector3 noiseWindSpeed = new Vector3(0, 0, 0);
    public Vector3 defaultNoiseWindSpeed = new Vector3(2, 2, -2);
    public Vector2 noiseMap = new Vector2(0, 1);
    public bool fogModeOverrideState = true;

    private bool isFogAvailable = false;  // To check if the fog component is available

    private VolumetricFog volumetricFog;

    void Start()
    {

        volume = GetComponent<Volume>();

        if (volume == null)
        {
            Debug.LogError("Volume component not found!");
            return;
        }

        // Check if 'Buto Volumetric Fog' override exists; if not, add one
        if (!volume.profile.Has<VolumetricFog>()) // Replace ButoVolumetricFog with the actual class name
        {
            volumetricFog = volume.profile.Add<VolumetricFog>(); // Replace ButoVolumetricFog with the actual class name;

            if (volume.profile.TryGet<VolumetricFog>(out volumetricFog))
            {
                // Assign your values to the VolumetricFog parameters
                volumetricFog.mode.value = fogMode;
                volumetricFog.mode.overrideState = fogModeOverrideState;
                volumetricFog.sampleCount.value = sampleCount;
                volumetricFog.animateSamplePosition.value = animateSamplePosition;
                volumetricFog.selfShadowingEnabled.value = selfShadowingEnabled;
                volumetricFog.maximumSelfShadowingOctaves.value = maximumSelfShadowingOctaves;
                volumetricFog.horizonShadowingEnabled.value = horizonShadowingEnabled;
                volumetricFog.maxDistanceVolumetric.value = maxDistanceVolumetric;
                volumetricFog.analyticFogEnabled.value = analyticFogEnabled;
                volumetricFog.maxDistanceAnalytic.value = maxDistanceAnalytic;
                volumetricFog.temporalAntiAliasingEnabled.value = temporalAntiAliasingEnabled;
                volumetricFog.temporalAntiAliasingIntegrationRate.value = temporalAntiAliasingIntegrationRate;
                volumetricFog.fogDensity.value = fogDensity;
                volumetricFog.anisotropy.value = anisotropy;
                volumetricFog.lightIntensity.value = lightIntensity;
                volumetricFog.shadowIntensity.value = shadowIntensity;
                volumetricFog.baseHeight.value = baseHeight;
                volumetricFog.attenuationBoundarySize.value = attenuationBoundarySize;
                volumetricFog.litColor.value = litColor;
                volumetricFog.shadowedColor.value = shadowedColor;
                volumetricFog.emitColor.value = emitColor;
                volumetricFog.colorRamp.value = colorRamp;
                volumetricFog.colorInfluence.value = colorInfluence;
                // volumetricFog.octaves.value = octaves;
                // volumetricFog.lacunarity.value = lacunarity;
                // volumetricFog.gain.value = gain;
                volumetricFog.noiseTiling.value = noiseTiling;
                volumetricFog.noiseWindSpeed.value = noiseWindSpeed;
                volumetricFog.noiseMap.value = noiseMap;

                isFogAvailable = true;
            }
        }

    }

    void Update()
    {
        if (isFogAvailable)
        {
            UpdateFogParameters();
        }
    }

    void UpdateFogParameters()
    {
        if (fog == null) return;

        // Conditionally update each parameter only if it has changed.
        if (fog.mode.value != fogMode) fog.mode.value = fogMode;
        if (fog.sampleCount.value != sampleCount) fog.sampleCount.value = sampleCount;
        if (fog.animateSamplePosition.value != animateSamplePosition) fog.animateSamplePosition.value = animateSamplePosition;
        if (fog.selfShadowingEnabled.value != selfShadowingEnabled) fog.selfShadowingEnabled.value = selfShadowingEnabled;
        if (fog.maximumSelfShadowingOctaves.value != maximumSelfShadowingOctaves) fog.maximumSelfShadowingOctaves.value = maximumSelfShadowingOctaves;
        if (fog.horizonShadowingEnabled.value != horizonShadowingEnabled) fog.horizonShadowingEnabled.value = horizonShadowingEnabled;
        if (fog.maxDistanceVolumetric.value != maxDistanceVolumetric) fog.maxDistanceVolumetric.value = maxDistanceVolumetric;
        if (fog.analyticFogEnabled.value != analyticFogEnabled) fog.analyticFogEnabled.value = analyticFogEnabled;
        if (fog.maxDistanceAnalytic.value != maxDistanceAnalytic) fog.maxDistanceAnalytic.value = maxDistanceAnalytic;
        if (fog.temporalAntiAliasingEnabled.value != temporalAntiAliasingEnabled) fog.temporalAntiAliasingEnabled.value = temporalAntiAliasingEnabled;
        if (fog.temporalAntiAliasingIntegrationRate.value != temporalAntiAliasingIntegrationRate) fog.temporalAntiAliasingIntegrationRate.value = temporalAntiAliasingIntegrationRate;
        if (fog.fogDensity.value != fogDensity) fog.fogDensity.value = fogDensity;
        if (fog.anisotropy.value != anisotropy) fog.anisotropy.value = anisotropy;
        if (fog.lightIntensity.value != lightIntensity) fog.lightIntensity.value = lightIntensity;
        if (fog.shadowIntensity.value != shadowIntensity) fog.shadowIntensity.value = shadowIntensity;
        if (fog.baseHeight.value != baseHeight) fog.baseHeight.value = baseHeight;
        if (fog.attenuationBoundarySize.value != attenuationBoundarySize) fog.attenuationBoundarySize.value = attenuationBoundarySize;
        if (fog.litColor.value != litColor) fog.litColor.value = litColor;
        if (fog.shadowedColor.value != shadowedColor) fog.shadowedColor.value = shadowedColor;
        if (fog.emitColor.value != emitColor) fog.emitColor.value = emitColor;
        if (fog.colorRamp.value != colorRamp) fog.colorRamp.value = colorRamp;
        if (fog.colorInfluence.value != colorInfluence) fog.colorInfluence.value = colorInfluence;
       // if (fog.octaves.value != octaves) fog.octaves.value = octaves;
       // if (fog.lacunarity.value != lacunarity) fog.lacunarity.value = lacunarity;
       // if (fog.gain.value != gain) fog.gain.value = gain;
        if (fog.noiseTiling.value != noiseTiling) fog.noiseTiling.value = noiseTiling;
        if (fog.noiseWindSpeed.value != noiseWindSpeed) fog.noiseWindSpeed.value = noiseWindSpeed;
        if (fog.noiseMap.value != noiseMap) fog.noiseMap.value = noiseMap;
    }

    public IEnumerator LerpFogDensity(float lerpDuration, float normalizedDensityTarget)
    {
        float timeElapsed = 0;

        // Clamp the target density and map it to the 0-20 range
        float targetDensity = Mathf.Clamp(normalizedDensityTarget, 0, 1) * 20;

        float startDensity = fog.fogDensity.value;
        float startMaxDistance = fog.maxDistanceVolumetric.value;

        // Compute the target maxDistanceVolumetric value based on the targetDensity
        float targetMaxDistance = Mathf.Clamp(normalizedDensityTarget, 0, 1) * 20; // or whatever your desired formula is

        while (timeElapsed < lerpDuration)
        {
            float lerpFactor = timeElapsed / lerpDuration;

            // Lerp both the fog density and the max distance
            fog.fogDensity.value = Mathf.Lerp(startDensity, targetDensity, lerpFactor);
            fog.maxDistanceVolumetric.value = Mathf.Lerp(startMaxDistance, targetMaxDistance, lerpFactor);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the target values are set when the lerp is done
        fog.fogDensity.value = targetDensity;
        fog.maxDistanceVolumetric.value = targetMaxDistance;
    }

    public IEnumerator LerpFogColor(float lerpDuration, Color targetEmitColor, Color targetShadowedColor, float targetColorInfluence)
    {
        float timeElapsed = 0;

        Color startEmitColor = fog.emitColor.value;
        Color startShadowedColor = fog.shadowedColor.value;

        float startInfluence = fog.colorInfluence.value;
        float targetInfluence = targetColorInfluence;

        while (timeElapsed < lerpDuration)
        {
            float lerpFactor = timeElapsed / lerpDuration;

            fog.emitColor.value = Color.Lerp(startEmitColor, targetEmitColor, lerpFactor);
            fog.shadowedColor.value = Color.Lerp(startShadowedColor, targetShadowedColor, lerpFactor);
            fog.colorInfluence.value = Mathf.Lerp(startInfluence, targetInfluence, lerpFactor);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the target colors and influence are set when the lerp is done
        fog.emitColor.value = targetEmitColor;
        fog.shadowedColor.value = targetShadowedColor;
        fog.colorInfluence.value = targetInfluence;
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
}

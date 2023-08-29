using System.Collections;
using UnityEngine;
using WaterSystem.Data;

public class WaterWaveControl : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private WaterSurfaceData waterSettings;

    [SerializeField] private float newMinAmplitude = 2f;
    [SerializeField] private float newMaxAmplitude = 50f;

    [SerializeField] private float newMinWaveLength = 2f;
    [SerializeField] private float newMaxWaveLength = 50f;

    [SerializeField] private int newMinWaveCount = 1;
    [SerializeField] private int newMaxWaveCount = 10;

    [SerializeField] private Camera cam;

    private Vector3 lastCamForward;
    private float lastCamRelativeDir;

    Renderer waterRenderer;

    void Awake()
    {
        waterRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        float currentCamRelativeDir = CameraRelativeDirection();
        if (currentCamRelativeDir != lastCamRelativeDir)
        {
            lastCamRelativeDir = currentCamRelativeDir;
            UpdateWaterSettings(waterSettings._basicWaveSettings.amplitude, waterSettings._basicWaveSettings.wavelength, waterSettings._basicWaveSettings.numWaves);
        }
    }

    public float CameraRelativeDirection()
    {
        Vector3 camFwd = cam.transform.forward;
        camFwd.y = 0f;
        camFwd.Normalize();

        if (camFwd != lastCamForward)
        {
            lastCamForward = camFwd;

            float dot = Vector3.Dot(-Vector3.forward, camFwd);
            float degrees = Mathf.LerpUnclamped(90.0f, 180.0f, dot);
            if (camFwd.x < 0)
                degrees *= -1f;

            lastCamRelativeDir = Mathf.RoundToInt(degrees * 1000) / 1000;
        }

        return lastCamRelativeDir;

    }

    private void OnEnable()
    {
        player.OnFaithChanged += KarmaModifier;
    }

    private void OnDisable()
    {
        player.OnFaithChanged -= KarmaModifier;
    }

    private void KarmaModifier(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);

        float newAmplitude = Mathf.Lerp(newMaxAmplitude, newMinAmplitude, t);
        float newWavelength = Mathf.Lerp(newMaxWaveLength, newMinWaveLength, t);
        int newWaveCount = Mathf.RoundToInt(Mathf.Lerp(newMaxWaveCount, newMinWaveCount, t));

        UpdateWaterSettings(newAmplitude, newWavelength, newWaveCount);
    }

    private void UpdateWaterSettings(float amplitude, float wavelength, int numWaves)
    {
        waterSettings._basicWaveSettings.amplitude = amplitude;
        waterSettings._basicWaveSettings.wavelength = wavelength;
        waterSettings._basicWaveSettings.numWaves = numWaves;
    }
}
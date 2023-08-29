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
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        UpdateWaterSettings(waterSettings._basicWaveSettings.amplitude, waterSettings._basicWaveSettings.wavelength, waterSettings._basicWaveSettings.numWaves);
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
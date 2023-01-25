using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WaterSystem.Data;

public class WaterWaveControl : MonoBehaviour
{

    [SerializeField] private CharacterClass player;
    [SerializeField] private WaterSurfaceData waterSettings;


    [SerializeField] private float newMin = 2f;
    [SerializeField] private float newMax = 50f;
    [SerializeField] private float output;

    [SerializeField] private float waveLength;

    void Start()
    {
        waterSettings = transform.GetComponent<WaterSurfaceData>();
        waveLength = waterSettings._basicWaveSettings.wavelength;
    }

    private void Update()
    {
        waveLength = output;
    }

    private void OnEnable()
    {
        player.OnFaithChanged += KarmaModifier;

        return;
    }

    private void OnDisable()
    {
        player.OnFaithChanged -= KarmaModifier;

        return;
    }

    private void KarmaModifier(float karma, float minKarma, float maxKarma)
    {
        var t = Mathf.InverseLerp(minKarma, maxKarma, karma);
        output = Mathf.Lerp(newMax, newMin, t);
    }


}

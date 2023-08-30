using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CloudControl : MonoBehaviour
{
    private float targetCloudPower = 1f;
    private float targetCloudSpeed = 1f;

    public Player player;
    public CharacterBehaviours behaviours;

    public bool CorruptionModifierActive = false;

    [SerializeField] private WeatherControl weatherControl;
    [SerializeField] private CamControl camControl;

    [SerializeField] private Material[] sharedMaterials;

    [SerializeField] private RainControl rain;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        sharedMaterials = transform.GetComponentInChildren<Renderer>().sharedMaterials;
    }

    void Start()
    {
        CorruptionModifierActive = true;

        foreach (Material m in sharedMaterials)
        {
            m.SetFloat("_CloudsPower", cloudPersistanceMin);
            m.SetFloat("_WindSpeed", cloudSpeedMin);
        }
    }

    private void OnEnable()
    {
        player.OnFaithChanged += CloudModifier;
    }

    private void OnDisable()
    {
        player.OnFaithChanged -= CloudModifier;
    }

    float time = 0;

    public float cloudPersistanceMin = 1f;
    public float cloudPersistanceMax = 1.6f;

    public float cloudSpeedMin = 25f;
    public float cloudSpeedMax = 250f;

    private bool cloudPowerOverride = false;
    private float overrideCloudPower = 0f;

    public float lerpSpeed = 1f;

    private void CloudModifier(float faith, float minKarma, float maxKarma)
    {
        var t = Mathf.InverseLerp(minKarma, maxKarma, faith);

        float cloudPowerOutput = 0;

        if (cloudPowerOverride || manualOverrideActive)
        {
            cloudPowerOutput = Mathf.Lerp(targetCloudPower, Mathf.Lerp(targetCloudPower, overrideCloudPower, t), Time.deltaTime * lerpSpeed); ;
        }
        else
        {
            // smoothly transition to the new value over time
            cloudPowerOutput = Mathf.Lerp(targetCloudPower, Mathf.Lerp(cloudPersistanceMax, cloudPersistanceMin, t), Time.deltaTime * lerpSpeed);
        }

        float cloudSpeedOutput = Mathf.Lerp(1, 0, t);

        targetCloudPower = cloudPowerOutput;
        targetCloudSpeed = weatherControl.windStrength;
    }

    private float modifier = 0f;

    private void LateUpdate()
    {
        UpdateClouds();
    }

    private void UpdateClouds()
    {
        foreach (Material m in sharedMaterials)
        {
            m.SetFloat("_CloudPersistance", targetCloudPower);
            m.SetFloat("_WindSpeed", targetCloudSpeed);
        }
    }

    public bool manualOverrideActive = false;

    public void ManualCloudOverrideStart()
    {
        manualOverrideActive = true;
        OverrideCloudPower(cloudPersistanceMin);
    }

    public void ManualCloudOverrideStop()
    {
        manualOverrideActive = false;
    }

    public void OverrideCloudPower(float value)
    {
        if (!manualOverrideActive)
        {
            cloudPowerOverride = true;
            overrideCloudPower = value;
        }
    }

    public void StopCloudPowerOverride()
    {
        if (!manualOverrideActive)
        {
            cloudPowerOverride = false;
        }
    }
}

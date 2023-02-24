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

    [SerializeField] private Material[] sharedMaterials;

    private void Awake()
    {
        sharedMaterials = transform.GetComponentInChildren<Renderer>().sharedMaterials;
    }

    void Start()
    {
        CorruptionModifierActive = true;

        foreach (Material m in sharedMaterials)
        {

            m.SetFloat("_CloudsPower", cloudPowerMin);
            m.SetFloat("_WindSpeed", cloudSpeedMin);

        }
        

        //behaviours = player.GetComponentInChildren<CharacterBehaviours>();
    }

    private void OnEnable() => player.OnFaithChanged += CloudModifier;
    private void OnDisable() => player.OnFaithChanged -= CloudModifier;

    float time = 0;

    public float cloudPowerMin = 1f;
    public float cloudPowerMax = 1.6f;

    public float cloudSpeedMin = 25f;
    public float cloudSpeedMax = 250;

    private void CloudModifier(float karma, float minKarma, float maxKarma)
    {
        var t = Mathf.InverseLerp(minKarma, maxKarma, karma);
        float cloudPowerOutput = Mathf.Lerp(cloudPowerMax, cloudPowerMin, t);
        float cloudSpeedOutput = Mathf.Lerp(1, 0, t);

        targetCloudPower = cloudPowerOutput;
        targetCloudSpeed = cloudSpeedOutput *= weatherControl.windStrength * 20 + 0.1f;
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
}

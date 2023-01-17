using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraParticleControl : MonoBehaviour
{ 

    [SerializeField] private Player player;

    [SerializeField] private ParticleSystem auraParticles;

    [SerializeField] AreaManager areaManager;

    private ParticleSystem.EmissionModule emission;

    ParticleSystem.MinMaxCurve gravityModifier;

    float time = 0;

    [SerializeField] private float auraDensityMin = 0;
    [SerializeField] private float auraDensityMax = 128;

    [SerializeField] private float auraGravityMin = 0;
    [SerializeField] private float auraGravityMax = -1;

    float auraDensityOutput;
    float auraGravityOutput;

    ParticleSystem.MinMaxCurve rate;

    void Start()
    {
        emission = auraParticles.emission;
        gravityModifier = auraParticles.main.gravityModifier.constant;

        rate = emission.rateOverTime;

        emission.enabled = false;

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

    public bool active = false;

    public void StartParticles()
    {
        emission.enabled = true;
    }

    public void StopParticles()
    {
        emission.enabled = false;
    }

    private void Update()
    {
        rate.constant = auraDensityOutput;
        gravityModifier = auraGravityOutput;
    }

    private void KarmaModifier(float karma, float minKarma, float maxKarma)
    {
        var t = Mathf.InverseLerp(minKarma, maxKarma, karma);
        auraDensityOutput = Mathf.Lerp(auraDensityMin, auraDensityMax, t);
        auraGravityOutput = Mathf.Lerp(auraGravityMin, auraGravityMax, t);
    }

}

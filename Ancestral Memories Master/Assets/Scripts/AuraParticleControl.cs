using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class AuraParticleControl : MonoBehaviour
{ 

    [SerializeField] private Player player;

    [SerializeField] private ParticleSystem auraParticles;
    [SerializeField] AreaManager areaManager;

    private EmissionModule emission;

    [SerializeField] private float rateOverTime;
    [SerializeField] private float gravityModifier;
    [SerializeField] private float simulationSpeed;
    [SerializeField] private float noiseStrength;
    [SerializeField] private float vertexDistance;

    float time = 0;


    [SerializeField] private float auraDensityMin = 0f;
    [SerializeField] private float auraDensityMax = 128f;

    [SerializeField] private float auraGravityMin = -0.4f;
    [SerializeField] private float auraGravityMax = 0.1f;

    [SerializeField] private float auraSpeedMin = 0.25f;
    [SerializeField] private float auraSpeedMax = 50f;

    [SerializeField] private float auraNoiseStrengthMin = 0f;
    [SerializeField] private float auraNoiseStrengthMax = 25f;

    [SerializeField] private float vertexDistanceMin = 0f;
    [SerializeField] private float vertexDistanceMax = 5f;

    float auraDensityOutput;
    float auraSpeedOutput;
    float auraGravityOutput;
    float auraNoiseOutput;
    float vertexDistanceOutput;

    void Awake()
    {
        auraParticles = transform.GetComponent<ParticleSystem>();
        emission = auraParticles.emission;
       

        gravityModifier = auraParticles.main.gravityModifier.constant;
        noiseStrength = auraParticles.noise.strength.constant;
        rateOverTime = auraParticles.emission.rateOverTime.constant;
        simulationSpeed = auraParticles.main.simulationSpeed;
        vertexDistance = auraParticles.trails.minVertexDistance;
        emission.enabled = false;
     
    }

    private void OnEnable()
    {
        player.OnFaithChanged += KarmaModifier;
        player.OnPsychChanged += PsychedeliaModifier;
    }

    private void OnDisable()
    {
        player.OnFaithChanged -= KarmaModifier;
        player.OnPsychChanged -= PsychedeliaModifier;
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

    private void KarmaModifier(float karma, float minKarma, float maxKarma)
    {
        var t = Mathf.InverseLerp(minKarma, maxKarma, karma);
        auraDensityOutput = Mathf.Lerp(auraDensityMin, auraDensityMax, t);
        auraGravityOutput = Mathf.Lerp(auraGravityMax, auraGravityMin, t);
        auraSpeedOutput = Mathf.Lerp(auraSpeedMin, auraSpeedMax, t);
        auraNoiseOutput = Mathf.Lerp(auraNoiseStrengthMax, auraNoiseStrengthMin, t);

        gravityModifier = auraGravityOutput;
        noiseStrength = auraNoiseOutput;
        rateOverTime = auraDensityOutput;
        simulationSpeed = auraSpeedOutput;
        vertexDistance = vertexDistanceOutput;
    }

    private void PsychedeliaModifier(float psychedelia, float minPsychedelia, float maxPsychedelia)
    {
        var t = Mathf.InverseLerp(minPsychedelia, maxPsychedelia, psychedelia);
        vertexDistanceOutput = Mathf.Lerp(vertexDistanceMin, vertexDistanceMax, t);
    }

}

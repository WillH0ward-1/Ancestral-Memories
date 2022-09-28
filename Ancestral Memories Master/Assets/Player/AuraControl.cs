using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraControl : Faith
{

    [SerializeField] private Material auraMaterial;

    private int maxAura = 1;
    private int minAura = 0;

    [SerializeField] private Renderer[] auraRenderers = new Renderer[0];

    private float targetAuraVal = 1f;
    private float currentAuraVal = 1f;

    public float auraIntensity;

    private void OnEnable()
    {
        OnFaithChanged += FaithChanged;
    }

    private void OnDisable()
    {
        OnFaithChanged -= FaithChanged;
    }

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;

        auraIntensity = auraMaterial.GetFloat("_AuraIntensity");
        auraIntensity = maxAura;

    }

    // Update is called once per frame
    void Update()
    {
        currentAuraVal = Mathf.Lerp(currentAuraVal, targetAuraVal, 2f * Time.deltaTime);

        foreach (Renderer renderer in auraRenderers)
        {
            renderer.material.SetFloat("_AuraIntensity", currentAuraVal);
        }

    }

    private void FaithChanged(int faith, int maxFaith)
    {
        targetAuraVal = (float)faith / maxFaith;
    }
}

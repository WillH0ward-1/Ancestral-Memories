using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraControl : MonoBehaviour
{

    public Material auraMaterial;

    [SerializeField]
    private CharacterClass player;

    public int maxAura = 1;
    public int minAura = 0;

    public Renderer[] auraRenderers = new Renderer[0];

    private float targetAuraVal = 1f;
    private float currentAuraVal = 1f;

    public float auraIntensity;

    private void OnEnable() => player.OnFaithChanged += FaithChanged;
    private void OnDisable() => player.OnFaithChanged -= FaithChanged;

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
            renderer.sharedMaterial.SetFloat("_AuraIntensity", currentAuraVal);
        }

    }

    private void FaithChanged(int faith, int maxFaith)
    {
        targetAuraVal = (float)faith / maxFaith;
    }
}

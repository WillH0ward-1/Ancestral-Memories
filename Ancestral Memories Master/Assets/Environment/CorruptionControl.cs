using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionControl : MonoBehaviour
{

    private Material corruptionMaterial;

    private CharacterClass player;

    public int maxKarma = 1;
    public int minKarma = 0;

    public Renderer[] auraRenderers = new Renderer[0];

    private float targetKarma = 1f;
    private float currentKarmaVal = 1f;

    public float corruptionIntensity;

    private void OnEnable() => player.OnFaithChanged += KarmaModify;
    private void OnDisable() => player.OnFaithChanged -= KarmaModify;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        Material corruptionMaterial = GetComponent<Renderer>().material;

        corruptionIntensity = corruptionMaterial.GetFloat("_Corruption");
        corruptionIntensity = maxKarma;

    }

    // Update is called once per frame
    void Update()
    {
        currentKarmaVal = Mathf.Lerp(currentKarmaVal, targetKarma, 2f * Time.deltaTime);

        foreach (Renderer renderer in auraRenderers)
        {
            renderer.material.SetFloat("_AuraIntensity", currentKarmaVal);
        }

    }

    private void KarmaModify(int faith, int maxFaith)
    {
        targetKarma = (float)faith / maxFaith;
    }
}

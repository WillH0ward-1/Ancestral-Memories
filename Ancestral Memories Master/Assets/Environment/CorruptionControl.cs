using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CorruptionControl : MonoBehaviour
{

    private Material corruptionMaterial;


    Renderer meshRenderer;

    private float targetCorruption = 1f;
    private float currentCorruption = 0f;


    private float targetAlpha = 0f;
    private float currentAlpha = 1f;

    public Player player;

    // Start is called before the first frame update

    void Start()
    {
        GetRenderers();
    }

    [SerializeField] List<Renderer> rendererList = new List<Renderer>();

    void GetRenderers()
    {
        Renderer[] objectRenderers = transform.GetComponentsInChildren<Renderer>();
        rendererList = objectRenderers.ToList();
    }

    private void OnEnable()
    {
        player.OnFaithChanged += Corruption;
        player.OnFaithChanged += Alpha;
    }

    private void OnDisable()
    {
        player.OnFaithChanged -= Corruption;
        player.OnFaithChanged -= Alpha;
    }

    float time = 0;

    // Update is called once per frame
    void Update()
    {
        currentCorruption = Mathf.Lerp(currentCorruption, targetCorruption, 2f * Time.deltaTime);
    }

    float newMin = 0;
    float newMax = 1;

    float newCorruptionMin = 1;
    float newCorruptionMax = 0;

    private void Corruption(float corruption, float minCorruption, float maxCorruption)
    {
        targetCorruption = corruption / maxCorruption;

        var t = Mathf.InverseLerp(minCorruption, maxCorruption, corruption);
        float output = Mathf.Lerp(newCorruptionMin, newCorruptionMax, t);

        targetCorruption = output;

        foreach (Renderer r in rendererList)
        {
            r.material.SetFloat("_Corruption", currentCorruption);
        }

    }

    private void Alpha(float alpha, float minAlpha, float maxAlpha)
    {
        targetAlpha = alpha / maxAlpha;

        var t = Mathf.InverseLerp(minAlpha, maxAlpha, alpha);
        float output = Mathf.Lerp(newMin, newMax, t);

        targetAlpha = output;

        foreach (Renderer r in rendererList)
        {
    
            r.material.SetFloat("_Alpha", currentAlpha);
        }
    }
}

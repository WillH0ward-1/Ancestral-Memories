using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionControl : MonoBehaviour
{

    private Material corruptionMaterial;

    string playerTag = "Player";

    Renderer meshRenderer;

    private float targetCorruption = 1f;
    private float currentCorruption = 0f;


    private float targetAlpha = 0f;
    private float currentAlpha = 1f;

   public Player player;

    // Start is called before the first frame update

    void Start()
    {

        meshRenderer = transform.GetComponent<Renderer>();

    }

    private void OnEnable()
    {
        player.OnFaithChanged -= Corruption;
        player.OnFaithChanged += Alpha;
    }

    private void OnDisable()
    {
        player.OnFaithChanged += Corruption;
        player.OnFaithChanged -= Alpha;
    }

    float time = 0;

    // Update is called once per frame
    void Update()
    {
        currentCorruption = targetCorruption;
        currentAlpha = targetAlpha;

        meshRenderer.sharedMaterial.SetFloat("_Corruption", currentCorruption);
        meshRenderer.sharedMaterial.SetFloat("_Alpha", currentAlpha);

    }

    private void Corruption(float corruption, float minCorruption, float maxCorruption)
    {
        targetCorruption = corruption / maxCorruption;
    }

    private void Alpha(float alpha, float minAlpha, float maxAlpha)
    {
        targetAlpha = alpha / maxAlpha;
    }
}

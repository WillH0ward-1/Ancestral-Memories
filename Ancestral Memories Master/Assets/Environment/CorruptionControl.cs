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

    private float corruptionMultiplier = 1f;

    [SerializeField] private Player characterClass;

    // Start is called before the first frame update

    void Start()
    {

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        characterClass = player.GetComponent<Player>();

        meshRenderer = GetComponent<Renderer>();

    }

    private void OnEnable()
    {
        characterClass.OnFaithChanged -= Corruption;
        characterClass.OnFaithChanged += Alpha;
    }

    private void OnDisable()
    {
        characterClass.OnFaithChanged += Corruption;
        characterClass.OnFaithChanged -= Alpha;
    }

    // Update is called once per frame
    void Update()
    {
        currentCorruption = Mathf.Lerp(currentCorruption, targetCorruption, corruptionMultiplier * Time.deltaTime);
        currentAlpha = Mathf.Lerp(currentAlpha, targetAlpha, corruptionMultiplier * Time.deltaTime);

        meshRenderer.sharedMaterial.SetFloat("_Corruption", currentCorruption);
        meshRenderer.sharedMaterial.SetFloat("_Alpha", currentCorruption);
    }

    private void Corruption(int corruption, int maxCorruption)
    {
        targetCorruption = (float)corruption / maxCorruption;
    }

    private void Alpha(int alpha, int maxAlpha)
    {
        targetAlpha = (float)alpha / maxAlpha;
    }
}

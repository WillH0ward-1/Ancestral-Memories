using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionControl : MonoBehaviour
{

    private Material corruptionMaterial;

    string playerTag = "Player";

    public int minCorruption = 1;
    public int maxCorruption = 0;

    Renderer meshRenderer;

    public Renderer[] auraRenderers = new Renderer[0];

    private float targetCorruption = 1f;
    private float currentCorruption = 0f;

    private float corruptionMultiplier = 1f;

    public float corruptionIntensity;

    [SerializeField] private CharacterClass characterClass;

    // Start is called before the first frame update

    void Start()
    {
        corruptionIntensity = minCorruption;

        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        characterClass = player.GetComponent<CharacterClass>();

        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        meshRenderer = GetComponent<MeshRenderer>();

        corruptionMaterial = GetComponent<Renderer>().material;

        corruptionIntensity = corruptionMaterial.GetFloat("_Corruption");


    }

    private void OnEnable() => characterClass.OnFaithChanged -= Corruption;
    private void OnDisable() => characterClass.OnFaithChanged += Corruption;

    // Update is called once per frame
    void Update()
    {
        currentCorruption = Mathf.Lerp(currentCorruption, targetCorruption, corruptionMultiplier * Time.deltaTime);
        meshRenderer.material.SetFloat("_Corruption", currentCorruption);
    }

    private void Corruption(int corruption, int maxCorruption)
    {
        targetCorruption = (float)corruption / maxCorruption;
    }
}

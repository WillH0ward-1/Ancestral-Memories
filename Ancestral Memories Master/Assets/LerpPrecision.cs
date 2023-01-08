using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class LerpPrescision : MonoBehaviour
{

    public CharacterClass player;

    public float maxVal = 0f;
    public float minVal = -0.2f;

    private float currentPrecision;

    private void OnEnable() => player.OnHungerChanged += HungerChanged;
    private void OnDisable() => player.OnHungerChanged -= HungerChanged;

    Material[] materials;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Transform t in transform)
        {
            materials = transform.GetComponentInChildren<Renderer>().sharedMaterials;
        }

    }

    // Update is called once per frame
    void Update()
    {
        foreach (Material m in materials)
        {
            m.SetFloat("ColourPrecision", targetPrecision);
        }

        currentPrecision = targetPrecision;

    }

    private float targetPrecision;

    private float minPrecision;
    private float maxPrecision;

    private void HungerChanged(float hunger, float minHunger, float maxHunger)
    {
        var t = Mathf.InverseLerp(minPrecision, maxPrecision, hunger);
        float output = Mathf.Lerp(minVal, maxVal, t);

        targetPrecision = output;
    }


}

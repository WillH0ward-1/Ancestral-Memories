using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class LerpDeformation : MonoBehaviour
{

    public CharacterClass player;

    public float maxVal = 0f;
    public float minVal = -0.2f;

    private float currentDeform;

    [SerializeField] private Deform.InflateDeformer inflate;

    private void OnEnable()
    {
        player.OnHungerChanged += HungerChanged;
    }

    private void OnDisable()
    {
        player.OnHungerChanged -= HungerChanged;
    }

    // Start is called before the first frame update
    void Awake()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;

        inflate = transform.GetComponentInChildren<Deform.InflateDeformer>();

        inflate.Factor = targetDeform;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        currentDeform = targetDeform;
        inflate.Factor = currentDeform;  
    }

    private float targetDeform;

    private void HungerChanged(float hunger, float minHunger, float maxHunger)
    {
        var t = Mathf.InverseLerp(minHunger, maxHunger, hunger);
        float output = Mathf.Lerp(minVal, maxVal, t);

        targetDeform = output;
    }


}

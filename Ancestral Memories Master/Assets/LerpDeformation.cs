using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class LerpDeformation : MonoBehaviour
{

    [SerializeField]
    private CharacterClass player;

    public int maxVal = 0;
    public float minVal = -0.2f;

    private float targetDeform = 1f;
    private float currentDeform = 1f;

    [SerializeField] Deform.InflateDeformer inflate;

    private void OnEnable() => player.OnHungerChanged += HungerChanged;
    private void OnDisable() => player.OnHungerChanged -= HungerChanged;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;

        inflate = transform.GetComponentInChildren<Deform.InflateDeformer>();

        inflate.Factor = maxVal;

    }

    // Update is called once per frame
    void Update()
    {
        currentDeform = Mathf.Lerp(currentDeform, targetDeform, 2f * Time.deltaTime);
        inflate.Factor = currentDeform;  

    }

    private void HungerChanged(int hunger, int maxHunger)
    {
        float x = 1;
        float t = Mathf.InverseLerp(hunger, maxHunger, x);
        float output = Mathf.Lerp(minVal, maxVal, t);

        targetDeform = (float)output / maxHunger;
    }


}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpDeformation : MonoBehaviour
{

    [SerializeField]
    private CharacterClass player;

    public int maxVal = 1;
    public float minVal = -0.2f;

    private float targetDeform = 1f;
    private float currentDeform = 1f;

    float inflationFactor;

    private void OnEnable() => player.OnHungerChanged += HungerChanged;
    private void OnDisable() => player.OnHungerChanged -= HungerChanged;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;

        Deform.InflateDeformer inflate = transform.GetComponentInChildren<Deform.InflateDeformer>();
        inflationFactor = inflate.Factor;

        inflationFactor = maxVal;

    }

    // Update is called once per frame
    void Update()
    {
        currentDeform = Mathf.Lerp(currentDeform, targetDeform, 2f * Time.deltaTime);
        inflationFactor = currentDeform;  

    }

    private void HungerChanged(int hunger, int maxHunger)
    {
        targetDeform = (float)hunger / maxHunger;
    }


}

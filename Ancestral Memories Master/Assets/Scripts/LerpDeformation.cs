using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class LerpDeformation : MonoBehaviour
{

    public Player player;

    public float maxVal = 0f;
    public float minVal = -0.02f;

    private float currentDeform;

    [SerializeField] private Deform.InflateDeformer inflate;
    [SerializeField] private Deform.InflateDeformer[] inflateDeformers;

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
        player = FindObjectOfType<Player>();

        inflateDeformers = transform.GetComponentsInChildren<Deform.InflateDeformer>();

        inflate = transform.GetComponentInChildren<Deform.InflateDeformer>();

        inflate.Factor = targetDeform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        currentDeform = targetDeform;
        foreach (Deform.InflateDeformer deformer in inflateDeformers)
        {
            if (deformer.gameObject.CompareTag("Aura"))
            {
                deformer.Factor = currentDeform += auraDeformationOffset;
            }
            else
            {
                deformer.Factor = currentDeform;
            }
        }
    }

    private float targetDeform;

    private float auraDeformationOffset = 0.00006f;

    private void HungerChanged(float hunger, float minHunger, float maxHunger)
    {
        var t = Mathf.InverseLerp(minHunger, maxHunger, hunger);
        float output = Mathf.Lerp(minVal, maxVal, t);

        targetDeform = output;
    }



}

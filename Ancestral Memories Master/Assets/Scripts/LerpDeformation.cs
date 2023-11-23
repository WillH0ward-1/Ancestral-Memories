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
    private float targetDeform;

    [SerializeField] private List<Deform.InflateDeformer> inflateDeformers;
    [SerializeField] private List<Deform.InflateDeformer> nonAuraDeformers;
    [SerializeField] private List<Deform.InflateDeformer> auraDeformers;
   
    private float auraDeformationOffset = 0.00006f;

    public void SubscribeToHunger()
    {
        if (player != null)
        {
            player.OnHungerChanged += HungerChanged;
        }
    }

    private void OnDisable()
    {
        if (player != null)
        {
            player.OnHungerChanged -= HungerChanged;
        }
    }

    private void Awake()
    {
        InitDeformers();
    }

    void InitDeformers()
    {
        inflateDeformers = new List<Deform.InflateDeformer>(transform.GetComponentsInChildren<Deform.InflateDeformer>());
        auraDeformers = new List<Deform.InflateDeformer>();

        foreach (Deform.InflateDeformer deformer in inflateDeformers)
        {
            deformer.update = true;
            Deform.Deformable deformable = deformer.transform.GetComponent<Deform.Deformable>();

            deformable.UpdateMode = Deform.UpdateMode.Auto;
            deformable.CullingMode = Deform.CullingMode.AlwaysUpdate;
            deformable.NormalsRecalculation = Deform.NormalsRecalculation.None;
            deformable.BoundsRecalculation = Deform.BoundsRecalculation.Never;
            deformable.ColliderRecalculation = Deform.ColliderRecalculation.None;

            if (deformer.gameObject.CompareTag("Aura"))
            {
                auraDeformers.Add(deformer);
            } else
            {
                nonAuraDeformers.Add(deformer);
            }
        }
    }

    private void HungerChanged(float hunger, float minHunger, float maxHunger)
    {
        var t = Mathf.InverseLerp(minHunger, maxHunger, hunger);
        float output = Mathf.Lerp(minVal, maxVal, t);

        targetDeform = output;

        currentDeform = targetDeform;

        // Apply deformation to all inflateDeformers
        foreach (Deform.InflateDeformer deformer in nonAuraDeformers)
        {
            deformer.Factor = currentDeform;
        }

        // Apply additional deformation for auraDeformers
        foreach (Deform.InflateDeformer auraDeformer in auraDeformers)
        {
            auraDeformer.Factor = currentDeform + auraDeformationOffset;
        }
    }
}

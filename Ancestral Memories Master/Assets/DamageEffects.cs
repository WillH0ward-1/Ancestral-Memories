using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DamageEffects : MonoBehaviour
{
    public Material unlitMaterial; // The Unlit material to be switched to
    private List<Material> litMaterials = new List<Material>(); // The original materials
    public float flashDuration = 0.1f; // Duration of the flash effect

    private List<Renderer> renderers = new List<Renderer>();

    private void Awake()
    {
        // Get all renderers
        renderers.AddRange(GetComponentsInChildren<Renderer>());

        // Store the original materials
        foreach (Renderer renderer in renderers)
        {
            litMaterials.Add(renderer.material);
        }
    }

    public void FlashRed()
    {
        StartCoroutine(FlashRedCoroutine());
    }

    private IEnumerator FlashRedCoroutine()
    {
        // Switch to the unlit material
        foreach (Renderer renderer in renderers)
        {
            renderer.material = unlitMaterial;
        }

        // Wait for the flash duration
        yield return new WaitForSeconds(flashDuration);

        // Switch back to the original materials
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = litMaterials[i];
        }
    }
}

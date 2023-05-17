using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SkinToneControl : MonoBehaviour
{
    float skinToneValue = 0f;
    [SerializeField] private Renderer[] renderers;

    private void Start()
    {
        // Generate a random skin tone value between 0 and 1
        skinToneValue = Random.Range(0f, 1f);

        // Get all child renderers
        renderers = GetComponentsInChildren<Renderer>();

        // Loop through each renderer
        foreach (Renderer renderer in renderers)
        {
            // Get all materials of the renderer
            Material[] materials = renderer.materials;

            // Loop through each material
            foreach (Material material in materials)
            {
                // Set the 'SkinTone' parameter to the same random value for all materials
                material.SetFloat("_SkinTone", skinToneValue);
            }
        }
    }
}

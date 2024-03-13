using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyHairControl : MonoBehaviour
{
    private AICharacterStats stats;

    public int maxHairThickness = 1;
    public int minHairThickness = 0;

    private List<Renderer> bodyHairRenderers = new List<Renderer>(); // Use a list to dynamically add renderers

    private float targetHairThickness = 1f;
    private float currentHairThickness = 1f;

    private void Awake()
    {
        // Recursively search and add renderers with the 'BodyHair' tag and _HairThickness property
        FindRenderersWithProperty(transform, "BodyHair", "_HairThickness", bodyHairRenderers);

        stats = transform.GetComponentInChildren<AICharacterStats>();
    }

    private void FindRenderersWithProperty(Transform parent, string tag, string propertyName, List<Renderer> renderersList)
    {
        foreach (Transform child in parent)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (renderer != null && renderer.gameObject.CompareTag(tag) && renderer.sharedMaterial.HasProperty(propertyName))
            {
                renderersList.Add(renderer);
            }

            // Recursively search in children
            FindRenderersWithProperty(child, tag, propertyName, renderersList);
        }
    }

    private void OnEnable()
    {
        if (stats == null)
        {
            stats = transform.GetComponentInChildren<AICharacterStats>();
        }

        stats.OnEvolutionChanged += EvolutionChanged;
    }

    private void OnDisable()
    {
        stats.OnEvolutionChanged -= EvolutionChanged;
    }

    void Start()
    {
        targetHairThickness = maxHairThickness;
    }

    void Update()
    {
        currentHairThickness = Mathf.Lerp(currentHairThickness, targetHairThickness, 2f * Time.deltaTime);

        // Update only those renderers that have the _HairThickness property
        foreach (var renderer in bodyHairRenderers)
        {
            renderer.sharedMaterial.SetFloat("_HairThickness", currentHairThickness);
        }
    }

    private void EvolutionChanged(float evolution, float minEvolution, float maxEvolution)
    {
        targetHairThickness = Mathf.Lerp(maxHairThickness, maxHairThickness, evolution / maxEvolution);
    }
}

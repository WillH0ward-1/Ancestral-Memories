using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraControl : MonoBehaviour
{
    private AICharacterStats characterStats;

    public int maxAura = 1;
    public int minAura = 0;

    private List<Renderer> auraRenderers = new List<Renderer>(); // Use a list to dynamically add renderers

    private float targetAuraVal = 1f;
    private float currentAuraVal = 1f;

    private void Awake()
    {
        // Recursively search and add renderers with the 'Aura' tag and _AuraIntensity property
        FindRenderersWithProperty(transform, "Aura", "_AuraIntensity", auraRenderers);

        characterStats = GetComponentInParent<AICharacterStats>();
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
        if (characterStats == null)
        {
            characterStats = GetComponentInParent<AICharacterStats>();
        }

        characterStats.OnFaithChanged += FaithChanged;
    }

    private void OnDisable()
    {
        if (characterStats != null)
        {
            characterStats.OnFaithChanged -= FaithChanged;
        }
    }

    void Start()
    {
        targetAuraVal = maxAura;
    }

    void Update()
    {
        currentAuraVal = Mathf.Lerp(currentAuraVal, targetAuraVal, 2f * Time.deltaTime);

        // Update only those renderers that have the _AuraIntensity property
        foreach (var renderer in auraRenderers)
        {
            renderer.sharedMaterial.SetFloat("_AuraIntensity", currentAuraVal);
        }
    }

    private void FaithChanged(float faithFraction, float minStat, float maxStat)
    {
        // Normalize faithFraction to a range of 0 to 1
        float normalizedFaith = (faithFraction - minStat) / (maxStat - minStat);

        // Define the threshold as a percentage of maxStat
        float thresholdPercentage = 0.75f; // 75% threshold
        float thresholdValue = thresholdPercentage * maxStat;

        // Apply exponential curve, making it more effective after reaching the threshold
        // Adjust the exponent (2 in this case) to control how steep the curve is
        float exponentialEffect = Mathf.Pow(normalizedFaith, 2);

        // Check if faithFraction exceeds the threshold
        if (faithFraction > thresholdValue)
        {
            targetAuraVal = Mathf.Lerp(minAura, maxAura, exponentialEffect);
        }
        else
        {
            targetAuraVal = minAura;
        }
    }

}

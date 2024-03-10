using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CorruptionControl : MonoBehaviour
{
    [SerializeField] private float currentCorruptionVal;

    public Player player;
    public CharacterBehaviours behaviours;

    public bool CorruptionModifierActive = false;

    [SerializeField] List<Transform> transformList = new List<Transform>();

    public void InitCorruption()
    {
        if (transform.CompareTag("Player"))
        {
            player = transform.GetComponentInChildren<Player>();
            behaviours = transform.GetComponentInChildren<CharacterBehaviours>();
        }

        // Iterate through all renderers in the object and its children
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in allRenderers)
        {
            foreach (Material m in renderer.sharedMaterials)
            {
                // Ensure the material is not null before applying changes
                if (m != null)
                {
                    // Check and set _MinKarma if it exists
                    if (m.HasProperty("_MinKarma"))
                    {
                        m.SetFloat("_MinKarma", newMin);
                    }

                    // Check and set _MaxKarma if it exists
                    if (m.HasProperty("_MaxKarma"))
                    {
                        m.SetFloat("_MaxKarma", newMax);
                    }

                    // Check and set _NewMin if it exists
                    if (m.HasProperty("_NewMin"))
                    {
                        m.SetFloat("_NewMin", newMin);
                    }

                    // Check and set _NewMax if it exists
                    if (m.HasProperty("_NewMax"))
                    {
                        m.SetFloat("_NewMax", newMax);
                    }

                    // Check and set _MinWarpStrength if it exists
                    if (m.HasProperty("_MinWarpStrength"))
                    {
                        m.SetFloat("_MinWarpStrength", newMin);
                    }

                    // Check and set _MaxWarpStrength if it exists
                    if (m.HasProperty("_MaxWarpStrength"))
                    {
                        m.SetFloat("_MaxWarpStrength", newMax);
                    }
                }
            }
        }

        SubscribeToCorruption();
        //behaviours = player.GetComponentInChildren<CharacterBehaviours>();
    }


    private void SubscribeToCorruption()
    {
        if (CorruptionModifierActive && player != null)
        {
            player.OnFaithChanged += CorruptionModifier;
        }

    }

    private void OnDisable()
    {
        if (CorruptionModifierActive && player != null)
        {
            player.OnFaithChanged -= CorruptionModifier;
        }
    }
    
    float time = 0;

    public float newMin = 0;
    public float newMax = 1;

    private void CorruptionModifier(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float faithOutput = Mathf.Lerp(newMin, newMax, t);

        UpdateCorruption(faithOutput);
    }

    private float modifier = 0f;
    [SerializeField] float manualLeafLerpSpeed = 1f;

    public RainControl rain;

    [SerializeField] private bool LeafManualOverrideActive = false;

    public float overrideModifer;

    private IEnumerator LeafOverride(float modifier, float target, float duration)
    {
        LeafManualOverrideActive = true;

        float time = 0f;

        while (time < duration && LeafManualOverrideActive)
        {
            time += Time.deltaTime / duration;

            overrideModifer = Mathf.Lerp(modifier, target, time);

            yield return null;
        }

        overrideModifer = target;

        LeafManualOverrideActive = false;

        yield break;
    }

    private void UpdateCorruption(float faithOutput)
    {
        // Assuming currentCorruptionVal is meant to smoothly transition to faithOutput over time
        // Removed redundant Lerp operation since it was effectively a no-op in the original code.
        currentCorruptionVal = faithOutput;

        // Check tags and conditions once, outside the loop
        bool isTree = transform.CompareTag("Trees");
        bool isDrought = rain != null && rain.drought;
        bool isPsychedelicMode = behaviours != null && behaviours.isPsychdelicMode;

        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in allRenderers)
        {
            foreach (Material m in renderer.sharedMaterials)
            {
                if (m != null)
                {
                    // Update _Karma property
                    if (m.HasProperty("_Karma"))
                    {
                        m.SetFloat("_Karma", currentCorruptionVal);
                    }

                    // Handle Trees tag specifics
                    if (isTree)
                    {
                        if (isDrought)
                        {
                            if (!LeafManualOverrideActive)
                            {
                                StartCoroutine(LeafOverride(currentCorruptionVal, newMin, manualLeafLerpSpeed));
                            }

                            // Update _LeafDensity for drought condition
                            if (m.HasProperty("_LeafDensity"))
                            {
                                m.SetFloat("_LeafDensity", overrideModifer);
                            }
                        }
                        else
                        {
                            // Normal tree handling
                            if (m.HasProperty("_LeafDensity"))
                            {
                                m.SetFloat("_LeafDensity", currentCorruptionVal);
                            }
                        }
                    }
                    else
                    {
                        if (m.HasProperty("_LeafDensity"))
                        {
                            m.SetFloat("_LeafDensity", currentCorruptionVal);
                        }
                    }

                    // Psychedelic mode handling
                    if (isPsychedelicMode && m.HasProperty("_WarpStrength"))
                    {
                        m.SetFloat("_WarpStrength", modifier);
                    }
                }
            }
        }
    }



}

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

        //CorruptionModifierActive = false;
        /*
        if (!transform.CompareTag("Trees"))
        {
            objectRenderers = transform.GetComponentsInChildren<MeshRenderer>();
            rendererList = objectRenderers.ToList();
        } else
        {
            objectRenderer = transform.GetComponent<MeshRenderer>();
            rendererList.Add(objectRenderer);
        }
        */

        transformList.Add(transform);

        foreach (Material m in transform.GetComponentInChildren<Renderer>().sharedMaterials)
        {
            m.SetFloat("_MinKarma", newMin);
            m.SetFloat("_MaxKarma", newMax);

            m.SetFloat("_NewMin", newMin);
            m.SetFloat("_NewMax", newMax);

            m.SetFloat("_MinWarpStrength", newMin);
            m.SetFloat("_MaxWarpStrength", newMax);

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
        float currentCorruption = faithOutput;
        currentCorruptionVal = Mathf.Lerp(currentCorruption, faithOutput, 2f * Time.deltaTime);

        foreach (Material m in transform.GetComponentInChildren<Renderer>().sharedMaterials)
        {
            m.SetFloat("_Karma", currentCorruptionVal);

            if (transform.CompareTag("Trees"))
            {
                if (rain != null && rain.drought)
                {
                    if (!LeafManualOverrideActive)
                    {
                        StartCoroutine(LeafOverride(currentCorruptionVal, newMin, manualLeafLerpSpeed));
                    }

                    m.SetFloat("_LeafDensity", overrideModifer);
                } else
                {
                    m.SetFloat("_LeafDensity", currentCorruptionVal);
                }
            }
            else 
            {
                m.SetFloat("_LeafDensity", currentCorruptionVal);
            }

                
            if (behaviours.isPsychdelicMode)
            {
                m.SetFloat("_WarpStrength", modifier);
            }
        }
    }
    
}

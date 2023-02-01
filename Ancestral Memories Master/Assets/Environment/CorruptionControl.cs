using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CorruptionControl : MonoBehaviour
{
    private float targetCorruption = 1f;
    private float currentCorruption = 0f;

    public Player player;
    public CharacterBehaviours behaviours;

    public bool CorruptionModifierActive = false;

    private MeshRenderer[] objectRenderers;

    private MeshRenderer objectRenderer;
    void Awake()
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

        rendererList.Add(transform);

        foreach (Transform transform in rendererList)
        {
            foreach (Material m in transform.GetComponentInChildren<Renderer>().sharedMaterials)
            {
                
                m.SetFloat("_MinKarma", newMin);
                m.SetFloat("_MaxKarma", newMax);

                m.SetFloat("_NewMin", newMin);
                m.SetFloat("_NewMax", newMax);

                m.SetFloat("_MinWarpStrength", newMin);
                m.SetFloat("_MaxWarpStrength", newMax);

            }
        }

        //behaviours = player.GetComponentInChildren<CharacterBehaviours>();
    }

    [SerializeField] List<Transform> rendererList = new List<Transform>();

    private void OnEnable()
    {
        if (CorruptionModifierActive)
        {
            player.OnFaithChanged += KarmaModifier;
        }

    }

    private void OnDisable()
    {
        if (CorruptionModifierActive)
        {
            player.OnFaithChanged -= KarmaModifier;
        }
    }
    
    float time = 0;

    // Update is called once per frame
    void Update()
    {
        currentCorruption = Mathf.Lerp(currentCorruption, targetCorruption, 2f * Time.deltaTime);

    }

    [SerializeField] private float newMin = 0;
    [SerializeField] private float newMax = 1;

     
    private void KarmaModifier(float karma, float minKarma, float maxKarma)
    {
        var t = Mathf.InverseLerp(minKarma, maxKarma, karma);
        float output = Mathf.Lerp(newMin, newMax, t);

        targetCorruption = output;

        UpdateCorruption(output);
    }

    private void UpdateCorruption(float output)
    {
        foreach (Transform transform in rendererList)
        {
            foreach (Material m in transform.GetComponentInChildren<Renderer>().sharedMaterials)
            {
                m.SetFloat("_Karma", output);
                m.SetFloat("_LeafDensity", output);

                if (behaviours.isPsychdelicMode)
                {
                    m.SetFloat("_WarpStrength", output);
                }
            }
        }
    }
}

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

    void Awake()
    {
        Renderer[] objectRenderers = transform.GetComponentsInChildren<Renderer>();
        rendererList = objectRenderers.ToList();
        //behaviours = player.GetComponentInChildren<CharacterBehaviours>();
    }

    [SerializeField] List<Renderer> rendererList = new List<Renderer>();

    private void OnEnable()
    {
        if (CorruptionModifierActive)
        {
            player.OnFaithChanged += KarmaModifier;
        }

        return;

    }

    private void OnDisable()
    {
        if (CorruptionModifierActive)
        {
            player.OnFaithChanged -= KarmaModifier;
        }

        return;
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

        foreach (Renderer r in rendererList)
        {
            foreach (Material m in r.sharedMaterials)
            {
                m.SetFloat("_Karma", output);
                m.SetFloat("_LeafDensity", output);

                m.SetFloat("_MinKarma", newMin);
                m.SetFloat("_MaxKarma", newMax);

                m.SetFloat("_NewMin", newMin);
                m.SetFloat("_NewMax", newMax);

                if (behaviours.isPsychdelicMode)
                {
                    m.SetFloat("_WarpStrength", output);
                    m.SetFloat("_MinWarpStrength", newMin);
                    m.SetFloat("_MaxWarpStrength", newMax);
                }
            }

        }

    }
 

}

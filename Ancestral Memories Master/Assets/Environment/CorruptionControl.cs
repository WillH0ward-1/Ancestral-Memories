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
    private CharacterBehaviours behaviours;
    // Start is called before the first frame update

    void Awake()
    {
        GetRenderers();

        behaviours = player.GetComponentInChildren<CharacterBehaviours>();
    }

    [SerializeField] List<Renderer> rendererList = new List<Renderer>();

    void GetRenderers()
    {
        Renderer[] objectRenderers = transform.GetComponentsInChildren<Renderer>();
        
        rendererList = objectRenderers.ToList();
    }

    private void OnEnable()
    {
        player.OnFaithChanged += KarmaModifier;
    }

    private void OnDisable()
    {
        player.OnFaithChanged -= KarmaModifier;
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
            r.sharedMaterial.SetFloat("_Karma", output);

            r.sharedMaterial.SetFloat("_MinKarma", newMin);
            r.sharedMaterial.SetFloat("_MaxKarma", newMax);

            r.sharedMaterial.SetFloat("_NewMin", newMin);
            r.sharedMaterial.SetFloat("_NewMax", newMax);

            if (behaviours.isPsychdelicMode)
            {
                r.sharedMaterial.SetFloat("_WarpStrength", output);
            }

        }

    }
 

}

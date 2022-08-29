using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodRayControl : MonoBehaviour
{

    public Material godRayShader;

    [SerializeField]
    private CharacterClass player;

    public float maxAura = 1f;
    public float minAura = -1f;

    public Renderer[] auraRenderers = new Renderer[0];

    private float auraIntensity;

    public bool godRay = false;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        godRay = false;
        auraIntensity = godRayShader.GetFloat("_AuraIntensity");

    }

    public void Update()
    {
        if (player.playerIsReviving == true && godRay == true )
        {
            TriggerGodRay();
        }

        if (godRay == false)
        {
            RemoveGodRay();
        }
    

        void TriggerGodRay()
        {
            auraIntensity = minAura;

            minAura = Mathf.Lerp(minAura, maxAura, 2f * Time.deltaTime);

            foreach (Renderer renderer in auraRenderers)
            {
                renderer.material.SetFloat("_AuraIntensity", minAura);
            }
        }

        void RemoveGodRay()
        {
            maxAura = Mathf.Lerp(maxAura, minAura, 2f * Time.deltaTime);

            foreach (Renderer renderer in auraRenderers)
            {
                renderer.material.SetFloat("_AuraIntensity", maxAura);
            }
        }
    }
}


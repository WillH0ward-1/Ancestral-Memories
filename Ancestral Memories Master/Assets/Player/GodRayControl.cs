using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodRayControl : Human

{
     public Material godRayShader;

     [SerializeField] private float maxAura = 1f;
     [SerializeField] private float minAura = 0f;

     public Renderer[] auraRenderers = new Renderer[0];

     private float auraIntensity;

     public bool godRay = false;

     [SerializeField] private float lerpDuration = 1;

     // Start is called before the first frame update
     void Start()
     {
         //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
         godRay = false;
         auraIntensity = godRayShader.GetFloat("_AuraIntensity");

     }

     public IEnumerator TriggerGodRay()
     {
         auraIntensity = minAura;

         float lerpAura = Mathf.Lerp(minAura, maxAura, Time.deltaTime / lerpDuration);
         float timeElapsed = 0;

         while (timeElapsed <= lerpDuration)
         {
             foreach (Renderer renderer in auraRenderers)
             {
                 renderer.material.SetFloat("_AuraIntensity", lerpAura);
             }
         }

         if (timeElapsed >= lerpDuration)
         {
             StartCoroutine(RemoveGodRay());
             yield return null;
         }
     }

     IEnumerator RemoveGodRay()
     {
         auraIntensity = minAura;

         float lerpAura = Mathf.Lerp(maxAura, minAura, Time.deltaTime / lerpDuration);
         float timeElapsed = 0;

         while (timeElapsed <= lerpDuration)
         {
             foreach (Renderer renderer in auraRenderers)
             {
                 renderer.material.SetFloat("_AuraIntensity", lerpAura);
             }
         }

         if (timeElapsed >= lerpDuration)
         {
             yield return null;
         }
     }
}




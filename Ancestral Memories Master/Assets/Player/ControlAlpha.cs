using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

public class ControlAlpha : MonoBehaviour
{

    [SerializeField] public CharacterClass player;

    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    private Material alphaShader;

    [SerializeField] public GameObject humanObject;
    [SerializeField] public GameObject monkeyObject;

    [SerializeField] private Renderer[] monkeyRenderers;
    [SerializeField] private Renderer[] humanRenderers;

    private Material humanMaterial;
    private Material monkeyMaterial;

    [SerializeField] private float humanCurrentAlphaValue = 0f;
    [SerializeField] private float humanTargetAlphaValue = 0f;

    [SerializeField] private float monkeyCurrentAlphaValue = 0f;
    [SerializeField] private float monkeyTargetAlphaValue = 0f;

    private float maxAlpha = 1f;
    private float minAlpha = 0f;

    [SerializeField] private float lerpDuration = 1f;
    [SerializeField] private float retriggerBuffer = 2f;

    private int blendShapeIndex = 0;

    [SerializeField] float blendShapeDuration = 1f;
    [SerializeField] float currentBlendShapeWeight = 0f;
    [SerializeField] float targetBlendShapeWeight = 0f;

    public bool playerIsHuman = false;
    public bool playerIsTransforming = false;

    private void Awake()
    {
        humanRenderers = humanObject.GetComponentsInChildren<Renderer>();
        monkeyRenderers = monkeyObject.GetComponentsInChildren<Renderer>();

        playerIsHuman = true;
        playerIsTransforming = false;
        //blendShapeDuration = lerpDuration;
        CheckRenderers();

    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (!playerIsTransforming) {

                playerIsHuman = !playerIsHuman; 

                player.SwitchAnimators();

                StartCoroutine(Fade());
                StartCoroutine(StartBlendShape());

                //Debug.Log("playerIsMonkey?: " + playerIsMonkey);
            }
        }
    }

    void CheckRenderers()
    {
        if (!playerIsTransforming)
        {
            if (!playerIsHuman)
            {
                DisableRenderers(humanObject);
            }

            else if (playerIsHuman)
            {
                DisableRenderers(monkeyObject);
            }
        }
    }

    private IEnumerator Fade()
    {

        EnableRenderers(humanObject);
        EnableRenderers(monkeyObject);

        playerIsTransforming = true;

        monkeyCurrentAlphaValue = playerIsHuman ? maxAlpha : minAlpha; // Fade in monkey.
        monkeyTargetAlphaValue = playerIsHuman ? minAlpha : maxAlpha;

        humanCurrentAlphaValue = playerIsHuman ? minAlpha : maxAlpha; // Fade out human.
        humanTargetAlphaValue = playerIsHuman ? maxAlpha : minAlpha;

        //float timeElapsed = 0;

        float t = 0f;

        /*
        while (timeElapsed <= lerpDuration)
        {

            timeElapsed += Time.deltaTime;

            float humanLerpVal = Mathf.Lerp(humanCurrentAlphaValue, humanTargetAlphaValue, timeElapsed / lerpDuration);
            float monkeyLerpVal = Mathf.Lerp(monkeyCurrentAlphaValue, monkeyTargetAlphaValue, timeElapsed / lerpDuration);

                foreach (Renderer renderer in humanRenderers)
                {

                    foreach (Material material in renderer.GetComponentInChildren<Renderer>().materials)
                    {
                        material.SetFloat("_Alpha", humanLerpVal);
                    }
                }
            
                foreach (Renderer renderer in monkeyRenderers)
                {
                    foreach (Material material in renderer.GetComponentInChildren<Renderer>().materials)
                    {
                        material.SetFloat("_Alpha", monkeyLerpVal);
                    }
                }
        }
        */

        while (t < 1f)
        {

            t += Time.deltaTime / lerpDuration;

            float humanLerpVal = Mathf.Lerp(humanCurrentAlphaValue, humanTargetAlphaValue, t);
            float monkeyLerpVal = Mathf.Lerp(monkeyCurrentAlphaValue, monkeyTargetAlphaValue, t);

            foreach (Renderer renderer in humanRenderers)
            {

                foreach (Material material in renderer.GetComponentInChildren<Renderer>().materials)
                {
                    material.SetFloat("_Alpha", humanLerpVal);
                }
            }

            foreach (Renderer renderer in monkeyRenderers)
            {
                foreach (Material material in renderer.GetComponentInChildren<Renderer>().materials)
                {
                    material.SetFloat("_Alpha", monkeyLerpVal);
                }
            }

            yield return null;
        }

        yield return new WaitForSeconds(retriggerBuffer);
        playerIsTransforming = false;
        CheckRenderers();
        
    }

    IEnumerator StartBlendShape()
    {
        SetTargetBlendShape();

        void SetTargetBlendShape()
        {
            if (playerIsHuman == false)
            {
                currentBlendShapeWeight = 100;
                targetBlendShapeWeight = 0;
            }

            else if (playerIsHuman == true)
            {
                currentBlendShapeWeight = 0;
                targetBlendShapeWeight = 100;
            }
        }

        float timeElapsed = 0;

        while (timeElapsed <= blendShapeDuration)
        {
            float lerpVal = Mathf.Lerp(currentBlendShapeWeight, targetBlendShapeWeight, timeElapsed / blendShapeDuration);
            timeElapsed += Time.deltaTime;

            meshRenderer.SetBlendShapeWeight(blendShapeIndex, lerpVal);

            yield return null;
        }
    }

    public void DisableRenderers(GameObject state)
    {

        SkinnedMeshRenderer[] meshRenderers = state.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }

    }

    public void EnableRenderers(GameObject state)
    { 
        SkinnedMeshRenderer[] meshRenderers = state.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

        foreach (SkinnedMeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = true;
        }
    }
}

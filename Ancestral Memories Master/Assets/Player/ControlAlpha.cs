using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

public class ControlAlpha : MonoBehaviour
{

    [SerializeField] private SkinnedMeshRenderer meshRenderer;

    [SerializeField] public GameObject humanObject;
    [SerializeField] public GameObject monkeyObject;

    [SerializeField] private GameObject humanRenderer;
    [SerializeField] private GameObject monkeyRenderer;

    [SerializeField] private Renderer[] monkeyRenderers;
    [SerializeField] private Renderer[] humanRenderers;

    private Material humanMaterial;
    private Material monkeyMaterial;

    [SerializeField] private float humanCurrentAlphaValue = 0f;
    [SerializeField] private float humanTargetAlphaValue = 0f;

    [SerializeField] private float monkeyCurrentAlphaValue = 0f;
    [SerializeField] private float monkeyTargetAlphaValue = 0f;

    private float maxAlpha = 1;
    private float minAlpha = 0;

    [SerializeField] private float lerpDuration = 1;
    [SerializeField] private float retriggerBuffer = 2;

    private int blendShapeIndex = 0;
    [SerializeField] float blendShapeDuration = 0;
    [SerializeField] float currentBlendShapeWeight = 0;
    [SerializeField] float targetBlendShapeWeight = 0;

    public bool playerIsHuman = false;
    public bool playerIsTransforming = false;

    private AnimationManager animator;

    private void Awake()
    {
        playerIsHuman = false;
        playerIsTransforming = false;
        blendShapeDuration = lerpDuration;
        CheckRenderers();

    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (!playerIsTransforming) {
               
                playerIsHuman ^= true; // Invert boolean with 'xor'

                animator.SwitchAnimators();

                StartFade();
                StartCoroutine(StartBlendShape());

                //Debug.Log("playerIsMonkey?: " + playerIsMonkey);
            }
        }

    }

    void CheckRenderers()
    {
        if (!playerIsTransforming)
        {
            if (playerIsHuman == false)
            {
                DisableRenderers(humanRenderer);
                EnableRenderers(monkeyRenderer);
            }

            else if (playerIsHuman == true)
            {
                DisableRenderers(monkeyRenderer);
                EnableRenderers(humanRenderer);
            }

        }
    }

    public void StartFade()
    {
        playerIsTransforming = true;

       
        humanRenderers = humanObject.GetComponentsInChildren<Renderer>();
        monkeyRenderers = monkeyObject.GetComponentsInChildren<Renderer>();

        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        if (playerIsHuman == false) // If player is already monkey, it has to switch to human & vice versa.
        {

            /* maxAlpha = Visible | minAlpha = Invisible; */

            monkeyCurrentAlphaValue = minAlpha; // Fade in monkey.
            monkeyTargetAlphaValue = maxAlpha;

            humanCurrentAlphaValue = maxAlpha; // Fade out human.
            humanTargetAlphaValue = minAlpha;

        }

        else if (playerIsHuman == true)
        {
            monkeyCurrentAlphaValue = maxAlpha; // Fade out monkey.
            monkeyTargetAlphaValue = minAlpha;

            humanCurrentAlphaValue = minAlpha; // Fade in human.
            humanTargetAlphaValue = maxAlpha;
        }


        float timeElapsed = 0;

        while (timeElapsed <= lerpDuration)
        {

            float humanLerpVal = Mathf.Lerp(humanCurrentAlphaValue, humanTargetAlphaValue, timeElapsed / lerpDuration);
            float monkeyLerpVal = Mathf.Lerp(monkeyCurrentAlphaValue, monkeyTargetAlphaValue, timeElapsed / lerpDuration);

            timeElapsed += Time.deltaTime;

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

        if (timeElapsed >= lerpDuration)
        {
            yield return new WaitForSeconds(retriggerBuffer);
            playerIsTransforming = false;
            CheckRenderers();
        }
    }

    IEnumerator StartBlendShape()
    {

        SetTargetBlend();

        void SetTargetBlend()
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

        while (timeElapsed <= lerpDuration)
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

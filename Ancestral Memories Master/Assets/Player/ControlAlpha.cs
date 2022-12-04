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
    [SerializeField] public GameObject skeletonObject;


    [SerializeField] private Renderer[] monkeyRenderers;
    [SerializeField] private Renderer[] humanRenderers;
    [SerializeField] private Renderer[] skeletonRenderers;

    private Material humanMaterial;
    private Material monkeyMaterial;
    private Material skeletonMaterial;

    [SerializeField] private float humanCurrentAlphaValue = 0f;
    [SerializeField] private float humanTargetAlphaValue = 0f;

    [SerializeField] private float monkeyCurrentAlphaValue = 0f;
    [SerializeField] private float monkeyTargetAlphaValue = 0f;

    [SerializeField] private float skeletonCurrentAlphaValue = 0f;
    [SerializeField] private float skeletonTargetAlphaValue = 0f;

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
    public bool playerIsSkeleton = false;

    private void Awake()
    {
        humanRenderers = humanObject.GetComponentsInChildren<Renderer>();
        monkeyRenderers = monkeyObject.GetComponentsInChildren<Renderer>();
        skeletonRenderers = skeletonObject.GetComponentsInChildren<Renderer>();

        playerIsHuman = true;
        playerIsSkeleton = false;

        playerIsTransforming = false;
        //blendShapeDuration = lerpDuration;
        CheckRenderers();

    }

    public void SwitchHumanState()
    {
        if (!playerIsTransforming) {

            playerIsHuman = !playerIsHuman; 

            player.Assign();

            StartCoroutine(Fade());
            StartCoroutine(StartBlendShape());

            //Debug.Log("playerIsMonkey?: " + playerIsMonkey);
        }
    }

    public void SwitchSkeleton()
    {
        if (!playerIsTransforming)
        {
            playerIsSkeleton = !playerIsSkeleton;

            player.Assign();

            StartCoroutine(FadeSkeleton());
            StartCoroutine(Fade());
            if (!playerIsHuman)
            {
                StartCoroutine(StartBlendShape());
            }
            //Debug.Log("playerIsMonkey?: " + playerIsMonkey);
        }
    }
    


    void CheckRenderers()
    {
        if (!playerIsTransforming)
        {
            if (playerIsSkeleton)
            {
                DisableRenderers(monkeyObject);
                DisableRenderers(humanObject);
            }

            if (!playerIsSkeleton && playerIsHuman)
            {

                DisableRenderers(skeletonObject);
                DisableRenderers(monkeyObject);

            }
            else if (!playerIsSkeleton && !playerIsHuman)
            {
                DisableRenderers(skeletonObject);
                DisableRenderers(humanObject);
            }
        }
    }

    private IEnumerator FadeSkeleton()
    {
        EnableRenderers(skeletonObject);

        if (playerIsHuman)
        {
            humanTargetAlphaValue = minAlpha;
        }
        else if (!playerIsHuman)
        {
            monkeyTargetAlphaValue = minAlpha;
        }

        skeletonCurrentAlphaValue = playerIsSkeleton ? minAlpha : maxAlpha;
        skeletonTargetAlphaValue = playerIsSkeleton ? maxAlpha : minAlpha;

        float t = 0f;

        while (t <= 1f)
        {

            t += Time.deltaTime / lerpDuration;

            float humanLerpVal = Mathf.Lerp(humanCurrentAlphaValue, humanTargetAlphaValue, t);
            float monkeyLerpVal = Mathf.Lerp(monkeyCurrentAlphaValue, monkeyTargetAlphaValue, t);
            float skeletonLerpVal = Mathf.Lerp(skeletonCurrentAlphaValue, skeletonTargetAlphaValue, t);

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

            foreach (Renderer renderer in skeletonRenderers)
            {
                foreach (Material material in renderer.GetComponentInChildren<Renderer>().materials)
                {
                    material.SetFloat("_Alpha", skeletonLerpVal);
                }
            }

            yield return null;
        }

        if (t >= 1f)
        {
            CheckRenderers();
            playerIsTransforming = false;
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

            if (t >= 1)
            {
                yield return null;
                playerIsTransforming = false;
                CheckRenderers();
            }
        }
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

        float time = 0;

        while (time <= 1f)
        {
            float lerpVal = Mathf.Lerp(currentBlendShapeWeight, targetBlendShapeWeight, time);
            time += Time.deltaTime / blendShapeDuration;

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

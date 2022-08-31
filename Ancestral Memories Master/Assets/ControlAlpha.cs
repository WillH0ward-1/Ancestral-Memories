using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

public class ControlAlpha : MonoBehaviour
{

    public CharacterClass player;

    public SkinnedMeshRenderer meshRenderer;

    public event Action<int, int> OnAlphaChanged;

    private Material alphaShader;

    [SerializeField] private GameObject humanObject;
    [SerializeField] private GameObject monkeyObject;

    [SerializeField] private GameObject humanRenderer;
    [SerializeField] private GameObject monkeyRenderer;

    private Material humanMaterial;
    private Material monkeyMaterial;

    public float humanCurrentAlphaValue = 0f;
    public float humanTargetAlphaValue = 0f;

    public float monkeyCurrentAlphaValue = 0f;
    public float monkeyTargetAlphaValue = 0f;

    public float humanAlphaIntensity;
    public float monkeyAlphaIntensity;

    private Renderer[] monkeyRenderers;
    public Renderer[] humanRenderers;

    private int maxAlpha = 1;
    private int minAlpha = 0;

    private int blendShapeIndex = 0;

    [SerializeField] float currentBlendShapeWeight = 0;
    [SerializeField] float endBlendShapeWeight = 0;

    public bool playerIsMonkey = false;

    private void Start()
    {
        playerIsMonkey = true;
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartFade();
            StartCoroutine(StartBlendShape());
            Debug.Log("playerIsMonkey?: " + playerIsMonkey);
        }

        if (playerIsMonkey == true)
        {
            DisableRenderers(humanRenderer);
            EnableRenderers(monkeyRenderer);
        }

        else if (playerIsMonkey == false)
        {
            DisableRenderers(monkeyRenderer);
            EnableRenderers(humanRenderer);
        }

    }

    public void StartFade()
    {

        humanRenderers = humanObject.GetComponentsInChildren<Renderer>();
        monkeyRenderers = monkeyObject.GetComponentsInChildren<Renderer>();

        Debug.Log("humanRenders: " + humanRenderers);
        Debug.Log("monkeyRenderers: " + monkeyRenderers);


        foreach (Renderer renderer in humanRenderers)
        {
            var materials = new Material[renderer.materials.Length];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                humanCurrentAlphaValue = renderer.material.GetFloat("_Alpha");
            }
        }

        foreach (Renderer renderer in monkeyRenderers)
        {
            var materials = new Material[renderer.materials.Length];

            for (int i = 0; i < renderer.materials.Length; i++)
            {
                monkeyCurrentAlphaValue = renderer.material.GetFloat("_Alpha");
            }
        }

        SetTargetAlpha();

        void SetTargetAlpha()
        {
            if (playerIsMonkey == false) // If player is already monkey, it has to switch to human & vice versa.
            {
                monkeyCurrentAlphaValue = 0; // Fade in monkey.
                monkeyTargetAlphaValue = 1;

                humanCurrentAlphaValue = 1; // Fade out human.
                humanTargetAlphaValue = 0;

            }

            else if (playerIsMonkey == true)
            {

                monkeyCurrentAlphaValue = 1; // Fade out monkey.
                monkeyTargetAlphaValue = 0;

                humanCurrentAlphaValue = 0; // Fade in human.
                humanTargetAlphaValue = 1;
            }

            StartCoroutine(Fade());
        }
    }


    //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;

    [SerializeField] private float lerpDuration = 5;

    private IEnumerator Fade()
    {
        float timeElapsed = 0;

        while (timeElapsed <= lerpDuration)
        {
            humanCurrentAlphaValue = Mathf.Lerp(humanCurrentAlphaValue, humanTargetAlphaValue, timeElapsed / lerpDuration);
            monkeyCurrentAlphaValue = Mathf.Lerp(monkeyCurrentAlphaValue, monkeyTargetAlphaValue, timeElapsed / lerpDuration);
            timeElapsed += Time.deltaTime;
            yield return null;


            foreach (Material m in humanObject.GetComponentInChildren<Renderer>().materials)
            {
                m.SetFloat("_Alpha", humanCurrentAlphaValue);
            }


            foreach (Material m in monkeyObject.GetComponentInChildren<Renderer>().materials)
            {
                m.SetFloat("_Alpha", monkeyCurrentAlphaValue);
            }
        }

    }

    IEnumerator StartBlendShape()
    {
        if (playerIsMonkey == true)
        {
            currentBlendShapeWeight = 100;
            endBlendShapeWeight = 0;
        }

        else if (playerIsMonkey == false)
        {
            currentBlendShapeWeight = 0;
            endBlendShapeWeight = 100;
        }

        float timeElapsed = 0;

        while (timeElapsed <= lerpDuration)
        {
            var lerpVal = Mathf.Lerp(currentBlendShapeWeight, endBlendShapeWeight, timeElapsed / lerpDuration);
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

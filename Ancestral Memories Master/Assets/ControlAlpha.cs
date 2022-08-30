using UnityEngine;
using System;
using System.Collections.Generic;
// Copy meshes from children into the parent's Mesh.
// CombineInstance stores the list of meshes.  These are combined
// and assigned to the attached Mesh.

public class ControlAlpha : MonoBehaviour
{

    public event Action<int, int> OnAlphaChanged;

    private Material alphaShader;

    [SerializeField]
    private GameObject alphaObject;

    public int maxAlpha = 1;
    public int minAlpha = 0;

    public Transform alphaRenderers;

    private float targetAlphaValue = 0f;
    private float currentAlphaValue = 0f;

    public float alphaIntensity;

    private void OnEnable() => OnAlphaChanged += alphaChanged;

    private void OnDisable() => OnAlphaChanged -= alphaChanged;

    private MonkeyMorph morph;

    List<Renderer> allChildRenderers;

    void Start()
    {

        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        alphaShader = alphaObject.GetComponent(alphaObject.transform.material.);
        alphaIntensity = alphaShader.GetFloat("_Alpha");
        alphaIntensity = maxAlpha;

        alphaRenderers.GetComponentsInChildren<Renderer>();

    }

    void Lerp()
    {

        currentAlphaValue = Mathf.Lerp(currentAlphaValue, targetAlphaValue, 2f * Time.deltaTime);

        foreach (Renderer renderer in alphaRenderers)
        {
            renderer.material.SetFloat("_Alpha", currentAlphaValue);
        }

        OnAlphaChanged?.Invoke((int)currentAlphaValue, maxAlpha);

    }

    private void alphaChanged(int alpha, int maxAlpha)
    {
        targetAlphaValue = (float)alpha / maxAlpha;
    }
}

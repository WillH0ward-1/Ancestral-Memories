using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeShaders : MonoBehaviour
{
    public Material leafMaterial;

    [SerializeField]
    private CharacterClass player;

    public int maxAlpha = 1;
    public int minAlpha = 0;

    public Renderer leaves;

    private float targetAlphaValue = 1f;
    private float currentAlphaValue = 1f;

    public float alpha;

    private void OnEnable() => player.OnFaithChanged += FaithChanged;
    private void OnDisable() => player.OnFaithChanged -= FaithChanged;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;

        alpha = minAlpha;
    }

    private bool kill;

    // Update is called once per frame
    public IEnumerator GrowLeaves(float duration)
    {
        targetAlphaValue = maxAlpha;

        float time = 0;

        while (time < duration)
        {
            currentAlphaValue = leaves.material.GetFloat("_Alpha");
            leaves.material.SetFloat("_Alpha", Mathf.Lerp(currentAlphaValue, targetAlphaValue, time / duration));
          
            yield return null;
        }

        if (time > duration)
        {
            yield break;
        }
    }

    public IEnumerator KillLeaves(float duration)
    {
        targetAlphaValue = minAlpha;

        float time = 0;

        while (time < duration)
        {

            currentAlphaValue = leaves.material.GetFloat("_Alpha");
            currentAlphaValue = Mathf.Lerp(currentAlphaValue, targetAlphaValue, time / duration);
            leaves.material.SetFloat("_Alpha", currentAlphaValue);

            yield return null;

        }

        if (time > duration)
        {
            yield break;
        }
    }

    private void FaithChanged(int faith, int maxFaith)
    {
        targetAlphaValue = (float)faith / maxFaith;
    }
}

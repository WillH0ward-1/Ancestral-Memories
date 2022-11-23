using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeShaders : MonoBehaviour
{
    public Material leafMaterial;

    string playerTag = "Player";

    [SerializeField] private Player characterClass;

    float alphaIntensity;

    public int maxAlpha = 1;
    public int minAlpha = 0;

    public Renderer[] mat;

    private float targetAlphaValue = 1f;
    private float currentAlphaValue = 1f;

    public float alpha;

    private void OnEnable() => characterClass.OnFaithChanged += LeafDensity;
    private void OnDisable() => characterClass.OnFaithChanged -= LeafDensity;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        characterClass = player.GetComponent<Player>();

    }

    private bool kill;

    // Update is called once per frame
    public IEnumerator GrowLeaves(float targetAlphaValue)
    {
        currentAlphaValue = Mathf.Lerp(currentAlphaValue, targetAlphaValue, 2f * Time.deltaTime);

        foreach (Renderer m in mat)
        {
            m.sharedMaterial.SetFloat("_Alpha", currentAlphaValue);
            yield return null; 
        }
    }

    private void LeafDensity(int faith, int maxFaith)
    {
        targetAlphaValue = (float)faith / maxFaith;
        StartCoroutine(GrowLeaves(targetAlphaValue));
    }
}

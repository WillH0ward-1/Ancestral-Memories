using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LeafControl : MonoBehaviour
{
    private float targetCorruption = 1f;
    private float currentCorruption = 0f;

    public Player player;
    private CharacterBehaviours behaviours;
    // Start is called before the first frame update

    void Start()
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
            r.sharedMaterial.SetFloat("_Alpha", output);

        }

    }


}

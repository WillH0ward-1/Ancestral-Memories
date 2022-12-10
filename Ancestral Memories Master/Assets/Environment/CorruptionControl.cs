using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CorruptionControl : MonoBehaviour
{
    private float targetCorruption = 1f;
    private float currentCorruption = 0f;

    public Player player;

    // Start is called before the first frame update

    void Awake()
    {
        GetRenderers();
    }

    [SerializeField] List<Renderer> rendererList = new List<Renderer>();

    void GetRenderers()
    {
        Renderer[] objectRenderers = transform.GetComponentsInChildren<Renderer>();
        rendererList = objectRenderers.ToList();
    }

    private void OnEnable()
    {
        player.OnFaithChanged += Corruption;
    }

    private void OnDisable()
    {
        player.OnFaithChanged -= Corruption;
    }

    float time = 0;

    // Update is called once per frame
    void Update()
    {
        currentCorruption = Mathf.Lerp(currentCorruption, targetCorruption, 2f * Time.deltaTime);

    }

    float newMin = 1;
    float newMax = 0;


    private void Corruption(float corruption, float minCorruption, float maxCorruption)
    {
        var t = Mathf.InverseLerp(minCorruption, maxCorruption, corruption);
        float output = Mathf.Lerp(newMin, newMax, t);

        targetCorruption = output;

        foreach (Renderer r in rendererList)
        {
            r.material.SetFloat("_Corruption", currentCorruption);

            r.material.SetFloat("_CorruptionMin", newMin);
            r.material.SetFloat("_CorruptionMax", newMax);

        }

    }
 

}

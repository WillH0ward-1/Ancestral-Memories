using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class NightSwitch : MonoBehaviour
{
    [SerializeField] private float darknessAmount;
    [SerializeField] private Material material;

    [SerializeField] private float currentDarkAmount;
    [SerializeField] private float targetDarkAmount;

    [SerializeField] private float transitionDuration = 1f;

    [SerializeField] private LightingManager lightingManager;

    
    public IEnumerator ToNightSky()
    {
        material = transform.GetComponent<MeshRenderer>().sharedMaterial;
        darknessAmount = material.GetFloat("_NightFilter");

        currentDarkAmount = darknessAmount;
        targetDarkAmount = 1f;


        nightTime = true;
        dayTime = false;

        float time = 0f;

        while (time <= 1f)
        {
            currentDarkAmount = Mathf.Lerp(darknessAmount, targetDarkAmount, time);
            time += Time.deltaTime / transitionDuration;

            yield return null;
        }


        yield break;
    }

    public bool dayTime;
    public bool nightTime;


    public IEnumerator ToDaySky()
    {
        material = transform.GetComponent<MeshRenderer>().sharedMaterial;
        darknessAmount = material.GetFloat("_NightFilter");

        currentDarkAmount = darknessAmount;
        targetDarkAmount = 0f;

        dayTime = true;
        nightTime = false;

        float time = 0f;

        while (time <= 1f)
        {
            currentDarkAmount = Mathf.Lerp(darknessAmount, targetDarkAmount, time);
            time += Time.deltaTime / transitionDuration;

            yield return null;
        }

        yield break;
        
    }

    void Update()
    {

        material.SetFloat("_NightFilter", currentDarkAmount);

    }
}

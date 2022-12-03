using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodRayControl : MonoBehaviour

{

    [SerializeField]
    private CharacterClass player;


    public bool godRay = false;

    [SerializeField] private GameObject godRayPrefab;

    [SerializeField] private float duration = 1f;
    [SerializeField] private float godRayDuration;

    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;

    [SerializeField] private float yOffset = 0f;

    private CharacterBehaviours characterBehaviours;


    [SerializeField] private int playerTargetSizeDivide = 25;
    [SerializeField] private int animalTargetSizeDivide = 50;
    [SerializeField] private int treeTargetSizeDivide = 50;

    Material godRayMat;

    private void Awake()
    {
        characterBehaviours = player.GetComponent<CharacterBehaviours>();
    }
    public void StartGodRay(Transform target, bool manuallyCease, float duration)
    {
        StartCoroutine(GodRay(target, manuallyCease, duration));
    }

    public IEnumerator GodRay(Transform target, bool manuallyCease, float duration)
    {
        int sizeDivide = 0;

        switch (target.tag)
        {
            case "Player":
                sizeDivide = playerTargetSizeDivide;
                break;
            case "Animal":
                sizeDivide = animalTargetSizeDivide;
                break;
            case "Trees":
                sizeDivide = treeTargetSizeDivide;
                break;
            default:
                Debug.Log("No case detected to set " + transform.name + " size");
                break;
        }

        Debug.Log(target.tag + sizeDivide);

        GameObject godRay = Instantiate(godRayPrefab, target.transform.position, Quaternion.identity, target.transform);

        Renderer renderer = godRay.GetComponent<Renderer>();
        godRayMat = renderer.material;

        godRay.transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);

        Vector3 sizeCalculated = target.transform.GetComponentInChildren<Renderer>().bounds.size / sizeDivide;
        godRay.transform.localScale = sizeCalculated;

        float timeElapsed = 0;

        float lerpAura;

        while (timeElapsed <= duration)
        {
            lerpAura = Mathf.Lerp(minIntensity, maxIntensity, timeElapsed);
            godRayMat.SetFloat("_AuraIntensity", lerpAura);

            timeElapsed += Time.deltaTime / duration;
            yield return null;
        }

        if (timeElapsed >= duration && manuallyCease)
        {
            yield return new WaitForSeconds(godRayDuration);
            yield return Retreat(godRay);
            yield break;

        } else if (timeElapsed >= duration && !manuallyCease)
        {
           
            StartCoroutine(Retreat(godRay));

            yield break;

        }
    }


    public IEnumerator Retreat(GameObject godRay)
    {
        Debug.Log("GodRay End!");

        float timeElapsed = 0;

        float lerpAura;

        while (timeElapsed <= 1f)
        {
            lerpAura = Mathf.Lerp(maxIntensity, minIntensity, timeElapsed);
            godRayMat.SetFloat("_AuraIntensity", lerpAura);

            timeElapsed += Time.deltaTime / duration;
            yield return null;
        }

        if (timeElapsed >= 1f)
        {
            Destroy(godRay);

            yield break;
        }
    }
}



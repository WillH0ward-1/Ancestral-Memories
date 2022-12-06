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

    [SerializeField] private float inDuration = 1f;
    [SerializeField] private float outDuration = 1f;

    [SerializeField] private float minIntensity;
    [SerializeField] private float maxIntensity;

    [SerializeField] private float yOffset = 0f;

    private CharacterBehaviours characterBehaviours;


    [SerializeField] private int playerTargetSizeDivide = 25;
    [SerializeField] private int animalTargetSizeDivide = 50;
    [SerializeField] private int treeTargetSizeDivide = 50;

    private Renderer[] renderers = new Renderer[0];

    private void Awake()
    {
        characterBehaviours = player.GetComponent<CharacterBehaviours>();
    }
    public void StartGodRay(Transform target, bool manuallyCease)
    {
        StartCoroutine(GodRay(target, manuallyCease));
    }

    private float lerpAura;

    public IEnumerator GodRay(Transform target, bool manuallyCease)
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

        Vector3 sizeCalculated = target.transform.GetComponentInChildren<Renderer>().bounds.size / sizeDivide;
        godRay.transform.localScale = sizeCalculated;


        renderers = godRay.GetComponentsInChildren<Renderer>();

        foreach (Renderer r in renderers)
        {
            Material godRayMat = r.material;

            godRay.transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);

            float time = 0;

            while (time <= 1f)
            {
                lerpAura = Mathf.Lerp(minIntensity, maxIntensity, time);
                godRayMat.SetFloat("_AuraIntensity", lerpAura);

                time += Time.deltaTime / inDuration;
                yield return null;
            }

            if (time >= 1f && manuallyCease)
            {
                StartCoroutine(Retreat(godRay));
                yield break;

            }
            else if (time >= 1 && !manuallyCease)
            {
                yield return new WaitUntil(() => !characterBehaviours.behaviourIsActive);
                StartCoroutine(Retreat(godRay));

                yield break;

            }
        }

        yield break;
    }


    public IEnumerator Retreat(GameObject godRay)
    {
        Debug.Log("GodRay End!");

        float time = 0;

        float lerpAura;

        foreach (Renderer r in renderers)
        {
            Material godRayMat = r.material;

            while (time <= 1f)
            {
                lerpAura = Mathf.Lerp(maxIntensity, minIntensity, time);
                godRayMat.SetFloat("_AuraIntensity", lerpAura);

                time += Time.deltaTime / outDuration;
                yield return null;
            }

            if (time >= 1f)
            {
                Destroy(godRay);

                yield break;
            }
        }
    }
}



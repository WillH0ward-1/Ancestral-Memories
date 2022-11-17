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

    Material godRayMat;

    private void Awake()
    {
        characterBehaviours = player.GetComponent<CharacterBehaviours>();
    }
    public void StartGodRay(Transform target, bool manuallyCease)
    {
        StartCoroutine(GodRay(target, manuallyCease));
    }

    public IEnumerator GodRay(Transform target, bool manuallyCease)
    {
        Debug.Log("GodRay!");

        GameObject godRay = Instantiate(godRayPrefab, target.transform.position, Quaternion.identity, target.transform);

        Renderer renderer = godRay.GetComponent<Renderer>();
        godRayMat = renderer.material;

        godRay.transform.position = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);

        Vector3 sizeCalculated = target.transform.GetComponentInChildren<Renderer>().bounds.size;
        godRay.transform.localScale = sizeCalculated;

        float timeElapsed = 0;

        float lerpAura;

        while (timeElapsed < duration)
        {
            lerpAura = Mathf.Lerp(minIntensity, maxIntensity, timeElapsed / duration);
            godRayMat.SetFloat("_AuraIntensity", lerpAura);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (timeElapsed >= duration && manuallyCease)
        {
            yield return new WaitForSeconds(godRayDuration);
            yield return Retreat(godRay);
            yield break;

        } else if (timeElapsed >= duration && !manuallyCease)
        {
            yield return new WaitUntil(() => !characterBehaviours.behaviourIsActive);

            StartCoroutine(Retreat(godRay));

            yield break;

        }
    }


    public IEnumerator Retreat(GameObject godRay)
    {
        Debug.Log("GodRay End!");

        float timeElapsed = 0;

        float lerpAura;

        while (timeElapsed < duration)
        {
            lerpAura = Mathf.Lerp(maxIntensity, minIntensity, timeElapsed / duration);
            godRayMat.SetFloat("_AuraIntensity", lerpAura);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        if (timeElapsed >= duration)
        {
            Destroy(godRay);

            yield break;
        }
    }
}



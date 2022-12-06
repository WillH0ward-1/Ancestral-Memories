using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnPlayer : MonoBehaviour
{

    private float currentBurn;
    private float targetBurn;

    [SerializeField] private float minBurn = 0;
    [SerializeField] private float maxBurn = 16;

    [SerializeField] private float burnSpeed = 1;

    public float burnIntensity;
    public Renderer[] renderers = new Renderer[0];
    public Material auraMaterial;

    private bool playerInFire;

    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        playerInFire = false;
        burnIntensity = auraMaterial.GetFloat("_AuraIntensity");
        burnIntensity = maxBurn;

        renderers = GetComponentsInChildren<Renderer>();

    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Fire"))
        {
            Debug.Log("Fire Detected!");
            playerInFire = true; 

            targetBurn = maxBurn;
            StartCoroutine(Burn(targetBurn));
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("Player"))
        {
            Debug.Log("Fire Exited!");

            playerInFire = false;
            StopCoroutine(Burn(targetBurn));

            targetBurn = minBurn;
            StartCoroutine(Burn(targetBurn));
        }
    }


    private IEnumerator Burn(float targetBurn)
    {
        float time = 0;

        foreach (Renderer renderer in renderers)
        {
            renderer.sharedMaterial.SetFloat("_ColourPrecision", currentBurn);
        }

        while (time <= 1f)
        {
            currentBurn = Mathf.Lerp(currentBurn, targetBurn, time);
            time += Time.deltaTime / burnSpeed;
        }

        if (time >= 1f || currentBurn >= targetBurn)
        {
            yield break;
        }

        yield return null;
    }
   
}

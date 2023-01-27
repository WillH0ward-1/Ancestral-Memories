using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnPlayer : MonoBehaviour
{

    private float currentBurn;
    private float targetBurn;

    [SerializeField] private float burn;
    [SerializeField] private float minBurn = 64;
    [SerializeField] private float maxBurn = 0;

    [SerializeField] private float burnSpeed = 5;

    public Renderer[] renderers = new Renderer[0];

    [SerializeField] private bool playerInFire;

    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        playerInFire = false;

        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();

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
        if (other.transform.CompareTag("Fire"))
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

        while (time <= 1f)
        {
            currentBurn = Mathf.Lerp(burn, targetBurn, time);

            foreach (Renderer renderer in renderers)
            {
                burn = renderer.sharedMaterial.GetFloat("_ColourPrecision");
                burn = currentBurn;
            }

            time += Time.deltaTime / burnSpeed;
        }

        if (time >= 1f || currentBurn >= targetBurn)
        {
            yield break;
        }

        yield return null;
    }
   
}

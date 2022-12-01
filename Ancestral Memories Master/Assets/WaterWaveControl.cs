using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWaveControl : MonoBehaviour
{

    [SerializeField]
    private CharacterClass player;


    private float targetDeform = 1f;
    private float currentDeform = 1f;

    private float waveSpeed = 1f;
    private float waveSpeedMultiplier = 1f;

    [SerializeField] float offset;

    private bool wavesActive = true;

    //private void OnEnable() => player.OnHungerChanged += HungerChanged;
    //private void OnDisable() => player.OnHungerChanged -= HungerChanged;

    // Start is called before the first frame update
    void Start()
    {
        //auraShader = GetComponent<SkinnedMeshRenderer>().sharedMaterial;
        wavesActive = true;
        offset = transform.GetComponentInChildren<Deform.RippleDeformer>().Offset;
        StartCoroutine(AnimateWaves());
    }


    private IEnumerator AnimateWaves()
    {

        while (wavesActive)
        {
            waveSpeed++;
            offset += waveSpeed * waveSpeedMultiplier;
            yield return null;
        }

        yield break;
    }

    /*
    private void HungerChanged(int hunger, int maxHunger)
    {
        float x = 1;
        float t = Mathf.InverseLerp(hunger, maxHunger, x);
        float output = Mathf.Lerp(minVal, maxVal, t);

        targetDeform = (float)output / maxHunger;
    }
    */
}

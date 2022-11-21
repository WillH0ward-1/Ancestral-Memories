using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafFallControl : MonoBehaviour
{
    [SerializeField] private ParticleSystem leaves;

    [SerializeField] public int emissionRate = 0; // 0 - 100

    public void Start()
    {
        leaves = transform.GetComponent<ParticleSystem>();

        var emission = leaves.emission;

        emission.enabled = false;

        emission.rateOverTime = emissionRate;

    }


 
 
}

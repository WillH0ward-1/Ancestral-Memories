using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowMushroom : MonoBehaviour
{
    [SerializeField] private ScaleControl scaleControl;

    [SerializeField] private Vector3 scaleStart;
    [SerializeField] private Vector3 scaleTarget;

    private float growDelay;
    [SerializeField] private float minGrowDelay;
    [SerializeField] private float maxGrowDelay;

    private float growDuration;
    [SerializeField] private float minGrowDuration;
    [SerializeField] private float maxGrowDuration;

    void Start()
    {
        scaleControl = transform.GetComponent<ScaleControl>();

        scaleStart = new(0, 0, 0);
        scaleTarget = new(1, 1, 1);

        growDelay = Random.Range(minGrowDelay, maxGrowDelay);
        growDuration = Random.Range(minGrowDuration, maxGrowDuration);

        StartCoroutine(scaleControl.LerpScale(transform.gameObject, scaleStart, scaleTarget, growDuration, growDelay));
    }

}

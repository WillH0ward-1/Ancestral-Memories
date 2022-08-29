using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterBob : MonoBehaviour
{
    [SerializeField]
    Vector3 startPos;

    [SerializeField]
    private float amplitude = 0.2f;

    [SerializeField]
    private float period = 1f;

    void Awake()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float theta = Time.timeSinceLevelLoad / period;
        float distance = amplitude * Mathf.Sin(theta);
        transform.position = startPos + Vector3.up * distance;
    }
}
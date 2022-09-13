using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterBob : MonoBehaviour
{
    [SerializeField]
    private Vector3 startPos;

    [SerializeField]
    private float amplitude = 0.2f;

    [SerializeField]
    private float period = 1f;

    private Vector3 nullVector = new Vector3(0,0,0);

    void Awake()
    {
        transform.position = startPos;
    }

    void Update()
    {
        if (transform.position != nullVector)
        {
            return;
        } else {

        float theta = Time.timeSinceLevelLoad / period;
        float distance = amplitude * Mathf.Sin(theta);
        transform.position = startPos + Vector3.up * distance;

        }
    }
}
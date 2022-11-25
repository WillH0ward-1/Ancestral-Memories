using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    private bool isFloating = false;

    [SerializeField]
    private float amplitude = 0.2f;

    [SerializeField]
    private float period = 1f;

    public IEnumerator Float(GameObject bobObject)
    {
        isFloating = true;

        while (isFloating)
        {
            float theta = Time.timeSinceLevelLoad / period;
            float distance = amplitude * Mathf.Sin(theta);
            bobObject.transform.position = bobObject.transform.position + Vector3.up * distance;
            yield return null;
        }

        if (!isFloating)
        {
            isFloating = false;
            yield break;
        }
    }
}

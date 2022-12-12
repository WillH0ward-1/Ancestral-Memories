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

    [SerializeField] private float sinkDuration = 1f;

    float bobObjectX;
    float bobObjectY;
    float bobObjectZ;

    float targetY;

    private void Awake()
    {
    
    }
    public IEnumerator Float(GameObject bobObject)
    {
        isFloating = true;

        bobObjectX = bobObject.transform.position.x;
        bobObjectY = bobObject.transform.position.y;
        bobObjectZ = bobObject.transform.position.z;


        targetY = bobObjectY - bobObjectY / 5;

        float time = 0;

        while (time <= 1f)
        {
            time += Time.deltaTime / sinkDuration;

            Vector3 target = new Vector3(bobObjectX, targetY, bobObjectZ);


            bobObject.transform.position = Vector3.Lerp(bobObject.transform.position, target, time);

            yield return null;
        }

        if (time >= 1f) {

            time = 0;

            while (isFloating)
            {
                time += Time.deltaTime;

                float theta = time / period;
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
}

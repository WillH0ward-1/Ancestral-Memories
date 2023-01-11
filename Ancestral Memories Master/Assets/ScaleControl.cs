using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleControl : MonoBehaviour
{
    [SerializeField] private float xAxisGrowMultiplier = 1;
    [SerializeField] private float yAxisGrowMultiplier = 1;
    [SerializeField] private float zAxisGrowMultiplier = 1;

    public bool isFullyGrown = false;
    public bool isGrowing = false;

    public GameObject scaleObjectRef;
    public float durationRef;
    public Vector3 scaleStartRef;
    public Vector3 scaleDestinationRef;
    public float delayRef;

    public float growthPercent;

    //[SerializeField] private RainControl rain;

    public IEnumerator LerpScale(GameObject scaleObject, Vector3 scaleStart, Vector3 scaleDestination, float duration, float delay)
    {
        if (scaleObject == null)
        {
            yield break;
        }
        else if (scaleObject != null)
        {
            isFullyGrown = false;
            isGrowing = true;

            durationRef = duration;
            scaleObjectRef = scaleObject;
            scaleStartRef = scaleStart;
            scaleDestinationRef = scaleDestination;
            delayRef = delay;

            scaleObject.transform.localScale = scaleStart;

            yield return new WaitForSeconds(delay);

            float time = 0;

            float localScaleX = scaleObject.transform.localScale.x;
            float localScaleY = scaleObject.transform.localScale.y;
            float localScaleZ = scaleObject.transform.localScale.z;

            while (time <= 1f) //&& rain.isRaining)
            {
                if (scaleObject == null)
                {
                    yield break;
                }

                time += Time.deltaTime / duration;

                growthPercent = time;

                localScaleX = Mathf.Lerp(scaleStart.x, scaleDestination.x, time * xAxisGrowMultiplier);
                localScaleY = Mathf.Lerp(scaleStart.y, scaleDestination.y, time * yAxisGrowMultiplier);
                localScaleZ = Mathf.Lerp(scaleStart.z, scaleDestination.z, time * zAxisGrowMultiplier);

                scaleObject.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);


                yield return null;
            }

            if (time >= 1f || scaleObject.transform.localScale == scaleDestination)
            {
                scaleObject.transform.localScale = scaleDestination;
                isFullyGrown = true;
                isGrowing = false;
                yield break;
            }
        }
    }
}

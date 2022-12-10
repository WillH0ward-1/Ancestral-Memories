using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleControl : MonoBehaviour
{
    public bool isFullyGrown = false;

    [SerializeField] private float xAxisGrowMultiplier = 1;
    [SerializeField] private float yAxisGrowMultiplier = 1;
    [SerializeField] private float zAxisGrowMultiplier = 1;

    public IEnumerator LerpScale(GameObject scaleObject, Vector3 scaleStart, Vector3 scaleDestination, float duration, float delay)
    {
        isFullyGrown = false;

        scaleObject.transform.localScale = scaleStart;

        yield return new WaitForSeconds(delay);

        float time = 0;

        float localScaleX = scaleObject.transform.localScale.x;
        float localScaleY = scaleObject.transform.localScale.y;
        float localScaleZ = scaleObject.transform.localScale.z;

        while (time <= 1f)
        {

            localScaleX = Mathf.Lerp(localScaleX, scaleDestination.x, time *= xAxisGrowMultiplier);
            localScaleY = Mathf.Lerp(localScaleY, scaleDestination.y, time *= yAxisGrowMultiplier);
            localScaleZ = Mathf.Lerp(localScaleZ, scaleDestination.z, time *= zAxisGrowMultiplier);

            time += Time.deltaTime / duration;

            scaleObject.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);

            yield return null;
        }

        if (time >= 1f)
        {
            scaleObject.transform.localScale = scaleDestination;
            isFullyGrown = true;
            yield break;
        }
    }
}

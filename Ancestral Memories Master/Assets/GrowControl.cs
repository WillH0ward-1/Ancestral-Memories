using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowControl : MonoBehaviour
{
    public bool isFullyGrown = false;

    public int xAxisGrowMultiplier = 1;
    public int yAxisGrowMultiplier = 1;
    public int zAxisGrowMultiplier = 1;

    public int minGrowDelay = 2;
    public int maxGrowDelay = 5;

    public IEnumerator Grow(GameObject scaleObject, Vector3 scaleStart, Vector3 scaleDestination, float duration)
    {
        scaleObject.transform.GetComponent<GrowControl>().isFullyGrown = false;

        scaleObject.transform.localScale = scaleStart;

        float startGrowDelay = Random.Range(minGrowDelay, maxGrowDelay);

        yield return new WaitForSeconds(startGrowDelay);

        float time = 0;

        float localScaleX = scaleObject.transform.localScale.x;
        float localScaleY = scaleObject.transform.localScale.y;
        float localScaleZ = scaleObject.transform.localScale.z;

        while (time <= duration)
        {
            time += Time.deltaTime;

            localScaleX = Mathf.Lerp(localScaleX, scaleDestination.x, time * xAxisGrowMultiplier / duration );
            localScaleY = Mathf.Lerp(localScaleY, scaleDestination.y, time * yAxisGrowMultiplier / duration);
            localScaleZ = Mathf.Lerp(localScaleZ, scaleDestination.z, time * zAxisGrowMultiplier / duration);

            scaleObject.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);
          
            yield return null;
        }

        if (time >= duration)
        {
            scaleObject.transform.GetComponent<GrowControl>().isFullyGrown = true;
            yield break;
        }
        
    }


}

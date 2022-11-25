using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowControl : MonoBehaviour
{
    public bool isFullyGrown = false;

    public int xAxisGrowMultiplier = 1;
    public int yAxisGrowMultiplier = 1;
    public int zAxisGrowMultiplier = 1;

    public IEnumerator Grow(GameObject scaleObject, Vector3 scaleStart, Vector3 scaleDestination, float duration, float delay)
    {

        isFullyGrown = false;

        scaleObject.transform.localScale = scaleStart;

        yield return new WaitForSeconds(delay);

        float time = 0;

        float localScaleX = scaleObject.transform.localScale.x;
        float localScaleY = scaleObject.transform.localScale.y;
        float localScaleZ = scaleObject.transform.localScale.z;

        while (time <= duration)
        {
            time += Time.deltaTime;

            localScaleX = Mathf.Lerp(localScaleX, scaleDestination.x, time / duration);
            localScaleY = Mathf.Lerp(localScaleY, scaleDestination.y, time / duration);
            localScaleZ = Mathf.Lerp(localScaleZ, scaleDestination.z, time / duration);

            localScaleX *= xAxisGrowMultiplier;
            localScaleY *= yAxisGrowMultiplier; 
            localScaleZ *= zAxisGrowMultiplier;

            scaleObject.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);
          
            yield return null;
        }

        if (time >= duration)
        {
            isFullyGrown = true;
            yield break;
        }
        
    }


}

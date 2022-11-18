using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowControl : MonoBehaviour
{
    public IEnumerator Grow(GameObject scaleObject, Vector3 scaleStart, Vector3 scaleDestination, float duration)
    {
        float startGrowDelay = Random.Range(0, 30);

        yield return new WaitForSeconds(startGrowDelay);

        float time = 0;

        Vector3 currentScale = scaleStart;

        while (time < duration)
        {
            scaleObject.transform.localScale = Vector3.Lerp(currentScale, scaleDestination, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        if (time >= duration)
        {
            yield break;
        }
    }
}

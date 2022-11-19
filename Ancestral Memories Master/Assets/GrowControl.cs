using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowControl : MonoBehaviour
{
    [SerializeField] private bool isFullyGrown;

    public float xAxisMultiplier;
    public float yAxisMultiplier;
    public float zAxisMultiplier;

    public List<GameObject> treeAppleList;

    public IEnumerator Grow(GameObject scaleObject, Vector3 scaleStart, Vector3 scaleDestination, float duration)
    {
        scaleObject.transform.localScale = scaleStart;

        float startGrowDelay = Random.Range(0, 100);

        yield return new WaitForSeconds(startGrowDelay);

        float time = 0;

        float localScaleX = scaleObject.transform.localScale.x;
        float localScaleY = scaleObject.transform.localScale.y;
        float localScaleZ = scaleObject.transform.localScale.z;

         float xAxisMultiplier = 1f;
         float yAxisMultiplier = 3f;
         float zAxisMultiplier = 1f;

        while (time < duration)
        {
            localScaleX = Mathf.Lerp(localScaleX, scaleDestination.x, time / duration * xAxisMultiplier);
            localScaleY = Mathf.Lerp(localScaleY, scaleDestination.y, time / duration * yAxisMultiplier);
            localScaleZ = Mathf.Lerp(localScaleZ, scaleDestination.z, time / duration * zAxisMultiplier);

            scaleObject.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);
            time += Time.deltaTime;
            yield return null;
        }

        if (time >= duration)
        {
            isFullyGrown = true;
            yield break;
        }
    }

    public IEnumerator GrowFruit(GameObject appleTree)
    {


        yield return null;
    }

}

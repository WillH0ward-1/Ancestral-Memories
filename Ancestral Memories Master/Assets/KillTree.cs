using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTree : MonoBehaviour
{

    public void Fall(GameObject treeObject, float duration)
    {
        Vector2 pointOnCircle = Random.insideUnitCircle * treeObject.transform.localScale.y;

        Vector3 fallPoint = treeObject.transform.position +
            pointOnCircle.x * treeObject.transform.right +
            pointOnCircle.y * treeObject.transform.forward;

        Vector3 updatedUpVector = Vector3.Normalize(fallPoint - treeObject.transform.position);

        StartCoroutine(UpdateUpVector(treeObject, updatedUpVector, duration, 0.001f));
    }

    public IEnumerator UpdateUpVector(GameObject target, Vector3 upVector, float duration, float threshold = 0.001f)
    {
        while (Vector3.Distance(upVector, target.transform.up) > threshold)
        {
            target.transform.up = Vector3.Lerp(target.transform.up, upVector, duration * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }
}

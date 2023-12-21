using System.Collections;
using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.2f;
    [SerializeField] private float period = 1f;
    [SerializeField] private float sinkDuration = 1f;

    private bool isFloating = false;

    public IEnumerator Float(GameObject bobObject)
    {
        isFloating = true;

        Vector3 start = bobObject.transform.position;
        Vector3 target = new Vector3(start.x, start.y / 5, start.z);

        // Sinking phase
        float sinkTime = 0f;
        while (sinkTime < sinkDuration)
        {
            sinkTime += Time.deltaTime;
            bobObject.transform.position = Vector3.Lerp(start, target, sinkTime / sinkDuration);
            yield return null;
        }

        // Bobbing phase
        float bobTime = 0f;
        while (isFloating)
        {
            bobTime += Time.deltaTime;
            float theta = bobTime / period;
            float distance = amplitude * Mathf.Sin(2 * Mathf.PI * theta);
            bobObject.transform.position = new Vector3(start.x, target.y + distance, start.z);
            yield return null;
        }
    }

    public void StopFloating()
    {
        isFloating = false;
    }
}


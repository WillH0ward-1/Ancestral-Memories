using UnityEngine;

[ExecuteInEditMode]
public class MoonMovement : MonoBehaviour
{
    public Transform pivot;
    public float distance = 10.0f;
    public TimeCycleManager timeCycleManager;
    [Range(0.1f, 2f)]
    public float timeScale = 1f;

    void OnEnable()
    {
        if (pivot == null)
        {
            pivot = new GameObject("Pivot").transform;
            pivot.position = Vector3.zero;
        }

        if (timeCycleManager == null)
        {
            timeCycleManager = FindObjectOfType<TimeCycleManager>();
        }

        if (timeCycleManager == null)
        {
            Debug.LogError("No TimeCycleManager found in the scene.");
        }
    }

    void Update()
    {
        if (timeCycleManager == null) return;

        float scaledTimeOfDay = (timeCycleManager.timeOfDay * timeScale) % 24f;
        float angle = 360 - (scaledTimeOfDay / 24f * 360f); // moon matches the current time

        if (angle < 0f)
        {
            angle += 360f;
        }

        float radian = angle * Mathf.Deg2Rad;

        float x = distance * Mathf.Cos(radian);
        float z = distance * Mathf.Sin(radian);
        float y = distance * Mathf.Sin(radian);

        transform.position = pivot.position + new Vector3(x, y, z);
    }
}

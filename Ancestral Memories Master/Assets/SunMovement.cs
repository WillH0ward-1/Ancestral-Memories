using UnityEngine;

[ExecuteInEditMode]
public class SunMovement : MonoBehaviour
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

        float scaledTimeOfDay = (timeCycleManager.TimeOfDay * timeScale) % 24f;
        float shiftedTimeOfDay = (scaledTimeOfDay + 12f) % 24f; // shift the sun 12 hours ahead
        float angle = 360 - (shiftedTimeOfDay / 24f * 360f);

        float radian = angle * Mathf.Deg2Rad;

        float x = distance * Mathf.Cos(radian);
        float z = distance * Mathf.Sin(radian);
        float y = distance * Mathf.Sin(radian);

        transform.position = pivot.position + new Vector3(x, y, z);
    }
}

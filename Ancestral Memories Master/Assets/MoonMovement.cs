using UnityEngine;

[ExecuteInEditMode]
public class MoonMovement : MonoBehaviour
{
    public Transform pivot;
    public float distance = 10.0f;
    public TimeCycleManager timeCycleManager;

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

        // Original angle calculation for the moon
        float angle = 360 - (timeCycleManager.TimeOfDay / 24f * 360f);
        angle %= 360f; // Ensure the angle stays within 0-360 degrees range

        float radian = angle * Mathf.Deg2Rad;

        float x = distance * Mathf.Cos(radian);
        float z = distance * Mathf.Sin(radian);
        float y = distance * Mathf.Sin(radian);

        transform.position = pivot.position + new Vector3(x, y, z);
    }
}

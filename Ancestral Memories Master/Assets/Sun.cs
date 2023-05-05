using UnityEngine;

public class Sun : MonoBehaviour
{
    [SerializeField] private float period = 60f; // in seconds
    [SerializeField] private float maxHeight = 30f;
    [SerializeField] private float minHeight = -30f;

    private float timeElapsed = 0f;
    private bool isRising = true;

    private void Update()
    {
        float normalizedTime = timeElapsed / period;
        float angle = normalizedTime * Mathf.PI * 2f;
        float height = (maxHeight - minHeight) * 0.5f * (Mathf.Sin(angle) + 1f) + minHeight;

        transform.position = new Vector3(transform.position.x, height, transform.position.z);

        if (isRising)
        {
            transform.rotation = Quaternion.Euler(360f * normalizedTime - 90f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(270f - 360f * normalizedTime, 0f, 0f);
        }

        timeElapsed += Time.deltaTime;

        if (timeElapsed >= period)
        {
            isRising = !isRising;
            timeElapsed = 0f;
        }
    }
}

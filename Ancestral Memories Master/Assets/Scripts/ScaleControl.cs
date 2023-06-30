using System.Collections;
using UnityEngine;

public class ScaleControl : MonoBehaviour
{
    [SerializeField] private float xAxisGrowMultiplier = 1f;
    [SerializeField] private float yAxisGrowMultiplier = 1f;
    [SerializeField] private float zAxisGrowMultiplier = 1f;

    public bool isFullyGrown = false;

    public float durationRef;
    public Vector3 scaleStartRef;
    public Vector3 scaleDestinationRef;
    public float delayRef;

    public float growthPercent;

    private Interactable interactable;

    private void Awake()
    {
        interactable = transform.GetComponent<Interactable>();
    }

    private void Start()
    {
        StartCoroutine(LerpScale(transform.gameObject, transform.localScale, scaleDestinationRef, durationRef, delayRef));
    }

    public IEnumerator LerpScale(GameObject scaleObject, Vector3 scaleStart, Vector3 scaleDestination, float duration, float delay)
    {
        if (scaleObject == null)
        {
            yield break;
        }

        isFullyGrown = false;
        durationRef = duration;
        scaleStartRef = scaleStart;
        scaleDestinationRef = scaleDestination;
        delayRef = delay;

        scaleObject.transform.localScale = scaleStart;

        yield return new WaitForSeconds(delay);

        if (scaleObject == null)
        {
            yield break;
        }

        float time = 0f;

        float localScaleX = scaleObject.transform.localScale.x;
        float localScaleY = scaleObject.transform.localScale.y;
        float localScaleZ = scaleObject.transform.localScale.z;

        float timeX = 0;
        float timeY = 0;
        float timeZ = 0;

        while (time <= 1f)
        {
            time += Time.deltaTime / duration;

            growthPercent = time;

            timeX += Time.deltaTime / (duration * xAxisGrowMultiplier);
            timeY += Time.deltaTime / (duration * yAxisGrowMultiplier);
            timeZ += Time.deltaTime / (duration * zAxisGrowMultiplier);

            growthPercent = time;

            localScaleX = Mathf.Lerp(scaleStart.x, scaleDestination.x, timeX);
            localScaleY = Mathf.Lerp(scaleStart.y, scaleDestination.y, timeY);
            localScaleZ = Mathf.Lerp(scaleStart.z, scaleDestination.z, timeZ);

            scaleObject.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);

            if (growthPercent <= 0.25f)
            {
                if (interactable != null)
                {
                    interactable.enabled = false;
                }
            }
            else if (growthPercent >= 0.25f)
            {
                if (interactable != null)
                {
                    interactable.enabled = true;
                }
            }

            yield return null;
        }

        scaleObject.transform.localScale = scaleDestination;

        isFullyGrown = true;
    }
}

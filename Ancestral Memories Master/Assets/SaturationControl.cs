using System.Collections;
using UnityEngine;

public class SaturationControl : MonoBehaviour
{
    private float currentSaturationAmount; // Current saturation amount
    public float saturationLerpDuration = 1.0f;
    private bool lerping = false;

    private Coroutine currentLerpCoroutine; // Variable to track the current coroutine
    private Renderer[] renderers; // Array of renderers to apply the property block
    private MaterialPropertyBlock propBlock; // Property block for material properties
    private const string kSaturationKey = "_Saturation"; // Key for saturation property in shader

    private const float minSaturation = 0f; // Minimum saturation
    private const float midSaturation = 1f; // Mid saturation
    private const float maxSaturation = 2f; // Maximum saturation

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();
        SetInitialSaturation();
    }

    private void SetInitialSaturation()
    {
        currentSaturationAmount = midSaturation; // Initialize with mid saturation
        ApplySaturationToAllRenderers(currentSaturationAmount);
    }

    private void ApplySaturationToAllRenderers(float saturationValue)
    {
        foreach (var rend in renderers)
        {
            if (rend != null && rend.enabled)
            {
                rend.GetPropertyBlock(propBlock);
                propBlock.SetFloat(kSaturationKey, saturationValue);
                rend.SetPropertyBlock(propBlock);
            }
        }
    }

    public void LerpSaturationToMin()
    {
        StartLerping(minSaturation);
    }

    public void LerpSaturationToMid()
    {
        StartLerping(midSaturation);
    }

    public void LerpSaturationToMax()
    {
        StartLerping(maxSaturation);
    }

    private void StartLerping(float targetSaturation)
    {
        if (lerping)
        {
            // Already lerping or already at target value
            return;
        }

        // Stop the existing coroutine if it's running
        if (currentLerpCoroutine != null)
        {
            StopCoroutine(currentLerpCoroutine);
        }

        lerping = true;
        currentLerpCoroutine = StartCoroutine(LerpSaturation(targetSaturation));
    }

    private IEnumerator LerpSaturation(float targetSaturation)
    {
        float elapsedTime = 0f;
        float startValue = currentSaturationAmount;

        while (elapsedTime < saturationLerpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / saturationLerpDuration;
            currentSaturationAmount = Mathf.Lerp(startValue, targetSaturation, t);
            ApplySaturationToAllRenderers(currentSaturationAmount);

            yield return null;
        }

        currentSaturationAmount = targetSaturation;
        lerping = false;
    }
}

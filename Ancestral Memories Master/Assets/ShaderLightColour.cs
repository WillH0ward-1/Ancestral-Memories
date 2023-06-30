#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class ShaderLightColor : MonoBehaviour
{
    private MeshRenderer rend;
    private MaterialPropertyBlock propBlock;
    public TimeCycleManager timeCycleManager;

    private void OnEnable()
    {
        timeCycleManager = FindObjectOfType<TimeCycleManager>();
        rend = GetComponent<MeshRenderer>();
        propBlock = new MaterialPropertyBlock();

#if UNITY_EDITOR
        EditorApplication.update += Update;
#else
        StartCoroutine(UpdateLightColorContinuously());
#endif
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.update -= Update;
#endif
    }

    private void Update()
    {
        UpdateLightColor();
    }

private void UpdateLightColor()
{
    float timePercent = timeCycleManager.timeOfDay / 24f;

    int currentColorIndex = Mathf.FloorToInt(timePercent * (timeCycleManager.timeColors.Length - 1)) % timeCycleManager.timeColors.Length;
    int nextColorIndex = (currentColorIndex + 1) % timeCycleManager.timeColors.Length;

    float t = Mathf.InverseLerp(currentColorIndex / (float)(timeCycleManager.timeColors.Length - 1), (currentColorIndex + 1) / (float)(timeCycleManager.timeColors.Length - 1), timePercent);

    Color currentSkyColor = timeCycleManager.timeColors[currentColorIndex].skyColor;
    Color nextSkyColor = timeCycleManager.timeColors[nextColorIndex].skyColor;
    Color currentLightColor = timeCycleManager.timeColors[currentColorIndex].lightColor;
    Color nextLightColor = timeCycleManager.timeColors[nextColorIndex].lightColor;

    Color lerpedSkyColor;
    Color lerpedLightColor;

    if (nextColorIndex == 0 && timePercent < (1f / timeCycleManager.timeColors.Length))
    {
        // Handle seamless transition from last colors to first colors
        Color lastSkyColor = timeCycleManager.timeColors[timeCycleManager.timeColors.Length - 1].skyColor;
        Color lastLightColor = timeCycleManager.timeColors[timeCycleManager.timeColors.Length - 1].lightColor;

        lerpedSkyColor = Color.Lerp(lastSkyColor, nextSkyColor, t);
        lerpedLightColor = Color.Lerp(lastLightColor, nextLightColor, t);
    }
    else
    {
        lerpedSkyColor = Color.Lerp(currentSkyColor, nextSkyColor, t);
        lerpedLightColor = Color.Lerp(currentLightColor, nextLightColor, t);
    }

    rend.GetPropertyBlock(propBlock);
    propBlock.SetColor("_SkyColour", lerpedSkyColor);
    propBlock.SetColor("_LightColor", lerpedLightColor);
    rend.SetPropertyBlock(propBlock);
}


    private IEnumerator UpdateLightColorContinuously()
    {
        while (timeCycleManager != null)
        {
            UpdateLightColor();
            yield return new WaitForSeconds(0.1f);
        }
    }
}

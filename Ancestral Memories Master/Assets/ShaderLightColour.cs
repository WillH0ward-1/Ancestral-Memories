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
        if (timeCycleManager != null)
        {
            UpdateLightColor();
        }
    }

    private void UpdateLightColor()
    {
        if (rend == null || !rend.enabled)
        {
            // Renderer is missing or disabled, so skip updating
            return;
        }

        float timePercent = timeCycleManager.TimeOfDay / 24f;

        int currentColorIndex = Mathf.FloorToInt(timePercent * timeCycleManager.timeColors.Length) % timeCycleManager.timeColors.Length;
        int nextColorIndex = (currentColorIndex + 1) % timeCycleManager.timeColors.Length;

        float t = Mathf.InverseLerp(currentColorIndex / (float)timeCycleManager.timeColors.Length, (currentColorIndex + 1) / (float)timeCycleManager.timeColors.Length, timePercent);

        Color currentSkyColor = timeCycleManager.timeColors[currentColorIndex].skyColor;
        Color nextSkyColor = timeCycleManager.timeColors[nextColorIndex].skyColor;
        Color currentLightColor = timeCycleManager.timeColors[currentColorIndex].lightColor;
        Color nextLightColor = timeCycleManager.timeColors[nextColorIndex].lightColor;

        Color lerpedSkyColor;
        Color lerpedLightColor;

        if (nextColorIndex == 0 && timePercent < (1f / timeCycleManager.timeColors.Length))
        {
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

using UnityEngine;

[ExecuteInEditMode]
public class SkyColourController : MonoBehaviour
{
    [System.Serializable]
    public class TimeColor
    {
        public Color color;
    }

    [SerializeField] private TimeColor[] timeColors;
    [SerializeField] private float colorChangeSpeed = 1f;
    private Material material;
    [Range(0, 24)]
    public float gameHours = 0f;
    public bool updateInEditor = true;

    private float startTime;

    private void OnEnable()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            material = renderer.sharedMaterial;
        }
        else
        {
            Debug.LogError("No Renderer found on this GameObject or its children.");
        }
    }

    private void Update()
    {
        if (Application.isPlaying || updateInEditor)
        {
            float elapsedTime;
            if (!Application.isPlaying)
            {
                elapsedTime = Time.realtimeSinceStartup - startTime;
            }
            else
            {
                elapsedTime = Time.time - startTime;
            }

            gameHours = (elapsedTime * colorChangeSpeed) % 24f;

            UpdateSkyColour();
        }
    }

    private void OnValidate()
    {
        UpdateSkyColour();
    }

    private void UpdateSkyColour()
    {
        int currentColorIndex = Mathf.FloorToInt((gameHours / 24f) * (timeColors.Length - 1));
        int nextColorIndex = (currentColorIndex + 1) % timeColors.Length;

        Color currentColor = timeColors[currentColorIndex].color;
        Color nextColor = timeColors[nextColorIndex].color;
        float t = Mathf.InverseLerp(currentColorIndex / (float)(timeColors.Length - 1), (currentColorIndex + 1) / (float)(timeColors.Length - 1), gameHours / 24f);
        Color lerpedColor = Color.Lerp(currentColor, nextColor, t);

        material.SetColor("_SkyColour", lerpedColor);

        if (!Application.isPlaying)
        {
            DynamicGI.UpdateEnvironment();
        }
    }
}

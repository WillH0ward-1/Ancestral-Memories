using FMODUnity;
using UnityEngine;

[ExecuteAlways]
public class TimeCycleManager : MonoBehaviour
{
    // TimeColor definition
    [System.Serializable]
    public class TimeColor
    {
        public Color color;
    }

    // Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private TimeColor[] timeColors;

    // Variables
    [SerializeField, Range(0, 24)] public float timeOfDay;
    public float timeMultiplier = 0.25f;
    public float defaultTimeMultiplier = 0.25f;
    public bool isNightTime; // Night time flag
    public bool updateInEditor = true; // Update in editor flag
    public GameObject skyBox;

    private Material material; // This will now be automatically assigned
    private float lastRealTime;


    private void Awake()
    {
        defaultTimeMultiplier = 0.25f;
        timeMultiplier = defaultTimeMultiplier;
        Renderer renderer = skyBox.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            material = renderer.sharedMaterial;
        }
        else
        {
            Debug.LogError("No Renderer found on this GameObject or its children.");
        }

        lastRealTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if (Application.isPlaying || updateInEditor)
        {
            UpdateTimeAndLight();
        }
    }

    private void OnValidate()
    {
        if (updateInEditor)
        {
            UpdateTimeAndLight();
        }
    }

    private void UpdateTimeAndLight()
    {
        if (Preset == null || material == null)
            return;

        float timeDelta = Application.isPlaying
            ? Time.deltaTime
            : (Time.realtimeSinceStartup - lastRealTime);

        lastRealTime = Time.realtimeSinceStartup;

        //(Replace with a reference to the game time)
        timeOfDay += timeDelta * timeMultiplier;
        timeOfDay %= 24; //Modulus to ensure always between 0-24

        isNightTime = timeOfDay < 6 || timeOfDay >= 18; // Define night time hours here

        UpdateLight(timeOfDay / 24f);

        if (Application.isPlaying)
        {
            RuntimeManager.StudioSystem.setParameterByName("TimeOfDay", timeOfDay);
        }
    }


    private void UpdateLight(float timePercent)
    {
        // Update sky color
        int currentColorIndex = Mathf.FloorToInt(timePercent * (timeColors.Length - 1));
        int nextColorIndex = (currentColorIndex + 1) % timeColors.Length;

        Color currentColor = timeColors[currentColorIndex].color;
        Color nextColor = timeColors[nextColorIndex].color;
        float t = Mathf.InverseLerp(currentColorIndex / (float)(timeColors.Length - 1), (currentColorIndex + 1) / (float)(timeColors.Length - 1), timePercent);
        Color lerpedColor = Color.Lerp(currentColor, nextColor, t);

        material.SetColor("_SkyColour", lerpedColor);

        // Set ambient and fog
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        // If the directional light is set then rotate and set its color
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);

            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

        if (!Application.isEditor)
        {
            var fmodTod = FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TimeOfDay", timeOfDay);
            Debug.Log("time of day (Fmod) = " + fmodTod);
        }
    }

    // Try to find a directional light to use if we haven't set one
    private void OnEnable()
    {
        if (DirectionalLight != null)
            return;

        // Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        // Search scene for light that fits criteria (directional)
        else
        {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

[ExecuteAlways]
public class TimeCycleManager : MonoBehaviour
{
    // TimeColor definition
    [System.Serializable]
    public class TimeColor
    {
        public Color skyColor;
        public Color lightColor;
    }

    // Scene References
    [SerializeField] private Light DirectionalLight;
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

    public MapObjGen mapObjGen;

    private List<GameObject> GetTreeList()
    {
        if (mapObjGen != null)
        {
            return mapObjGen.treeList;
        }
        else
        {
            Debug.LogError("MapObjGen has not been assigned.");
            return null;
        }
    }

    private void Start()
    {
        if (!Application.isEditor)
        {
            StartCoroutine(WaitUntilMapObjectsGenerated());
        }
    }

    private IEnumerator WaitUntilMapObjectsGenerated()
    {
        while (!mapObjGen.mapObjectsGenerated)
        {
            yield return null; // Wait for one frame
        }

        StartCoroutine(UpdateLightColorContinuously());
    }

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
        if (material == null)
            return;

        float timeDelta = Application.isPlaying
            ? Time.deltaTime
            : (Time.realtimeSinceStartup - lastRealTime);

        lastRealTime = Time.realtimeSinceStartup;

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

        Color currentSkyColor = timeColors[currentColorIndex].skyColor;
        Color nextSkyColor = timeColors[nextColorIndex].skyColor;
        Color currentLightColor = timeColors[currentColorIndex].lightColor;
        Color nextLightColor = timeColors[nextColorIndex].lightColor;

        float t = Mathf.InverseLerp(currentColorIndex / (float)(timeColors.Length - 1), (currentColorIndex + 1) / (float)(timeColors.Length - 1), timePercent);

        Color lerpedSkyColor = Color.Lerp(currentSkyColor, nextSkyColor, t);
        Color lerpedLightColor = Color.Lerp(currentLightColor, nextLightColor, t);

        material.SetColor("_SkyColour", lerpedSkyColor);

        if (DirectionalLight != null)
        {
            RenderSettings.ambientLight = lerpedLightColor;
            RenderSettings.fogColor = lerpedLightColor;
            DirectionalLight.color = lerpedLightColor;
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }

        if (!Application.isEditor)
        {
            var fmodTod = FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TimeOfDay", timeOfDay);
            Debug.Log("time of day (Fmod) = " + fmodTod);
        }
    }

    private IEnumerator UpdateLightColorContinuously()
    {
        while (true)
        {
            var treeList = GetTreeList();
            if (treeList != null) // Check if tree list is not null
            {
                foreach (GameObject tree in treeList)
                {
                    ShaderLightColor treeLightColor = tree.GetComponentInChildren<ShaderLightColor>();

                    if (treeLightColor != null)
                    {
                        int currentColorIndex = Mathf.FloorToInt(timeOfDay * (timeColors.Length - 1));
                        int nextColorIndex = (currentColorIndex + 1) % timeColors.Length;

                        Color currentLightColor = timeColors[currentColorIndex].lightColor;
                        Color nextLightColor = timeColors[nextColorIndex].lightColor;

                        float t = Mathf.InverseLerp(currentColorIndex / (float)(timeColors.Length - 1), (currentColorIndex + 1) / (float)(timeColors.Length - 1), timeOfDay);

                        Color lerpedLightColor = Color.Lerp(currentLightColor, nextLightColor, t);

                        treeLightColor.UpdateLightColor(lerpedLightColor);
                    }
                }
            }

            yield return new WaitForSeconds(0.1f); // Set the interval of updating color here
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
                
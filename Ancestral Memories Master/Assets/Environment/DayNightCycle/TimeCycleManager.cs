﻿using System.Collections;
using System.Collections.Generic;
//using FMODUnity;
using UnityEngine;

[ExecuteAlways]
public class TimeCycleManager : MonoBehaviour
{
    [System.Serializable]
    public class TimeColor
    {
        public Color skyColor;
        public Color lightColor;
    }

    [SerializeField] private Light DirectionalLight;
    public TimeColor[] timeColors;
    [SerializeField, Range(0, 24)] private float _timeOfDay;
    public float timeMultiplier = 0.25f;
    public bool isNightTime;
    public bool updateInEditor = true;
    public GameObject skyBox;
    public MapObjGen mapObjGen;

    [SerializeField, Range(0, 365)] private int _dayOfYear; // Current day of the year
    public int daysPerSeason = 90;
    [SerializeField] private SeasonManager seasonManager;

     public float defaultTimeMultiplier = 0.25f;
    [SerializeField] private float editorTimeMultiplier = 0.8f;

    public int DayOfYear
    {
        get { return _dayOfYear; }
        set
        {
            _dayOfYear = value % seasonManager.GetTotalDaysInYear(); // Use the SeasonManager to get the total days
            // We only change the dayOfYear and let the TimeCycleManager continue from the start of the new day
        }
    }

    public float TimeOfDay
    {
        get { return _timeOfDay; }
        set
        {
            _timeOfDay = value % 24;
            if (_timeOfDay < 0)
            {
                _timeOfDay += 24;
            }
        }
    }

    public int CurrentDay
    {
        get { return DayOfYear % 30 + 1; } // Days are 1-based
    }

    public int CurrentMonth
    {
        get { return DayOfYear / 30 + 1; } // Months are 1-based
    }

    public int CurrentYear
    {
        get { return DayOfYear / 360; } // Assuming 12 months of 30 days each
    }

    private Material material;
    private float lastRealTime;

    private int GetNumberOfSeasons()
    {
        return System.Enum.GetValues(typeof(SeasonManager.Season)).Length;
    }

    private void Awake()
    {
        seasonManager = GetComponent<SeasonManager>();

        seasonManager.InitTime();

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
        _dayOfYear = Mathf.RoundToInt(TimeOfDay / 24f * seasonManager.GetTotalDaysInYear());
    }

    private void LateUpdate()
    {
        if (Application.isPlaying || updateInEditor)
        {
            float previousTime = TimeOfDay;
            UpdateTimeAndLight();
            if (TimeOfDay < previousTime)
            {
                DayOfYear = (DayOfYear + 1) % (daysPerSeason * GetNumberOfSeasons());
            }
        }
    }


    private void OnValidate()
    {
        if (updateInEditor)
        {
            UpdateTimeAndLight();
        }
    }

    private bool hasFocus = true;  // Add this variable to keep track of application focus

    void OnApplicationFocus(bool focus)
    {
        hasFocus = focus;
        if (focus)
        {
            lastRealTime = Time.realtimeSinceStartup;  // Reset the lastRealTime when focus is regained
        }
    }

    private void UpdateTimeAndLight()
    {
        if (!hasFocus || material == null)
        {
            return;  // Return early if the application doesn't have focus or material is null
        }

        float timeDelta = Application.isPlaying
            ? Time.deltaTime
            : Mathf.Min(0.1f, Time.realtimeSinceStartup - lastRealTime);  // Cap the timeDelta to prevent overload

        lastRealTime = Time.realtimeSinceStartup;

        TimeOfDay += timeDelta * timeMultiplier;
        TimeOfDay %= 24;

        isNightTime = TimeOfDay < 6 || TimeOfDay >= 18;

        UpdateLight(TimeOfDay / 24f);

        if (Application.isPlaying)
        {
            //RuntimeManager.StudioSystem.setParameterByName("TimeOfDay", TimeOfDay);
        }
    }


    private void UpdateLight(float timePercent)
    {
        // Update sky color
        int currentColorIndex = Mathf.FloorToInt(timePercent * timeColors.Length);
        int nextColorIndex = (currentColorIndex + 1) % timeColors.Length;

        Color currentSkyColor = timeColors[currentColorIndex].skyColor;
        Color nextSkyColor = timeColors[nextColorIndex].skyColor;
        Color currentLightColor = timeColors[currentColorIndex].lightColor;
        Color nextLightColor = timeColors[nextColorIndex].lightColor;

        float t = Mathf.InverseLerp(currentColorIndex / (float)timeColors.Length, (currentColorIndex + 1) / (float)timeColors.Length, timePercent);

        Color lerpedSkyColor;
        Color lerpedLightColor;

        if (nextColorIndex == 0 && timePercent < (1f / timeColors.Length))
        {
            // Handle seamless transition from last colors to first colors
            Color lastSkyColor = timeColors[timeColors.Length - 1].skyColor;
            Color lastLightColor = timeColors[timeColors.Length - 1].lightColor;

            lerpedSkyColor = Color.Lerp(lastSkyColor, nextSkyColor, t);
            lerpedLightColor = Color.Lerp(lastLightColor, nextLightColor, t);
        }
        else
        {
            lerpedSkyColor = Color.Lerp(currentSkyColor, nextSkyColor, t);
            lerpedLightColor = Color.Lerp(currentLightColor, nextLightColor, t);
        }

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
           // var fmodTod = FMODUnity.RuntimeManager.StudioSystem.setParameterByName("TimeOfDay", TimeOfDay);
           // Debug.Log("time of day (Fmod) = " + fmodTod);

        }
    }

    // Try to find a directional light to use if we haven't set one
    private void OnEnable()
    {
        seasonManager = transform.GetComponent<SeasonManager>();

        if (Application.isEditor)
        {
            timeMultiplier = editorTimeMultiplier;
        } else
        {
            timeMultiplier = defaultTimeMultiplier;
        }

        if (seasonManager == null)
        {
            Debug.LogError("(TIME MANAGER) SeasonManager component not found: See hierarchy for TIME MANAGER.");
            return; // Optionally, you could return here to prevent further execution if seasonManager is essential
        }

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

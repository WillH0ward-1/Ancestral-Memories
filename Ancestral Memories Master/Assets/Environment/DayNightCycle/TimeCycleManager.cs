using FMODUnity;
using UnityEngine;

[ExecuteAlways]
public class TimeCycleManager : MonoBehaviour
{
    //Scene References
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    //Variables
    [SerializeField, Range(0, 24)] public float timeOfDay;
    public float timeMultiplier = 0.25f;
    public float defaultTimeMultiplier = 0.25f;

    public bool isNightTime;

    [SerializeField] private NightSwitch nightSwitch;

    private void Awake()
    {
        defaultTimeMultiplier = 0.25f;
        timeMultiplier = defaultTimeMultiplier;
    }

    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            //(Replace with a reference to the game time)
            timeOfDay += Time.deltaTime * timeMultiplier;
            timeOfDay %= 24; //Modulus to ensure always between 0-24
            UpdateLight(timeOfDay / 24f);
        }
        else
        {
            UpdateLight(timeOfDay / 24f);
        }

        RuntimeManager.StudioSystem.setParameterByName("TimeOfDay", timeOfDay);
    }

    [SerializeField] private float nightThresholdMin = 6f;
    [SerializeField] private float nightThresholdMax = 22f;

    private void UpdateLight(float timePercent)
    {
        /*
        if (timeOfDay <= nightThresholdMin || timeOfDay >= nightThresholdMax)
        {
            isNightTime = true;
            if (!nightSwitch.nightTime)
            {
                nightSwitch.StartCoroutine(nightSwitch.ToNightSky());
            }
        } else
        {
            isNightTime = false;
            if (!nightSwitch.dayTime)
            {
                nightSwitch.StartCoroutine(nightSwitch.ToDaySky());
            }
        }
        */

        //Set ambient and fog
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);

        //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
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



    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight != null)
            return;

        //Search for lighting tab sun
        if (RenderSettings.sun != null)
        {
            DirectionalLight = RenderSettings.sun;
        }
        //Search scene for light that fits criteria (directional)
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

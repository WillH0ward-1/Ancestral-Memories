using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CanvasFX : MonoBehaviour
{
    [SerializeField] private Material heatDistortionMaterial;
    [SerializeField] private Image image;

    public TimeCycleManager timeCycleManager;
    public SeasonManager seasonMangager;

    [SerializeField] private float heatDistortionValue = 0f;
    [SerializeField] private float sunIntensityThreshold = 0.7f;

    [SerializeField] private float springHeatTarget = 0.01f;
    [SerializeField] private float summerHeatTarget = 0.025f;
    [SerializeField] private float autumnHeatTarget = 0f;
    [SerializeField] private float winterHeatTarget = 0f;

    // The min and max values for the HeatDistortion parameter
    [SerializeField] private const float MinHeatDistortion = 0f;
    [SerializeField] private float MaxHeatDistortion = 0.025f;

    private float targetHeatDistortion = 0f;
    private float lerpDuration = 1f; // Duration of the lerp transition
    private float lerpStartTime;

    private void OnEnable()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            heatDistortionMaterial = image.material;
        }
        else
        {
            Debug.LogError("Image component not found.");
        }

        timeCycleManager = FindObjectOfType<TimeCycleManager>();
        seasonMangager = timeCycleManager.seasonManager;

        summerHeatTarget = MaxHeatDistortion;
        winterHeatTarget = MinHeatDistortion;

        seasonMangager.OnSeasonChanged.AddListener(OnSeasonChange);
    }

    private void OnDisable()
    {
        if (seasonMangager != null)
        {
            seasonMangager.OnSeasonChanged.RemoveListener(OnSeasonChange);
        }
    }

    public void OnSeasonChange(SeasonManager.Season season)
    {
        switch (season)
        {
            case SeasonManager.Season.Spring:
                targetHeatDistortion = springHeatTarget;
                break;
            case SeasonManager.Season.Summer:
                targetHeatDistortion = summerHeatTarget;
                break;
            case SeasonManager.Season.Autumn:
                targetHeatDistortion = autumnHeatTarget;
                break;
            case SeasonManager.Season.Winter:
                targetHeatDistortion = winterHeatTarget;
                break;
        }
        lerpStartTime = Time.time; // Reset lerp start time
    }

    private void Update()
    {
        if (timeCycleManager != null && timeCycleManager.sunIntensity > sunIntensityThreshold)
        {
            LerpHeatDistortion(MaxHeatDistortion);
        }
        else if (heatDistortionValue > MinHeatDistortion)
        {
            LerpHeatDistortion(MinHeatDistortion);
        }

        ApplyHeatDistortion();
    }

    private void LerpHeatDistortion(float target)
    {
        targetHeatDistortion = target;
        lerpStartTime = Time.time; // Reset lerp start time
    }

    private void ApplyHeatDistortion()
    {
        float timeSinceStarted = Time.time - lerpStartTime;
        float percentageComplete = timeSinceStarted / lerpDuration;

        heatDistortionValue = Mathf.Lerp(heatDistortionValue, targetHeatDistortion, percentageComplete);
        heatDistortionMaterial.SetFloat("HeatDistortion_", heatDistortionValue);

        if (image != null)
        {
            image.material = heatDistortionMaterial;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteAlways]
public class LerpTerrain : MonoBehaviour
{
    private Player player;
    public float Desert = 0f;
    public float Oasis = 12f;
    public float Wet = 22f;

    [SerializeField] List<Renderer> rendererList = new List<Renderer>();

    [SerializeField] private float duration = 15f;
    [SerializeField] private float killDuration = 10f;
    [SerializeField] private float reviveDuration = 12f;

    [SerializeField] private string VertexTileParam = "_VertexTile";
    [SerializeField] private string SnowDensityParam = "_SnowDensity";
    [SerializeField] private string AutumnColourParam = "_AutumnColour";

    private float currentSnowHeight;
    private float minSnowHeight = 0.95f;
    private float maxSnowHeight = 1.2f;

    private SeasonManager seasonManager; // Reference to the SeasonManager script

    private Coroutine autumnLerpCoroutine;
    private Coroutine snowLerpCoroutine;
    private Coroutine vertexLerpCoroutine;

    [SerializeField] private string SnowHeightParam = "_SnowHeight";  // Name of the shader property for snow height
    [SerializeField] private float growSnowDuration = 5f;  // Duration to fully grow snow
    [SerializeField] private float meltSnowDuration = 5f;  // Duration to fully melt snow
    private Coroutine snowCoroutine;
    private float snowMeltDelay = 5f; // Time buffer before snow starts melting after rain stops

    private Coroutine growSnowCoroutine;
    private Coroutine meltSnowCoroutine;

    public RainControl rainControl;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        seasonManager = FindObjectOfType<SeasonManager>();
        seasonManager.OnSeasonChanged.AddListener(HandleSeasonChange);

        GetRenderers();
        InitSnowHeight();
        ApplyImmediateSeasonState(seasonManager.initSeason);
    }

    private void InitSnowHeight()
    {
        foreach (Renderer r in rendererList)
        {
            r.sharedMaterial.SetFloat(SnowHeightParam, minSnowHeight);
        }
    }

    void HandleSeasonChange(SeasonManager.Season newSeason)
    {
        ApplyImmediateSeasonState(newSeason);

        // Melt snow if the new season is not Winter
        if (newSeason != SeasonManager.Season.Winter)
        {
            MeltSnow();
        }
    }

    private float minVertexTile = 0;
    private float maxVertexTile = 1;

    private void Update()
    {
        if (seasonManager._currentSeason == SeasonManager.Season.Autumn)
        {
            if (autumnLerpCoroutine == null)
            {
                autumnLerpCoroutine = StartCoroutine(LerpAutumnColor(player.faith < player.maxStat / 2 ? 0f : 1f));
            }
        }

        else if (seasonManager._currentSeason == SeasonManager.Season.Winter)
        {
            if (snowLerpCoroutine == null)
            {
                snowLerpCoroutine = StartCoroutine(LerpSnowDensity(maxVertexTile));
            }
        }

        else // Not Autumn or Winter (Spring/Summer)
        {
            if (autumnLerpCoroutine == null)
            {
                autumnLerpCoroutine = StartCoroutine(LerpAutumnColor(minVertexTile));
            }
            if (snowLerpCoroutine == null && CheckHeight(currentSnowHeight));
            {
                snowLerpCoroutine = StartCoroutine(LerpSnowDensity(minVertexTile));
            }
        }

        HandleSnowBehavior();
    }

    private bool CheckHeight(float currentSnowHeight)
    {
        bool validHeight = Mathf.Approximately(minSnowHeight, currentSnowHeight);

        return validHeight;
    }

    void ApplyImmediateSeasonState(SeasonManager.Season season)
    {
        if (vertexLerpCoroutine != null)
        {
            StopCoroutine(vertexLerpCoroutine);
            vertexLerpCoroutine = null;
        }

        switch (season)
        {
            case SeasonManager.Season.Summer:
                ToOasis();
                break;
            case SeasonManager.Season.Autumn:
                ToOasis();
                break;
            case SeasonManager.Season.Winter:
                ToWetOasis();
                break;
            case SeasonManager.Season.Spring:
                ToOasis();
                break;
            default:
                // Handle default case
                break;
        }
    }

    public void ToDesert()
    {
        if (seasonManager._currentSeason != SeasonManager.Season.Winter)
        {
            ChangeState(Desert);
        }
    }

    public void ToOasis()
    {
        ChangeState(Oasis);
    }

    public void ToWetOasis()
    {
        ChangeState(Wet);
    }

    void ChangeState(float newState)
    {
        if (vertexLerpCoroutine != null)
        {
            StopCoroutine(vertexLerpCoroutine);
        }
        vertexLerpCoroutine = StartCoroutine(LerpVertexTile(newState, duration));
    }

    IEnumerator LerpVertexTile(float targetState, float duration)
    {
        float time = 0f;

        while (time < 1f)
        {
            float lerpFactor = time / duration;
            foreach (Renderer r in rendererList)
            {
                Vector4 currentState = r.sharedMaterial.GetVector(VertexTileParam);
                currentState.y = Mathf.Lerp(currentState.y, targetState, lerpFactor);
                r.sharedMaterial.SetVector(VertexTileParam, currentState);
            }

            time += Time.deltaTime;
            yield return null;
        }

        vertexLerpCoroutine = null;

        yield break;
    }

    private IEnumerator LerpAutumnColor(float targetValue)
    {
        if (snowCoroutine == null)
        {
            float lerpDuration = targetValue == 0f ? killDuration : reviveDuration;
            if (rendererList.Count > 0)
            {
                float currentAutumnColour = rendererList[0].sharedMaterial.GetFloat(AutumnColourParam);
                float time = 0f;

                while (time < 1f)
                {
                    float lerpFactor = time / lerpDuration;
                    float newAutumnColour = Mathf.Lerp(currentAutumnColour, targetValue, lerpFactor);
                    foreach (Renderer r in rendererList)
                    {
                        r.sharedMaterial.SetFloat(AutumnColourParam, newAutumnColour);
                    }

                    time += Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                Debug.LogWarning("rendererList is empty in LerpAutumnColor");
            }

            autumnLerpCoroutine = null;
        }

        yield break;
    }


    private float currentSnowDensity;

    private IEnumerator LerpSnowDensity(float targetValue)
    {
        float lerpDuration = targetValue == minVertexTile ? killDuration : reviveDuration;

        // Ensure there are elements in the list before accessing
        if (rendererList.Count > 0)
        {
            currentSnowDensity = rendererList[0].sharedMaterial.GetFloat(SnowDensityParam);
            float time = 0f;

            while (time < 1f)
            {
                float lerpFactor = time / lerpDuration;
                float newSnowDensity = Mathf.Lerp(currentSnowDensity, targetValue, lerpFactor);
                foreach (Renderer r in rendererList)
                {
                    r.sharedMaterial.SetFloat(SnowDensityParam, newSnowDensity);
                }

                time += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("rendererList is empty in LerpSnowDensity");
        }

        snowLerpCoroutine = null;

        yield break;
    }

    private void HandleSnowBehavior()
    {
        if (seasonManager._currentSeason == SeasonManager.Season.Winter)
        {
            if (rainControl.isRaining && snowCoroutine == null)
            {
                // Start growing snow if it's winter and raining
                GrowSnow();
            }
            else if (!rainControl.isRaining && snowCoroutine == null)
            {
                // Start a delayed snow melting coroutine after rain stops
                StartCoroutine(DelayedSnowMelt());
            }
        }
        else if (snowCoroutine == null)
        {
            // Melt snow if it's not winter
            MeltSnow();
        }
    }

    private IEnumerator DelayedSnowMelt()
    {
        yield return new WaitForSeconds(snowMeltDelay);
        // Check again to ensure it's still not raining after the delay
        if (!rainControl.isRaining)
        {
            MeltSnow();
        }

        yield break;
    }


    private float GetCurrentSnowHeight()
    {
        // Assuming rendererList is not empty and all renderers have the same snow height
        if (rendererList.Count > 0)
        {
            return rendererList[0].sharedMaterial.GetFloat(SnowHeightParam);
        }
        return 0f; // Default value if rendererList is empty
    }

    public void GrowSnow()
    {
        if (currentSnowDensity == maxVertexTile)
        {
            currentSnowHeight = GetCurrentSnowHeight();
            if (currentSnowHeight < maxSnowHeight)
            {
                StartSnowCoroutine(currentSnowHeight, maxSnowHeight, growSnowDuration);
            }
        }
    }

    public void MeltSnow()
    {
        float currentSnowHeight = GetCurrentSnowHeight();
        if (currentSnowHeight > minSnowHeight)
        {
            StartSnowCoroutine(currentSnowHeight, minSnowHeight, meltSnowDuration);
        }
    }

    private void StartSnowCoroutine(float startValue, float endValue, float duration)
    {
        if (snowCoroutine != null)
        {
            StopCoroutine(snowCoroutine);
        }
        snowCoroutine = StartCoroutine(LerpSnowHeight(startValue, endValue, duration));
    }

    private IEnumerator LerpSnowHeight(float startValue, float endValue, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            float currentSnowHeight = Mathf.Lerp(startValue, endValue, time / duration);
            SetSnowHeight(currentSnowHeight);

            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the final value is set at the end of the duration
        SetSnowHeight(endValue);
        snowCoroutine = null;

        yield break;
    }

    private void SetSnowHeight(float height)
    {
        foreach (Renderer r in rendererList)
        {
            r.sharedMaterial.SetFloat(SnowHeightParam, height);
        }
    }





    void OnDestroy()
    {
        seasonManager.OnSeasonChanged.RemoveListener(HandleSeasonChange);
    }

    void GetRenderers()
    {
        Renderer[] objectRenderers = transform.GetComponentsInChildren<Renderer>();
        rendererList = objectRenderers.ToList();
    }
}

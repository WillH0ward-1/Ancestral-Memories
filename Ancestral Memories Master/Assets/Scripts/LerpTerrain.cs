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
    [SerializeField] private float killDuration = 3f;
    [SerializeField] private float reviveDuration = 12f;

    [SerializeField] private float currentState;

    private SeasonManager seasonManager; // Reference to the SeasonManager script

    [SerializeField] private string VertexTileParam = "_VertexTile";
    [SerializeField] private string SnowDensityParam = "_SnowDensity";
    [SerializeField] private string AutumnColourParam = "_AutumnColour";

    private RainControl raincontrol;

    private bool isLerpingAutumnColor;
    private bool isLerpingSnowDensity;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        raincontrol = FindObjectOfType<RainControl>();
        seasonManager = FindObjectOfType<SeasonManager>();
        seasonManager.OnSeasonChanged.AddListener(HandleSeasonChange);
        seasonManager = FindObjectOfType<SeasonManager>();
    }


    void Start()
    {
        GetRenderers();
        ToState(Oasis, 0.1f);
    }

    private void Update()
    {
        if (seasonManager._currentSeason == SeasonManager.Season.Autumn)
        {
            LerpAutumnColor(player.faith < player.maxStat / 2 ? 0f : 1f);
        }
        else if (seasonManager._currentSeason == SeasonManager.Season.Winter)
        {
            LerpSnowDensity(1f);
        }
        else
        {
            LerpAutumnColor(0f);
            LerpSnowDensity(0f);
        }
    }

    private void LerpAutumnColor(float targetValue)
    {
        float duration = targetValue == 0f ? killDuration : reviveDuration;

        foreach (Renderer r in rendererList)
        {
            float currentAutumnColour = r.sharedMaterial.GetFloat(AutumnColourParam);
            float newAutumnColour = Mathf.Lerp(currentAutumnColour, targetValue, Time.deltaTime / duration);
            r.sharedMaterial.SetFloat(AutumnColourParam, newAutumnColour);
        }
    }

    private void LerpSnowDensity(float targetValue)
    {
        float duration = targetValue == 0f ? killDuration : reviveDuration;

        foreach (Renderer r in rendererList)
        {
            float currentSnowDensity = r.sharedMaterial.GetFloat(SnowDensityParam);
            float newSnowDensity = Mathf.Lerp(currentSnowDensity, targetValue, Time.deltaTime / duration);
            r.sharedMaterial.SetFloat(SnowDensityParam, newSnowDensity);
        }
    }



    void HandleSeasonChange(SeasonManager.Season newSeason)
    {
        if (isSeasonOverride)
        {
            isSeasonOverride = false;
        }
    }

    void OnDestroy()
    {
        seasonManager.OnSeasonChanged.RemoveListener(HandleSeasonChange);
    }

    IEnumerator SetTerrainState(float newState)
    {
        foreach (Renderer r in rendererList)
        {
            newState = r.sharedMaterial.GetVector(VertexTileParam).y;
            r.sharedMaterial.SetVector(VertexTileParam, new Vector4(0, newState, 0, 0));
            yield return null;
        }
    }

    void GetRenderers()
    {
        Renderer[] objectRenderers = transform.GetComponentsInChildren<Renderer>();
        rendererList = objectRenderers.ToList();
    }

    public IEnumerator ToOasis(float duration)
    {
        ToState(Oasis, duration);
        yield break;
    }

    public IEnumerator ToWetOasis(float duration)
    {
        ToState(Wet, duration);
        yield break;
    }

    public IEnumerator ToDesert(float duration)
    {
        if (seasonManager._currentSeason == SeasonManager.Season.Summer)
        {
            ToState(Desert, duration);
            yield break;
        }
    }

    void ToState(float newState, float duration)
    {
        StopAllCoroutines();

        if (newState != currentState)
        {
            currentState = newState;
            StartCoroutine(LerpVertexTile(newState, duration));
            return;
        }
    }

    private bool isVertexLerping;
    private bool isSeasonOverride;

    float state;

    private IEnumerator LerpVertexTile(float targetState, float duration)
    {
        float time = 0f;

        while (time <= 1f)
        {
            isVertexLerping = true;

            if (isVertexLerping == false)
            {
                yield break;
            }

            foreach (Renderer r in rendererList)
            {
                state = r.sharedMaterial.GetVector(VertexTileParam).y;
                float stateval = state;
                state = Mathf.Lerp(stateval, targetState, time);
                r.sharedMaterial.SetVector(VertexTileParam, new Vector4(0, state, 0, 0));
                time += Time.deltaTime / duration;
                yield return null;
            }

            yield return null;
        }

        if (time >= 1f)
        {
            isVertexLerping = false;
            yield break;
        }
    }
}

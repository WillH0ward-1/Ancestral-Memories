using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentManager : MonoBehaviour
{
    public enum InstrumentType
    {
        ShellFlute
    }

    [SerializeField] private AICharacterStats aiStats;
    [SerializeField] private float faith;
    [SerializeField] private float minFaith;
    [SerializeField] private float maxFaith;

    private Vector3 originalScale;
    [SerializeField] private float maxScaleFactor = 2f; // Max scale factor, can be set in the Inspector


    public InstrumentType instrumentType;
    private Coroutine scaleCoroutine;

    private void Start()
    {
        originalScale = transform.localScale; // Store the original scale
    }

    public void InitInstrument(AICharacterStats stats)
    {
        aiStats = stats;
        if (stats != null)
        {
            faith = stats.faith;
            minFaith = stats.minStat;
            maxFaith = stats.maxStat;
            scaleCoroutine = StartCoroutine(ScaleInstrument(stats));
        }
        else
        {
            Debug.LogError("AICharacterStats not found!");
        }
    }

    IEnumerator ScaleInstrument(AICharacterStats stats)
    {
        while (true)
        {
            if (stats != null)
            {
                faith = stats.faith; // Update faith value
                float lerpFactor = Mathf.InverseLerp(minFaith, maxFaith, faith);
                Vector3 targetScale = originalScale * (1 + lerpFactor * (maxScaleFactor - 1)); // Scale between original and maxScaleFactor
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime);
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }
    }
}

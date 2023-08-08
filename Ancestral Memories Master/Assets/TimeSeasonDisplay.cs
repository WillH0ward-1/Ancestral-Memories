using System.Collections;
using TMPro;
using UnityEngine;

public class TimeSeasonDisplay : MonoBehaviour
{
    [SerializeField] private SeasonManager seasonManager;
    [SerializeField] private TextMeshProUGUI seasonText;
    [SerializeField] private float lerpDuration = 1f;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private Vector2 dilationRange = new Vector2(0f, 0.1f);
    [SerializeField] private Vector2 alphaRange = new Vector2(0f, 1f);

    private void Awake()
    {
        seasonManager = transform.GetComponent<SeasonManager>();
        seasonText = transform.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (seasonManager != null && seasonText != null)
        {
            // Update the display with the current season when the game starts
            UpdateSeasonDisplay(seasonManager.CurrentSeason);

            // Subscribe to season change event.
            seasonManager.OnSeasonChanged.AddListener(UpdateSeasonDisplay);
        }
    }

    public void UpdateSeasonDisplay(SeasonManager.Season season)
    {
        seasonText.text = season.ToString();
        StartCoroutine(DisplaySeasonChange());
    }

    private IEnumerator DisplaySeasonChange()
    {
        // Enable text
        seasonText.enabled = true;

        // Lerp dilate and alpha from min to max concurrently
        float timer = 0;
        while (timer <= lerpDuration)
        {
            float t = timer / lerpDuration;

            // Modify dilation
            float dilation = Mathf.Lerp(dilationRange.x, dilationRange.y, t);
            seasonText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilation);

            // Modify alpha
            float alpha = Mathf.Lerp(alphaRange.x, alphaRange.y, t);
            seasonText.color = new Color(seasonText.color.r, seasonText.color.g, seasonText.color.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        // Hold dilation and alpha at max for holdDuration
        yield return new WaitForSeconds(holdDuration);

        // Lerp dilate and alpha from max to min concurrently
        timer = 0;
        while (timer <= lerpDuration)
        {
            float t = timer / lerpDuration;

            // Modify dilation
            float dilation = Mathf.Lerp(dilationRange.y, dilationRange.x, t);
            seasonText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilation);

            // Modify alpha
            float alpha = Mathf.Lerp(alphaRange.y, alphaRange.x, t);
            seasonText.color = new Color(seasonText.color.r, seasonText.color.g, seasonText.color.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        // Disable text
        seasonText.enabled = false;
    }
}

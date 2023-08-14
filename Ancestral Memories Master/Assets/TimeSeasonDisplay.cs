using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class TimeSeasonDisplay : MonoBehaviour
{
    [SerializeField] private SeasonManager seasonManager;
    [SerializeField] private TimeCycleManager timeCycleManager;
    [SerializeField] private TextMeshProUGUI seasonText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI yearText; // New serialized field for year text
    [SerializeField] private float lerpDuration = 1f;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private Vector2 dilationRange = new Vector2(0f, 0.1f);
    [SerializeField] private Vector2 alphaRange = new Vector2(0f, 1f);

    private void Awake()
    {
        seasonManager = transform.GetComponent<SeasonManager>();
        timeCycleManager = transform.GetComponent<TimeCycleManager>();

        Transform seasonsTransform = transform.Find("UI: Seasons");
        Transform dateTransform = transform.Find("UI: Date");
        Transform yearTransform = transform.Find("UI: Year"); // Find the year transform

        if (seasonsTransform != null) { seasonText = seasonsTransform.GetComponentInChildren<TextMeshProUGUI>(); }
        if (dateTransform != null) { dateText = dateTransform.GetComponentInChildren<TextMeshProUGUI>(); }
        if (yearTransform != null) { yearText = yearTransform.GetComponentInChildren<TextMeshProUGUI>(); } // Get the year text component
    }

    private void Start()
    {
        if (seasonManager != null && seasonText != null)
        {
            UpdateSeasonDisplay(seasonManager.CurrentSeason);
            seasonManager.OnSeasonChanged.AddListener(UpdateSeasonDisplay);
        }

        // Initialize the date and year displays
        UpdateDateDisplay();
        UpdateYearDisplay();
    }


    private int lastMonth = -1; // Tracks the last month
    private int lastDay = -1; // Tracks the last day

    private void Update()
    {
        if (timeCycleManager != null && dateText != null)
        {
            // Only update if the day or month has changed
            if (lastMonth != timeCycleManager.CurrentMonth || lastDay != timeCycleManager.CurrentDay)
            {
                UpdateDateDisplay();
                lastMonth = timeCycleManager.CurrentMonth; // Update last month
                lastDay = timeCycleManager.CurrentDay; // Update last day
            }
        }
    }


    public void UpdateSeasonDisplay(SeasonManager.Season season)
    {
        seasonText.text = season.ToString();
        StartCoroutine(DisplayTextChange(seasonText));
    }

    private void UpdateDateDisplay()
    {
        int day = timeCycleManager.CurrentDay;
        string suffix = GetOrdinalSuffix(day);
        dateText.text = string.Format("{0} {1}{2}", GetMonthName(timeCycleManager.CurrentMonth), day, suffix);
        StartCoroutine(DisplayTextChange(dateText));
    }



    private void UpdateYearDisplay()
    {
        if (yearText != null)
        {
            yearText.text = "Year " + timeCycleManager.CurrentYear.ToString();
            StartCoroutine(DisplayTextChange(yearText));
        }
    }


    private string GetMonthName(int monthNumber)
    {
        return DateTimeFormatInfo.CurrentInfo.GetMonthName(monthNumber);
    }


    private IEnumerator DisplayTextChange(TextMeshProUGUI textMeshPro)
    {
        if (textMeshPro == null) yield break;

        // Enable text
        textMeshPro.enabled = true;

        // Lerp dilate and alpha from min to max concurrently
        float timer = 0;
        while (timer <= lerpDuration)
        {
            float t = timer / lerpDuration;

            // Modify dilation
            float dilation = Mathf.Lerp(dilationRange.x, dilationRange.y, t);
            textMeshPro.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilation);

            // Modify alpha
            float alpha = Mathf.Lerp(alphaRange.x, alphaRange.y, t);
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, alpha);

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
            textMeshPro.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilation);

            // Modify alpha
            float alpha = Mathf.Lerp(alphaRange.y, alphaRange.x, t);
            textMeshPro.color = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, alpha);

            timer += Time.deltaTime;
            yield return null;
        }

        // Disable text
        textMeshPro.enabled = false;
    }

    private string GetOrdinalSuffix(int number)
    {
        int lastTwoDigits = number % 100;
        if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
        {
            return "th";
        }

        return (number % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th",
        };
    }

}

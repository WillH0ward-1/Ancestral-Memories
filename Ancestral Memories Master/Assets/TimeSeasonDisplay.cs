using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class TimeSeasonDisplay : MonoBehaviour
{
    [SerializeField] private Canvas uiCanvas;
    [SerializeField] private TMP_FontAsset fontAsset;
    [SerializeField] private Sprite timeSprite;
    [SerializeField] private Sprite seasonSprite;
    [SerializeField] private Color timeColor = Color.white;
    [SerializeField] private Color seasonColor = Color.white;
    [SerializeField] private float spriteSize = 50f;
    [SerializeField] private int pathResolution = 100;

    private GameObject timeMarker;
    private GameObject seasonMarker;
    private GameObject pathDrawer;

    private TimeCycleManager timeCycleManager;
    private SeasonManager seasonManager;

    private Image timeImage;
    private Image seasonImage;

    private void OnEnable()
    {
        uiCanvas = CreateCanvas();
        timeCycleManager = GetComponent<TimeCycleManager>();
        seasonManager = GetComponent<SeasonManager>();

        if (timeMarker == null)
        {
            timeMarker = CreateUIElement("TimeMarker");
            timeImage = timeMarker.AddComponent<Image>();
            timeImage.sprite = timeSprite;
            timeImage.color = timeColor;
            timeMarker.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Centered position
        }

        if (seasonMarker == null)
        {
            seasonMarker = CreateUIElement("SeasonMarker");
            seasonImage = seasonMarker.AddComponent<Image>();
            seasonImage.sprite = seasonSprite;
            seasonImage.color = seasonColor;
            seasonMarker.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Centered position
        }

        if (pathDrawer == null)
        {
            pathDrawer = CreateUIElement("PathDrawer");
            LineRenderer lineRenderer = pathDrawer.AddComponent<LineRenderer>();
            lineRenderer.enabled = false; // Disable rendering of the line

            RectTransform rectTransform = pathDrawer.GetComponent<RectTransform>();
            rectTransform.pivot = new Vector2(0.5f, 0f); // Bottom center
            rectTransform.anchorMin = new Vector2(0.5f, 0f); // Bottom center
            rectTransform.anchorMax = new Vector2(0.5f, 0f); // Bottom center
        }
    }

    private void Update()
    {
        if (timeCycleManager != null)
        {
            // Set the desired value on the time marker sprite
            // Example: timeImage.fillAmount = timeCycleManager.timeOfDay;
        }

        if (seasonManager != null)
        {
            // Set the desired value on the season marker sprite
            // Example: seasonImage.fillAmount = seasonManager.currentSeason;
        }

        // Animate the GUI elements along the curved path
        float timeAnimationDuration = 10f; // Duration of the animation in seconds for time
        float seasonAnimationDuration = 20f; // Duration of the animation in seconds for season

        float timeElapsedTime = Time.time % timeAnimationDuration; // Elapsed time since the time animation started
        float seasonElapsedTime = Time.time % seasonAnimationDuration; // Elapsed time since the season animation started

        // Calculate the normalized time from 0 to 1 for time and season
        float timeT = timeElapsedTime / timeAnimationDuration;
        float seasonT = seasonElapsedTime / seasonAnimationDuration;

        // Set the new anchored positions along the curved path with mapped speeds
        timeMarker.GetComponent<RectTransform>().anchoredPosition = GetCurvedPosition(timeT, spriteSize / timeAnimationDuration);
        seasonMarker.GetComponent<RectTransform>().anchoredPosition = GetCurvedPosition(1f - seasonT, spriteSize / seasonAnimationDuration); // Reverse the curve for the season marker
    }

    private Vector2 GetCurvedPosition(float t, float speed)
    {
        float x = Mathf.Lerp(-100f, 100f, t); // Adjust the x-axis values based on the desired width
        float y = Mathf.Lerp(0f, spriteSize, Mathf.Sin(t * Mathf.PI)); // Adjust the y-axis values based on the desired height and curve shape

        // Map the speed of movement along the curve
        float mappedSpeed = speed * (1f - t);

        return new Vector2(x, y + mappedSpeed);
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("UICanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = Camera.main;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        return canvas;
    }

    private GameObject CreateUIElement(string name)
    {
        GameObject uiObject = new GameObject(name);
        uiObject.transform.SetParent(uiCanvas.transform, false);

        // Set the rect transform properties
        RectTransform rectTransform = uiObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0f); // Bottom center
        rectTransform.anchorMax = new Vector2(0.5f, 0f); // Bottom center
        rectTransform.pivot = new Vector2(0.5f, 0f); // Bottom center

        return uiObject;
    }
}

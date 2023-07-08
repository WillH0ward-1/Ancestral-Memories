using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private float verticalOffset = 8.05f;
    [SerializeField] private Color healthColor = Color.red;
    [SerializeField] private Color faithColor = Color.cyan;
    [SerializeField] private Color hungerColor = Color.yellow;

    private GameObject uiParent;
    private GameObject healthFill;
    private GameObject faithFill;
    private GameObject hungerFill;

    private bool statsInitialized = false;

    [SerializeField] private float initialScale = 0.01f; // Initial scale of the UI elements
    [SerializeField] private float fillWidth = 100f; // Width of the fill area

    private AICharacterStats stats; // Reference to the AICharacterStats component

    private void Awake()
    {
        stats = GetComponent<AICharacterStats>(); // Get the AICharacterStats component

        uiParent = new GameObject("UIParent");
        uiParent.transform.SetParent(transform, false);

        // Instantiate the UI elements in world space.
        healthFill = CreateUIElement("HealthFill", healthColor);
        healthFill.SetActive(false);

        if (stats.useFaith)
        {
            faithFill = CreateUIElement("FaithFill", faithColor);
            faithFill.SetActive(false);
        }

        hungerFill = CreateUIElement("HungerFill", hungerColor);
        hungerFill.SetActive(false);

        SetUIElementScale(healthFill, initialScale);
        if (stats.useFaith)
        {
            SetUIElementScale(faithFill, initialScale);
        }
        SetUIElementScale(hungerFill, initialScale);

        BillBoardUI billboard = uiParent.AddComponent<BillBoardUI>();
        billboard.camera = Camera.main;
    }

    private void Start()
    {
        // Check if the stats are already initialized
        if (!statsInitialized)
        {
            InitializeStats();
        }
    }

    [SerializeField] private float tightness = 2.89f; // Adjust this value to control the tightness

    private void Update()
    {
        // Calculate the total height of the UI elements
        float totalHeight = healthFill.GetComponent<RectTransform>().rect.height;
        if (stats.useFaith)
        {
            totalHeight += faithFill.GetComponent<RectTransform>().rect.height;
        }
        totalHeight += hungerFill.GetComponent<RectTransform>().rect.height;

        // Calculate the offset based on the total height and tightness
        float offset = totalHeight / (2 * tightness);

        // Calculate the individual offsets for each UI element
        float healthOffset = offset - healthFill.GetComponent<RectTransform>().rect.height / 2;
        float hungerOffset = offset - hungerFill.GetComponent<RectTransform>().rect.height / 2;

        // Position the UI elements above the character with the adjusted offsets
        Vector3 basePosition = transform.position + new Vector3(0, verticalOffset, 0);
        healthFill.transform.position = basePosition + new Vector3(0, healthOffset, 0);
        if (stats.useFaith)
        {
            float faithOffset = 0f;
            faithFill.transform.position = basePosition + new Vector3(0, faithOffset, 0);
            UpdateFaith(stats.FaithFraction);
        }
        hungerFill.transform.position = basePosition + new Vector3(0, -hungerOffset, 0);

        // Update the fill amount based on the stats
        UpdateHealth(stats.HealthFraction);
        UpdateHunger(stats.HungerFraction);
    }


    public void SetUIElementScale(GameObject uiElement, float scale)
    {
        // Get the RectTransform component of the UI element
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();

        // Set the scale of the UI element
        rectTransform.localScale = new Vector3(scale, scale, scale);
    }

    private GameObject CreateUIElement(string name, Color color)
    {
        GameObject uiObject = new GameObject(name);
        uiObject.transform.SetParent(uiParent.transform, false);

        // Add a Canvas and set it to world space
        Canvas canvas = uiObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        // Set the Event Camera to the main camera
        canvas.worldCamera = Camera.main;
        // Scale it down (optional) and set its size
        canvas.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        canvas.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 10);

        // Create the RectMask2D component
        RectMask2D rectMask = uiObject.AddComponent<RectMask2D>();

        // Create the fill object as a child of the uiObject
        GameObject fillObject = new GameObject("Fill");
        fillObject.transform.SetParent(uiObject.transform, false);

        // Add an Image component to the fill object
        Image fillImage = fillObject.AddComponent<Image>();
        fillImage.color = color;

        // Set the rect transform properties
        RectTransform fillRect = fillObject.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(1f, 0.5f); // Right-aligned (anchorMin.x = 1f)
        fillRect.anchorMax = new Vector2(1f, 0.5f); // Right-aligned (anchorMax.x = 1f)
        fillRect.pivot = new Vector2(1f, 0.5f); // Right-aligned (pivot.x = 1f)
        fillRect.anchoredPosition = Vector2.zero;
        fillRect.sizeDelta = new Vector2(fillWidth, 10f); // Set the width of the fill area

        return uiObject;
    }

    private void HealthStat(float healthFraction, float min, float max)
    {
        UpdateHealth(healthFraction);
    }

    private void FaithStat(float faithFraction, float min, float max)
    {
        UpdateFaith(faithFraction);
    }

    private void HungerStat(float hungerFraction, float min, float max)
    {
        UpdateHunger(hungerFraction);
    }

    private void InitializeStats()
    {
        if (stats != null)
        {
            statsInitialized = true;

            // Attach the update functions to the stat change events.
            stats.OnHealthChanged += HealthStat;
            if (stats.useFaith)
            {
                stats.OnFaithChanged += FaithStat;
            }
            stats.OnHungerChanged += HungerStat;
        }
    }

    private void UpdateHealth(float healthFraction)
    {
        healthFill.SetActive(true);
        Image fillImage = healthFill.GetComponentInChildren<Image>();
        fillImage.fillAmount = healthFraction;

        // Calculate the mapped value of padding based on the health fraction
        float paddingValue = (1f - healthFraction) * 100f;
        RectMask2D rectMask = healthFill.GetComponentInChildren<RectMask2D>();
        rectMask.padding = new Vector4(0f, 0f, paddingValue, 0f);
    }

    private void UpdateFaith(float faithFraction)
    {
        faithFill.SetActive(true);
        Image fillImage = faithFill.GetComponentInChildren<Image>();
        fillImage.fillAmount = faithFraction;

        // Calculate the mapped value of padding based on the faith fraction
        float paddingValue = (1f - faithFraction) * 100f;
        RectMask2D rectMask = faithFill.GetComponentInChildren<RectMask2D>();
        rectMask.padding = new Vector4(0f, 0f, paddingValue, 0f);
    }

    private void UpdateHunger(float hungerFraction)
    {
        hungerFill.SetActive(true);
        Image fillImage = hungerFill.GetComponentInChildren<Image>();
        fillImage.fillAmount = hungerFraction;

        // Calculate the mapped value of padding based on the hunger fraction
        float paddingValue = (1f - hungerFraction) * 100f;
        RectMask2D rectMask = hungerFill.GetComponentInChildren<RectMask2D>();
        rectMask.padding = new Vector4(0f, 0f, paddingValue, 0f);
    }

    private void OnDestroy()
    {
        if (stats != null)
        {
            stats.OnHealthChanged -= HealthStat;
            if (stats.useFaith)
            {
                stats.OnFaithChanged -= FaithStat;
            }
            stats.OnHungerChanged -= HungerStat;
        }

        Destroy(uiParent);
    }
}

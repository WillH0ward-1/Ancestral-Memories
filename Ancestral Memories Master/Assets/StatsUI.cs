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

    [SerializeField] private float initialScale = 0.01f;
    [SerializeField] private float fillWidth = 100f;

    private AICharacterStats stats;

    private RectTransform healthFillRect;
    private RectTransform faithFillRect;
    private RectTransform hungerFillRect;

    private Image healthFillImage;
    private Image faithFillImage;
    private Image hungerFillImage;

    private RectMask2D healthFillMask;
    private RectMask2D faithFillMask;
    private RectMask2D hungerFillMask;

    private Player player;

    private bool isUIVisible;
 
    private void Awake()
    {
        player = FindObjectOfType<Player>();

        stats = GetComponent<AICharacterStats>();

        uiParent = new GameObject("UIParent");
        uiParent.transform.SetParent(transform, false);

        healthFill = CreateUIElement("HealthFill", healthColor);
        healthFill.SetActive(true);

        healthFillRect = healthFill.GetComponent<RectTransform>();
        healthFillImage = healthFill.GetComponentInChildren<Image>();
        healthFillMask = healthFill.GetComponentInChildren<RectMask2D>();

        if (stats.useFaith)
        {
            faithFill = CreateUIElement("FaithFill", faithColor);
            faithFill.SetActive(true);

            faithFillRect = faithFill.GetComponent<RectTransform>();
            faithFillImage = faithFill.GetComponentInChildren<Image>();
            faithFillMask = faithFill.GetComponentInChildren<RectMask2D>();
        }

        hungerFill = CreateUIElement("HungerFill", hungerColor);
        hungerFill.SetActive(true);

        hungerFillRect = hungerFill.GetComponent<RectTransform>();
        hungerFillImage = hungerFill.GetComponentInChildren<Image>();
        hungerFillMask = hungerFill.GetComponentInChildren<RectMask2D>();

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
        if (!statsInitialized)
        {
            InitializeStats();
        }
    }

    [SerializeField] private float tightness = 2.89f;

    public float hideUIthreshold = 50;

    private bool InRange(GameObject target)
    {
        bool inRange;

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance >= hideUIthreshold)
        {
            inRange = false;
        }
        else
        {
            inRange = true;
        }

        return inRange;
    }

    private void Update()
    {
        if (!transform.CompareTag("Player") && !transform.CompareTag("Animal") && InRange(player.gameObject))
        {
            if (stats.isDead)
            {
                ShowUI();
                UpdateUI();
            } else
            {
                HideUI();
            }
        } else if (transform.CompareTag("Player"))
        {
            ShowUI();
            UpdateUI();
        } else
        {
            HideUI();
        }
    }

    public void HideUI()
    {
        if (uiParent.activeSelf)
        {
            uiParent.SetActive(false);  // Deactivate the parent GameObject, which will hide all children
        }

        if (isUIVisible)
        {
            isUIVisible = false;        // Update the flag
        }
    }

    public void ShowUI()
    {
        if (!uiParent.activeSelf)
        {
            uiParent.SetActive(true);  // Activate the parent GameObject, which will show all children
        }

        if (!isUIVisible)
        {
            isUIVisible = true;        // Update the flag
        }
    }


    private void UpdateUI()
    {
        ShowUI();

        float totalHeight = healthFillRect.rect.height;
        if (stats.useFaith)
        {
            totalHeight += faithFillRect.rect.height;
        }
        totalHeight += hungerFillRect.rect.height;

        float offset = totalHeight / (2 * tightness);
        float healthOffset = offset - healthFillRect.rect.height / 2;
        float hungerOffset = offset - hungerFillRect.rect.height / 2;

        Vector3 basePosition = transform.position + new Vector3(0, verticalOffset, 0);
        healthFill.transform.position = basePosition + new Vector3(0, healthOffset, 0);

        if (stats.useFaith)
        {
            float faithOffset = 0f;
            faithFill.transform.position = basePosition + new Vector3(0, faithOffset, 0);
            UpdateFaith(stats.FaithFraction);
        }

        hungerFill.transform.position = basePosition + new Vector3(0, -hungerOffset, 0);

        UpdateHealth(stats.HealthFraction);
        UpdateHunger(stats.HungerFraction);
    }

    public void SetUIElementScale(GameObject uiElement, float scale)
    {
        RectTransform rectTransform = uiElement.GetComponent<RectTransform>();
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
            stats.OnHealthChanged += HealthStat;
            if (stats.useFaith)
            {
                stats.OnFaithChanged += FaithStat;
            }
            stats.OnHungerChanged += HungerStat;
        }
    }

    private void UpdateUI(float valueFraction, Image fillImage, RectMask2D rectMask)
    {
        fillImage.fillAmount = valueFraction;
        float paddingValue = (1f - valueFraction) * 100f;
        rectMask.padding = new Vector4(0f, 0f, paddingValue, 0f);
    }

    private void UpdateHealth(float healthFraction)
    {
        UpdateUI(healthFraction, healthFillImage, healthFillMask);
    }

    private void UpdateFaith(float faithFraction)
    {
        UpdateUI(faithFraction, faithFillImage, faithFillMask);
    }

    private void UpdateHunger(float hungerFraction)
    {
        UpdateUI(hungerFraction, hungerFillImage, hungerFillMask);
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

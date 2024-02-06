using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(HorizontalLayoutGroup))]
public class ResourcesUI : MonoBehaviour
{
    public TMP_FontAsset fontAsset;
    public float uiHeight = 50f;
    public float spacing = 10f;
    public float horizontalOffset = 0f; // Variable to control the horizontal shift
    public float verticalOffset = 0f; // Variable to control the vertical shift

    private HorizontalLayoutGroup layoutGroup;
    private float previousHorizontalOffset; // For tracking changes in horizontalOffset
    private float previousVerticalOffset; // For tracking changes in verticalOffset

    public List<RectTransform> uiElements = new List<RectTransform>(); // Keep track of UI elements
    private Dictionary<string, TextMeshProUGUI> resourceTexts = new Dictionary<string, TextMeshProUGUI>();

    private void Awake()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.spacing = spacing;

        previousHorizontalOffset = horizontalOffset;
        previousVerticalOffset = verticalOffset;

        ApplyOffsets();
    }

    private void ApplyOffsets()
    {
        layoutGroup.padding.left = (int)horizontalOffset;
        layoutGroup.padding.top = (int)verticalOffset;

        foreach (var uiElement in uiElements)
        {
            UpdateUIElementPosition(uiElement);
        }
    }

    private void OnValidate()
    {
        if (layoutGroup == null) layoutGroup = GetComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = spacing;
        ApplyOffsets();
    }

    private void Update()
    {
        if (!Mathf.Approximately(horizontalOffset, previousHorizontalOffset) ||
            !Mathf.Approximately(verticalOffset, previousVerticalOffset))
        {
            ApplyOffsets();
            previousHorizontalOffset = horizontalOffset;
            previousVerticalOffset = verticalOffset;
        }
    }

public void UpdateResourceCount(string resourceTypeName, int count)
{
    if (resourceTexts.TryGetValue(resourceTypeName, out var textComponent))
    {
        // Update the text component with the new count
        textComponent.text = $"{resourceTypeName}: {count}";

        // Find the background GameObject by name
        var backgroundName = $"{resourceTypeName}Background";
        var backgroundGameObject = textComponent.transform.parent.Find(backgroundName)?.gameObject;

        if (backgroundGameObject != null)
        {
            // Calculate the width based on the number of digits
            var digitCount = Mathf.Max(1, Mathf.FloorToInt(Mathf.Log10(count) + 1));
            var additionalWidth = (digitCount - 1) * 10f; // Add 10 units of width for each additional digit beyond 1
            var backgroundRectTransform = backgroundGameObject.GetComponent<RectTransform>();
            
            // Calculate the new width
            var newWidth = textComponent.preferredWidth + padding + additionalWidth;
            backgroundRectTransform.sizeDelta = new Vector2(newWidth, backgroundRectTransform.sizeDelta.y);
        }
    }
}


    public void ClearUIElementsList()
    {
        uiElements.Clear();
    }

    public void InitializeUI(Dictionary<string, int> resources)
    {
        ClearUIElements();

        foreach (var entry in resources)
        {
            CreateResourceUI(entry.Key, entry.Value);
        }

        foreach (var uiElement in uiElements)
        {
            UpdateUIElementPosition(uiElement);
        }
    }

    public void ClearUIElements()
    {
        foreach (RectTransform element in uiElements)
        {
            if (element != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(element.gameObject);
                }
                else
                {
                    DestroyImmediate(element.gameObject);
                }
            }
        }

        ClearUIElementsList();
        resourceTexts.Clear();
    }

    float padding = 5f;

    private void CreateResourceUI(string resourceName, int value)
    {
        // Create the UI GameObject with RectTransform
        GameObject resourceUI = new GameObject($"UI: {resourceName}", typeof(RectTransform));
        resourceUI.transform.SetParent(transform, false);

        // Add and setup TextMeshProUGUI component
        TextMeshProUGUI tmp = resourceUI.AddComponent<TextMeshProUGUI>();
        SetupTextMeshPro(tmp, resourceName, value);

        // Calculate height based on font size
        float backgroundHeight = (tmp.fontSize / 12) * tmp.fontSize; // Additional space to cover text appropriately

        // Create background image GameObject as a child of the resource UI
        GameObject bgImageGameObject = new GameObject($"{resourceName}Background", typeof(RectTransform), typeof(Canvas), typeof(Image));
        bgImageGameObject.transform.SetParent(resourceUI.transform, false);

        // Add and configure the Canvas component
        Canvas bgCanvas = bgImageGameObject.GetComponent<Canvas>();
        bgCanvas.overrideSorting = true;
        bgCanvas.sortingOrder = -1; // Set sorting order to ensure it's rendered behind the text

        // Setup the background image
        Image bgImage = bgImageGameObject.GetComponent<Image>();
        bgImage.color = Color.black; // Set background color to black
        RectTransform bgRectTransform = bgImageGameObject.GetComponent<RectTransform>();
        bgRectTransform.anchorMin = new Vector2(0.5f, 1f); // Center horizontally, top vertically
        bgRectTransform.anchorMax = new Vector2(0.5f, 1f);
        bgRectTransform.anchoredPosition = new Vector2(0, 0); // Position at the anchors
        bgRectTransform.sizeDelta = new Vector2(tmp.preferredWidth + padding, backgroundHeight); // Set size based on text
        bgRectTransform.pivot = new Vector2(0.5f, 1f); // Pivot at the top center
        bgImage.raycastTarget = false; // Make sure it's not a raycast receiver

        // Adjust RectTransform of the resource UI to fit the content
        RectTransform rectTransform = resourceUI.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(tmp.preferredWidth, backgroundHeight);
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);

        // Add the RectTransform to the list for management and positioning
        uiElements.Add(rectTransform);

        // Update the position of the RectTransform in the UI
        UpdateUIElementPosition(rectTransform);
    }



    private void SetupTextMeshPro(TextMeshProUGUI tmp, string resourceName, int value)
    {
        tmp.text = $"{resourceName}: {value}";
        tmp.fontSize = 15;
        tmp.fontStyle = FontStyles.Italic | FontStyles.SmallCaps;
        tmp.color = Color.white;
        tmp.font = fontAsset;
        tmp.enableWordWrapping = false;

        // Configure TextMeshPro properties as needed
    }

    private void UpdateUIElementPosition(RectTransform uiElement)
    {
        int index = uiElements.IndexOf(uiElement);
        float elementXPosition = horizontalOffset + (spacing * index) - (spacing * (uiElements.Count - 1) / 2.0f);
        float elementYPosition = -verticalOffset;

        uiElement.anchoredPosition = new Vector2(elementXPosition, elementYPosition);
    }
}

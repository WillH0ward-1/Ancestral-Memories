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
            textComponent.text = $"{resourceTypeName}: {count}";
        }
        else
        {
            // Handle the case where the resource type is not found
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

    private void CreateResourceUI(string resourceName, int value)
    {
        GameObject resourceUI = new GameObject($"UI: {resourceName}", typeof(RectTransform), typeof(TextMeshProUGUI));
        resourceUI.transform.SetParent(transform, false);

        TextMeshProUGUI tmp = resourceUI.GetComponent<TextMeshProUGUI>();
        SetupTextMeshPro(tmp, resourceName, value);

        RectTransform rectTransform = resourceUI.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, uiHeight);
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);

        uiElements.Add(rectTransform);
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

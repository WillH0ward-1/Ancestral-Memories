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

    private HorizontalLayoutGroup layoutGroup;
    public List<RectTransform> uiElements = new List<RectTransform>(); // Keep track of UI elements

    private void Awake()
    {
        layoutGroup = GetComponent<HorizontalLayoutGroup>();
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.childForceExpandHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childControlHeight = true;
        layoutGroup.spacing = spacing;
        layoutGroup.padding = new RectOffset(50, 50, 50, 50);
        previousHorizontalOffset = horizontalOffset;
        layoutGroup.padding.left = (int)horizontalOffset;
    }

    private Dictionary<string, TextMeshProUGUI> resourceTexts = new Dictionary<string, TextMeshProUGUI>();

    public void UpdateResourceCount(string resourceTypeName, int count)
    {
        if (resourceTexts.TryGetValue(resourceTypeName, out var textComponent))
        {
            // Update the resource count in the UI
            textComponent.text = $"{resourceTypeName}: {count}";
        }
        else
        {
            Debug.LogError($"No UI element found for resource type: {resourceTypeName}");
        }
    }

    private void OnValidate()
    {
        if (layoutGroup == null) layoutGroup = GetComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = spacing;

        // Update the padding to incorporate the horizontalOffset.
        layoutGroup.padding.left = (int)horizontalOffset;

        // Update the height of all UI elements.
        foreach (RectTransform uiElement in uiElements)
        {
            if (uiElement != null)
            {
                uiElement.sizeDelta = new Vector2(uiElement.sizeDelta.x, uiHeight);
            }
        }
    }

    private float previousHorizontalOffset; // Add this variable at the class level

    private void Update()
    {
        if (!Mathf.Approximately(horizontalOffset, previousHorizontalOffset))
        {
            // Update the padding of the layout group to shift the elements.
            layoutGroup.padding.left = (int)horizontalOffset;

            // Force the layout group to re-arrange the elements immediately.
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)layoutGroup.transform);

            previousHorizontalOffset = horizontalOffset;
        }
    }

    public void UpdateAllUIPositions()
    {
        foreach (var uiElement in uiElements)
        {
            UpdateUIElementPosition(uiElement);
        }
    }


private void UpdateUIElementPosition(RectTransform uiElement)
{
    int index = uiElements.IndexOf(uiElement);
    // Assuming a centered anchor, calculate the position for each element based on its index
    float elementXPosition = horizontalOffset + (spacing * index) - (spacing * (uiElements.Count - 1) / 2.0f);
    uiElement.anchoredPosition = new Vector2(elementXPosition, uiElement.anchoredPosition.y);
}



    public void ClearUIElementsList()
    {
        if (uiElements != null)
        {
            uiElements.Clear();
        }
    }

    public void InitializeUI(Dictionary<string, int> resources)
    {
        ClearUIElements();

        foreach (var entry in resources)
        {
            CreateResourceUI(entry.Key, entry.Value);
        }

        // Ensure all positions are updated after initialization
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
                DestroyImmediate(element.gameObject);
            }
        }
        uiElements.Clear();
        resourceTexts.Clear(); // Clear the dictionary as well
    }



    private void CreateResourceUI(string resourceName, int value)
    {
        GameObject resourceUI = new GameObject($"UI: {resourceName}", typeof(RectTransform), typeof(TextMeshProUGUI));
        resourceUI.transform.SetParent(transform, false);

        TextMeshProUGUI tmp = resourceUI.GetComponent<TextMeshProUGUI>();
        SetupTextMeshPro(tmp, resourceName, value);

        RectTransform rectTransform = resourceUI.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, uiHeight); // Adjust the width as needed
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

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
        tmp.outlineWidth = 0.1f;
        tmp.outlineColor = Color.black;

        tmp.fontMaterial = new Material(fontAsset.material);
        tmp.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 0.3f);
        tmp.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, 0.35f);

        tmp.fontSharedMaterial = new Material(fontAsset.material);
        tmp.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, -1f);
        tmp.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY, 1f);
        tmp.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate, 1f);
        tmp.fontSharedMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness, 0f);
        tmp.fontSharedMaterial.SetColor(ShaderUtilities.ID_UnderlayColor, Color.black);
    }
}

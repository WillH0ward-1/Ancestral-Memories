using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour
{
    public BuildMenuButton buttonPrefab;
    private List<BuildMenuButton> buttons = new List<BuildMenuButton>();
    private BuildingManager manager;

    [SerializeField] private float hoverAmplitude = 5.0f;
    [SerializeField] private float hoverFrequency = 1.0f;

    [SerializeField] private float popDuration = 0.25f; // Duration of the pop effect
    [SerializeField] private AnimationCurve popCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // Customize this curve in the inspector

    // Newly added properties
    public BuildMenuButton selected;
    public string selectionText;

    [SerializeField] private TextMeshProUGUI selectionTextDisplay; // TextMeshPro Element

    private void Awake()
    {
        selectionText = "";
        selectionTextDisplay = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SpawnButtons(List<BuildingManager.BuildingOption> options, BuildingManager manager)
    {
        this.manager = manager;
        StartCoroutine(SpawnAndPopButtons(options));
    }

    private IEnumerator SpawnAndPopButtons(List<BuildingManager.BuildingOption> options)
    {
        for (int i = 0; i < options.Count; i++)
        {
            BuildMenuButton newButton = Instantiate(buttonPrefab);
            newButton.transform.SetParent(transform, false);

            // Calculate the position of the button
            float theta = (2 * Mathf.PI / options.Count) * i;
            float xPos = Mathf.Sin(theta);
            float yPos = Mathf.Cos(theta);

            newButton.transform.localPosition = new Vector3(xPos, yPos, 0f) * 150f; // Adjust radius as needed
            newButton.transform.localScale = Vector3.zero;
            newButton.circle.color = options[i].color;
            newButton.icon.sprite = options[i].sprite;
            newButton.title = options[i].title;
            newButton.myMenu = this;
            newButton.buildingType = options[i].type;

            buttons.Add(newButton);

            float hoverOffset = Random.Range(0f, 2f * Mathf.PI);
            StartCoroutine(HoverButton(newButton, hoverOffset));

            if (i == 0)
            {
                StartCoroutine(PopButton(newButton));
            }
            else
            {
                float delay = Random.Range(0.25f, 0.5f);
                yield return new WaitForSeconds(delay);
                StartCoroutine(PopButton(newButton));
            }
        }
    }

    private IEnumerator PopButton(BuildMenuButton button)
    {
        float time = 0;
        while (time < popDuration)
        {
            time += Time.deltaTime;
            float scale = popCurve.Evaluate(time / popDuration);
            button.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        button.transform.localScale = Vector3.one; // Ensure it ends at the correct scale
    }

    private IEnumerator HoverButton(BuildMenuButton button, float offset)
    {
        Vector3 originalPosition = button.transform.localPosition;
        while (true)
        {
            float y = hoverAmplitude * Mathf.Sin(hoverFrequency * Time.time + offset);
            button.transform.localPosition = originalPosition + new Vector3(0, y, 0);
            yield return null;
        }
    }

    public void SelectOption(BuildingManager.BuildingType buildingType)
    {
        manager.SelectBuildingOption(buildingType);
    }

    void Update()
    {
        if (selectionTextDisplay != null)
        {
            selectionTextDisplay.text = selectionText; // Update the text display
        }

        if (Input.GetMouseButtonUp(0) && selected != null) // Left-click
        {
            SelectOption(selected.buildingType);
        }

        if (Input.GetMouseButtonUp(1)) // Right-click
        {
            manager.CloseMenu();
        }
    }

    private void CleanUpButtons()
    {
        foreach (var button in buttons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        buttons.Clear();
    }

    void OnDestroy()
    {
        CleanUpButtons();
    }

    public bool IsActive { get { return gameObject.activeInHierarchy; } }

    public void CloseMenu()
    {
        CleanUpButtons();
        Destroy(gameObject);
    }
}

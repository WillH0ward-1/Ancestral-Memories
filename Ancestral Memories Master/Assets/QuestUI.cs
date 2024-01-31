using UnityEngine;
using TMPro;
using System.Collections;

public class QuestUI : MonoBehaviour
{
    public TMP_FontAsset fontAsset;
    public float uiHeight = 50f;
    private TextMeshProUGUI questText;
    private QuestManager questManager;
    private Coroutine showHideCoroutine;
    private float maxDilation = 0.22f;
    private float minDilation = 0f;


    public void InitializeQuests()
    {
        questManager = QuestManager.Instance;
        questText = GetComponentInChildren<TextMeshProUGUI>(true);

        // Check if questManager is not null before subscribing to the event
        if (questManager != null)
        {
            questManager.OnQuestChanged += OnQuestChanged;
            questManager.OnQuestChanged += ShowQuestText;
        }
        else
        {
            Debug.LogError("QuestManager instance not found.");
            return;
        }

        InitializeQuestUI();
        SetupTextMeshPro();
        ShowQuestText();
        StartRandomShowHideTimer();
    }

    private void InitializeQuestUI()
    {
        if (!questText) return; // If no TextMeshProUGUI is found in children, do nothing

        // Set up the RectTransform to be full width and the defined height at the bottom center
        RectTransform rectTransform = questText.rectTransform;
        rectTransform.anchorMin = new Vector2(0.5f, 0f);
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.sizeDelta = new Vector2(Screen.width, uiHeight);
        rectTransform.anchoredPosition = new Vector2(0, uiHeight / 2);
    }

    private void SetupTextMeshPro()
    {
        // Ensure the TextMeshProUGUI component is not null
        if (!questText) return;

        questText.text = $"Current Quest: {questManager.GetCurrentQuest()}";
        questText.fontSize = 20; // Assign the desired font size
        questText.alignment = TextAlignmentOptions.Center; // Center the text
        questText.font = fontAsset; // Assign the font asset
        questText.color = Color.white; // Assign the text color
        questText.enableWordWrapping = false; // Disable word wrapping
    }

    public void ShowQuestText()
    {
        if (showHideCoroutine != null)
        {
            StopCoroutine(showHideCoroutine);
        }
        showHideCoroutine = StartCoroutine(ChangeQuestTextVisibility(true));
    }

    public void HideQuestText()
    {
        if (showHideCoroutine != null)
        {
            StopCoroutine(showHideCoroutine);
        }
        showHideCoroutine = StartCoroutine(ChangeQuestTextVisibility(false));
    }

    private IEnumerator ChangeQuestTextVisibility(bool show)
    {
        float targetDilation = show ? maxDilation : minDilation;
        float targetAlpha = show ? 1f : 0f;
        float currentDilation = questText.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);
        float currentAlpha = questText.color.a;

        while (show ? currentDilation < maxDilation : currentDilation > minDilation)
        {
            currentDilation = Mathf.MoveTowards(currentDilation, targetDilation, Time.deltaTime * (maxDilation - minDilation));
            currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime);
            questText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, currentDilation);
            questText.color = new Color(questText.color.r, questText.color.g, questText.color.b, currentAlpha);
            yield return null;
        }

        if (!show)
        {
            questText.gameObject.SetActive(false);
        }
    }

    private void HideQuestTextInstant()
    {
        questText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, minDilation);
        questText.color = new Color(questText.color.r, questText.color.g, questText.color.b, 0f);
        questText.gameObject.SetActive(false);
    }

    private void StartRandomShowHideTimer()
    {
        if (showHideCoroutine != null)
        {
            StopCoroutine(showHideCoroutine);
        }
        showHideCoroutine = StartCoroutine(RandomShowHideTimer());
    }

    private IEnumerator RandomShowHideTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(120f, 180f));
            ShowQuestText();
            yield return new WaitForSeconds(Random.Range(120f, 180f));
            HideQuestText();
        }
    }

    private void OnQuestChanged()
    {
        ShowQuestText();
        StartRandomShowHideTimer();
    }

    private void OnDisable()
    {
        // Unsubscribe from the OnQuestChanged event when the GameObject is disabled
        if (questManager != null)
        {
            questManager.OnQuestChanged -= OnQuestChanged;
        }
    }

    private void Update()
    {
        // Ensure the TextMeshProUGUI component and questManager are not null
        if (!questText || !questManager) return;

        // Update the displayed quest text if the current quest changes
        questText.text = $"Current Quest: {questManager.GetCurrentQuest()}";
    }
}

using System.Collections;
using UnityEngine;

public class MouseClickWaiter : MonoBehaviour
{
    [SerializeField] private CamControl camControl;
    [SerializeField] private TitleUIControl titleControlUI;
    [SerializeField] private AutoFitText autoFitText;

    [SerializeField] private bool waitForClick = true;

    private Camera cam;

    QuestManager questManager;
    QuestUI questUI;

    private void Awake()
    {
        cam = Camera.main;
        camControl = cam.GetComponentInChildren<CamControl>();
        titleControlUI = GetComponentInChildren<TitleUIControl>();
        autoFitText = GetComponentInChildren<AutoFitText>();
    }

    private void Start()
    {
        questManager = QuestManager.Instance;
        questUI = questManager.transform.GetComponentInChildren<QuestUI>();
        StartCoroutine(WaitForMouseClick());
    }

    private IEnumerator WaitForMouseClick()
    {
        // Start any necessary coroutines or logic before waiting for the mouse click
        titleControlUI.StartCoroutine(titleControlUI.FadeTextToFullAlpha(2f));

        waitForClick = true;

        while (waitForClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                camControl.ToSpawnZoom();
                titleControlUI.StartCoroutine(titleControlUI.FadeTextToZeroAlpha(2f));
                questUI.InitializeQuests();
                waitForClick = false;
                autoFitText.isResizing = false;
            }

            yield return null;
        }

        yield break;
    }
}

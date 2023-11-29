using System.Collections;
using UnityEngine;

public class MouseClickWaiter : MonoBehaviour
{
    [SerializeField] private CamControl camControl;
    [SerializeField] private TitleUIControl titleControlUI;
    [SerializeField] private AutoFitText autoFitText;

    [SerializeField] private bool waitForClick = true;

    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
        camControl = cam.GetComponentInChildren<CamControl>();
        titleControlUI = GetComponentInChildren<TitleUIControl>();
        autoFitText = GetComponentInChildren<AutoFitText>();
    }

    private void Start()
    {
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
                waitForClick = false;
                autoFitText.isResizing = false;
            }

            yield return null;
        }

        yield break;
    }
}

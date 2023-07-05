using System.Collections;
using UnityEngine;

public class MouseClickWaiter : MonoBehaviour
{
    [SerializeField] private CamControl camControl;
    [SerializeField] private TitleUIControl titleControlUI;

    [SerializeField] private bool waitForClick = true;

    private void Awake()
    {
        camControl = FindObjectOfType<CamControl>();
        titleControlUI = GetComponentInChildren<TitleUIControl>();
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
            }

            yield return null;
        }

        yield break;
    }
}

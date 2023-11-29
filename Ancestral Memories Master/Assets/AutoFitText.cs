using UnityEngine;
using TMPro;

[ExecuteAlways]
public class AutoFitText : MonoBehaviour
{
    public float baseFontSize = 100f;
    public float scaleFactor = 0.01f;

    public bool isResizing = true; // Add this field to control the resizing

    private TMP_Text tmpText;
    private Vector2 lastScreenSize;

    private void OnEnable()
    {
        tmpText = GetComponent<TMP_Text>();
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        if (isResizing)
        {
            ResizeText();
        }
    }

    private void Update()
    {
        if (!isResizing) return; // Check the flag before proceeding

        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        // Check if the screen size has changed since the last frame
        if (currentScreenSize != lastScreenSize)
        {
            ResizeText();
            lastScreenSize = currentScreenSize;
        }
    }

    private void ResizeText()
    {
        float ratio = Screen.width / (float)Screen.height;
        float newFontSize = baseFontSize * ratio * scaleFactor;
        tmpText.fontSize = newFontSize;
    }
}

using UnityEngine;
using TMPro;

public class AutoFitText : MonoBehaviour
{
    public float baseFontSize = 100f;
    public float scaleFactor = 0.01f;

    private TMP_Text tmpText;

    private void Start()
    {
        tmpText = GetComponent<TMP_Text>();
        ResizeText();
    }

    private void Update()
    {
        ResizeText();
    }

    private void ResizeText()
    {
        float ratio = Screen.width / (float)Screen.height;
        float newFontSize = baseFontSize * ratio * scaleFactor;
        tmpText.fontSize = newFontSize;
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(TextMeshProUGUI))]
public class UITextAnimator : MonoBehaviour
{
    [Header("Shake Effect Settings")]
    [SerializeField, Tooltip("Strength of the shaking effect.")]
    private float shakeMagnitude = 0.1f;

    [SerializeField, Tooltip("Speed of the shaking effect.")]
    private float shakeSpeed = 1.0f;

    private TextMeshProUGUI textComponent;
    private float[] characterOffsets;

    private void OnEnable()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        if (textComponent == null)
            yield break;

        string originalText = textComponent.text;
        characterOffsets = new float[originalText.Length];

        while (true)
        {
            if (originalText != textComponent.text)
            {
                originalText = textComponent.text;
                characterOffsets = new float[originalText.Length];
            }

            for (int i = 0; i < originalText.Length; i++)
            {
                characterOffsets[i] = (Mathf.PerlinNoise(i, Time.time * shakeSpeed) - 0.5f) * shakeMagnitude;
            }

            string newText = string.Empty;
            for (int i = 0; i < originalText.Length; i++)
            {
                char c = originalText[i];
                float offset = characterOffsets[i];
                newText += $"<voffset={offset}em>{c}</voffset>";
            }

            textComponent.text = newText;
            yield return null;
        }
    }
}

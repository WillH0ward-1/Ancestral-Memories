using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleUIControl : MonoBehaviour
{
    [SerializeField] private Vector2 alphaRange = new Vector2(0f, 1f);
    [SerializeField] private Vector2 dilationRange = new Vector2(-1f, -0.3f);

    private TextMeshProUGUI text;

    [SerializeField] private bool isFading = false;

    [SerializeField] private float fadeDelay = 0;

    private IEnumerator fadeToFullAlphaCoroutine;
    private IEnumerator fadeToZeroAlphaCoroutine;

    private void Awake()
    {
        text = transform.GetComponent<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, alphaRange.x);

        text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilationRange.x);
    }

    public IEnumerator FadeTextToFullAlpha(float t)
    {
        yield return new WaitForSeconds(fadeDelay);

        isFading = true;

        text.color = new Color(text.color.r, text.color.g, text.color.b, alphaRange.x);

        float time = 0;
        time += Time.deltaTime / t;

        float startValue = dilationRange.x;
        float endValue = dilationRange.y;
        float elapsedTime = 0;

        while (text.color.a <= alphaRange.y || isFading)
        {
            float t2 = elapsedTime / t;
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(alphaRange.x, alphaRange.y, t2));
            text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, Mathf.Lerp(startValue, endValue, t2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (fadeToFullAlphaCoroutine != null)
        {
            StopCoroutine(fadeToFullAlphaCoroutine);
        }

        fadeToFullAlphaCoroutine = null;
    }

    public IEnumerator FadeTextToZeroAlpha(float t)
    {
        isFading = true;

        text.color = new Color(text.color.r, text.color.g, text.color.b, alphaRange.y);

        float time = 0;
        time += Time.deltaTime / t;

        float startValue = dilationRange.y;
        float endValue = dilationRange.x;
        float elapsedTime = 0;

        while (text.color.a >= alphaRange.x || isFading)
        {
            float t2 = elapsedTime / t;
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(alphaRange.y, alphaRange.x, t2));
            text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, Mathf.Lerp(startValue, endValue, t2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        DisableTitleText();

        if (fadeToZeroAlphaCoroutine != null)
        {
            StopCoroutine(fadeToZeroAlphaCoroutine);
        }

        fadeToZeroAlphaCoroutine = null;
    }

    public void StartTextAnimations()
    {
        if (fadeToFullAlphaCoroutine != null)
        {
            StopCoroutine(fadeToFullAlphaCoroutine);
        }

        if (fadeToZeroAlphaCoroutine != null)
        {
            StopCoroutine(fadeToZeroAlphaCoroutine);
        }

        fadeToFullAlphaCoroutine = FadeTextToFullAlpha(1f);
        fadeToZeroAlphaCoroutine = FadeTextToZeroAlpha(1f);

        StartCoroutine(fadeToFullAlphaCoroutine);
        StartCoroutine(fadeToZeroAlphaCoroutine);
    }

    public void DisableTitleText()
    {
        transform.gameObject.SetActive(false);
    }
}
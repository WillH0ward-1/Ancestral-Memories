using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleUIControl : MonoBehaviour
{

    private TextMeshProUGUI text;

    private bool fading = false;

    private void Awake()
    {
        text = transform.GetComponent<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }

    public IEnumerator FadeTextToFullAlpha(float t)
    {
        fading = false;

        fading = true;

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

        float time = 0;
        time += Time.deltaTime / t;

        while (text.color.a <= 1f || fading)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + time);
            yield return null;
        }

        yield break;
    }

    public IEnumerator FadeTextToZeroAlpha(float t)
    {
        fading = false;

        fading = true;

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        float time = 0;
        time += Time.deltaTime / t;

        while (text.color.a >= 0f || fading)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - time);
            yield return null;
        }

        yield break;
    }
}


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TitleUIControl : MonoBehaviour
{

    private TextMeshProUGUI text;

    [SerializeField] private bool isFading = false;

    [SerializeField] private float fadeDelay = 0;
    private float textDilation = 0;

    private void Awake()
    {
        text = transform.GetComponent<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

        textDilation = text.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);
        textDilation = 0;
    }

    public IEnumerator FadeTextToFullAlpha(float t)
    {
        yield return new WaitForSeconds(fadeDelay);

        isFading = false;
        isFading = true;

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

        float time = 0;
        time += Time.deltaTime / t;

        while (text.color.a <= 1f || isFading)
        {
            textDilation++;
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + time);
            yield return null;
        }

        yield break;
    }

    public IEnumerator FadeTextToZeroAlpha(float t)
    {
        isFading = false;

        isFading = true;

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        float time = 0;
        time += Time.deltaTime / t;

        while (text.color.a >= 0f || isFading)
        {
            textDilation--;
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - time);
            yield return null;
        }

        DisableTitleText();

        yield break;
    }

    public void DisableTitleText()
    {
        transform.gameObject.SetActive(false);
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpImageColour : MonoBehaviour
{
    private Image image;
    private Color colour;
    private Color targetColour;

    [SerializeField] private float colourFactor = 0.1f;
    [SerializeField] private float duration = 1f;

    void OnEnable()
    {
        image = transform.GetComponent<Image>();
        colour = image.color;

        float r = colour.r + colourFactor;
        float g = colour.g + colourFactor;
        float b = colour.b + colourFactor;
        float a = colour.a;

        targetColour = new Color(r, g, b, a);

        StartCoroutine(GlowUp());
    }

    private IEnumerator GlowUp()
    {
        float time = 0;

        while (time <= 1f)
        {
            time += Time.deltaTime / (duration / 2);
            image.color = Color.Lerp(colour, targetColour, time);

            yield return null;
        }

        if (time >= 1f)
        {
            StartCoroutine(GlowDown());
        }

        yield break;
    }

    private IEnumerator GlowDown()
    {
        float time = 0;

        while (time <= 1f)
        {
            time += Time.deltaTime / (duration / 2);
            image.color = Color.Lerp(targetColour, colour, time);

            yield return null;
        }

        if (time >= 1f)
        {
            StartCoroutine(GlowUp());
        }

        yield break;
    }
}

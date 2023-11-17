using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TitleUIControl : MonoBehaviour
{
    [SerializeField] private Vector2 alphaRange = new Vector2(0f, 1f);
    [SerializeField] private Vector2 dilationRange = new Vector2(-1f, -0.3f);

    private TextMeshProUGUI text;
    public Material SkyMaterial; // Reference to the Sky Material

    [SerializeField] private bool isFading = false;

    [SerializeField] private float fadeDelay = 0;

    private IEnumerator fadeToFullAlphaCoroutine;
    private IEnumerator fadeToZeroAlphaCoroutine;
    private Coroutine textWobbleCoroutine;

    [SerializeField] private float wobbleRadius = 1f;
    [SerializeField] private float wobbleSpeed = 1f;

    private void Awake()
    {
        text = transform.GetComponent<TextMeshProUGUI>();
        text.color = new Color(text.color.r, text.color.g, text.color.b, alphaRange.x);

        text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilationRange.x);
    }

    private void OnEnable()
    {
        if (text == null)
        {
            text = transform.GetComponent<TextMeshProUGUI>();
            StartTextWobbleAnimation(wobbleRadius, wobbleSpeed); // Example values for radius and speed
        }

        if (SkyMaterial != null && text != null)
        {
            // Get the sky color from the material
            Color skyColor = SkyMaterial.GetColor("_SkyColour");
            skyColor.a = alphaRange.y; // Set alpha to maximum

            // Set the gradient color for the text
            text.colorGradient = new VertexGradient(Color.white, Color.white, skyColor, skyColor);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alphaRange.y);

            // If in edit mode, set dilation to maximum
            if (!Application.isPlaying)
            {
                text.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilationRange.y);
            }
        }
    }

    private void Update()
    {
        if (SkyMaterial != null && text != null)
        {
            // Get the sky color from the material
            Color skyColor = SkyMaterial.GetColor("_SkyColour");

            // If in edit mode, set alpha to max
            if (!Application.isPlaying)
            {
                skyColor.a = 1f; // Set alpha to maximum
            }

            // Set the gradient color for the text
            text.colorGradient = new VertexGradient(Color.white, Color.white, skyColor, skyColor);
        }
    }

    public IEnumerator FadeTextToFullAlpha(float t)
    {
        text.enabled = true;

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
        text.enabled = true;

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
        StopTextWobbleAnimation();
        transform.gameObject.SetActive(false);
    }

    private IEnumerator AnimateTextCharacters(float radius, float speed)
    {
        while (text != null && text.gameObject.activeInHierarchy)
        {
            TMP_TextInfo textInfo = text.textInfo;
            if (textInfo == null)
            {
                Debug.LogError("textInfo is null");
                yield break;
            }

            TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

            for (int i = 0; i < textInfo.characterCount; ++i)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int materialIndex = charInfo.materialReferenceIndex;
                int vertexIndex = charInfo.vertexIndex;

                Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
                // Apply a continuous and changing offset
                Vector3 offset = Random.insideUnitSphere * radius * Mathf.Sin(Time.time * speed);

                textInfo.meshInfo[materialIndex].vertices[vertexIndex + 0] = sourceVertices[vertexIndex + 0] + offset;
                textInfo.meshInfo[materialIndex].vertices[vertexIndex + 1] = sourceVertices[vertexIndex + 1] + offset;
                textInfo.meshInfo[materialIndex].vertices[vertexIndex + 2] = sourceVertices[vertexIndex + 2] + offset;
                textInfo.meshInfo[materialIndex].vertices[vertexIndex + 3] = sourceVertices[vertexIndex + 3] + offset;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; ++i)
            {
                textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
                text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            }

            // A smaller yield time for more frequent updates
            yield return new WaitForSeconds(0.05f);
        }
    }



    public void StartTextWobbleAnimation(float radius, float speed)
    {
        // Stop existing coroutine if it's running
        if (textWobbleCoroutine != null)
        {
            StopCoroutine(textWobbleCoroutine);
        }
        textWobbleCoroutine = StartCoroutine(AnimateTextCharacters(radius, speed));
    }

    public void StopTextWobbleAnimation()
    {
        if (textWobbleCoroutine != null)
        {
            StopCoroutine(textWobbleCoroutine);
            textWobbleCoroutine = null;
        }
    }

}
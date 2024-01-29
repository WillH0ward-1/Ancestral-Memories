using UnityEngine;
using TMPro;
using System.Collections;
using Random = UnityEngine.Random;
using System;

public class EmotionTextAnimator : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private Vector3 originalScale;
    private Vector3 originalPosition;

    public Dialogue dialogueReference;

    public float joyWaveAmplitude = 3f;
    public float joyWaveFrequency = 0.5f;
    public float fearShakeAmount = 2f;
    public float curiosityRotationAmount = 15f;
    public float contentmentScaleAmount = 0.05f;
    public float contentmentScaleFrequency = 2f;
    public float alertnessScale = 1.5f;
    public float sadnessMoveAmount = -10f;
    public float praiseFloatAmount = 3f;
    public float insaneShakeRange = 5f;
    public float springGrowthAmount = 2f;
    public float summerPulseAmount = 0.5f;
    public float summerPulseFrequency = 2f;
    public float autumnFallAmount = 2f;
    public float winterShimmerAmount = 0.5f;
    public float winterShimmerFrequency = 3f;

    private void Awake()
    {
        dialogueReference = GetComponent<Dialogue>();
        textComponent = dialogueReference.textComponent;
    }

    private DialogueLines.Emotions lastCheckedEmotion;

    private void Start()
    {
        if (dialogueReference == null)
        {
            dialogueReference = GetComponent<Dialogue>();
        }
        StartCoroutine(CheckForEmotionChanges());
    }

    private void OnEnable()
    {
        dialogueReference.OnTextComponentChanged += HandleTextComponentChanged;
    }

    private void OnDisable()
    {
        dialogueReference.OnTextComponentChanged -= HandleTextComponentChanged;
        StopCoroutine(CheckForEmotionChanges());
    }

    private void HandleTextComponentChanged(TextMeshProUGUI newTextComponent)
    {
        textComponent = newTextComponent;

        if (textComponent != null)
        {
            originalScale = textComponent.transform.localScale;
            originalPosition = textComponent.transform.position;
        }
    }


    IEnumerator CheckForEmotionChanges()
    {
        while (true)
        {
            if (dialogueReference.dialogueActive && dialogueReference.CurrentEmotion != lastCheckedEmotion)
            {
                ApplyEmotionEffect(dialogueReference.CurrentEmotion);
                lastCheckedEmotion = dialogueReference.CurrentEmotion;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private Coroutine currentEffectCoroutine;

    public void ApplyEmotionEffect(DialogueLines.Emotions emotion)
    {
        if (currentEffectCoroutine != null)
        {
            StopCoroutine(currentEffectCoroutine);
        }

        switch (emotion)
        {
            case DialogueLines.Emotions.Joy:
                currentEffectCoroutine = StartCoroutine(JoyEffect());
                break;
            case DialogueLines.Emotions.Fear:
                currentEffectCoroutine = StartCoroutine(FearEffect());
                break;
            case DialogueLines.Emotions.Curiosity:
                currentEffectCoroutine = StartCoroutine(CuriosityEffect());
                break;
            case DialogueLines.Emotions.Contentment:
                currentEffectCoroutine = StartCoroutine(ContentmentEffect());
                break;
            case DialogueLines.Emotions.Alertness:
                currentEffectCoroutine = StartCoroutine(AlertnessEffect());
                break;
            case DialogueLines.Emotions.Sadness:
                currentEffectCoroutine = StartCoroutine(SadnessEffect());
                break;
            case DialogueLines.Emotions.Praise:
                currentEffectCoroutine = StartCoroutine(PraiseEffect());
                break;
            case DialogueLines.Emotions.Insane:
                currentEffectCoroutine = StartCoroutine(InsaneEffect());
                break;
            case DialogueLines.Emotions.SeasonsSpring:
                currentEffectCoroutine = StartCoroutine(SeasonsSpringEffect());
                break;
            case DialogueLines.Emotions.SeasonsSummer:
                currentEffectCoroutine = StartCoroutine(SeasonsSummerEffect());
                break;
            case DialogueLines.Emotions.SeasonsAutumn:
                currentEffectCoroutine = StartCoroutine(SeasonsAutumnEffect());
                break;
            case DialogueLines.Emotions.SeasonsWinter:
                currentEffectCoroutine = StartCoroutine(SeasonsWinterEffect());
                break;
            default:
                ResetEffects();
                break;
        }
    }


    struct VertexModification
    {
        public float xModifier;
        public float yModifier;
        public float zModifier;

        public VertexModification(float x = 0, float y = 0, float z = 0)
        {
            this.xModifier = x;
            this.yModifier = y;
            this.zModifier = z;
        }
    }

    IEnumerator ApplyEffectToText(Func<TMP_CharacterInfo, int, VertexModification> effectFunc)
    {
        TMP_TextInfo textInfo = textComponent.textInfo;

        while (true)
        {
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                VertexModification modification = effectFunc(charInfo, i);

                int materialIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                for (int j = 0; j < 4; j++)
                {
                    vertices[charInfo.vertexIndex + j].x += modification.xModifier;
                    vertices[charInfo.vertexIndex + j].y += modification.yModifier;
                    vertices[charInfo.vertexIndex + j].z += modification.zModifier;
                }
            }
            textComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
            yield return null;
        }
    }

    IEnumerator JoyEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float offset = Mathf.Sin(Time.time + i * joyWaveFrequency) * joyWaveAmplitude;
            return new VertexModification(0, offset);
        });
    }

    IEnumerator FearEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float randX = Random.Range(-fearShakeAmount, fearShakeAmount);
            float randY = Random.Range(-fearShakeAmount, fearShakeAmount);
            return new VertexModification(randX, randY);
        });
    }

    IEnumerator CuriosityEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float rotation = Mathf.Sin(Time.time + i) * curiosityRotationAmount;
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, rotation), Vector3.one);
            Vector3 transformedPoint = matrix.MultiplyPoint3x4(new Vector3(0, 0, 0));
            return new VertexModification(transformedPoint.x, transformedPoint.y);
        });
    }

    IEnumerator ContentmentEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float scale = 1 + Mathf.Sin(Time.time * contentmentScaleFrequency + i) * contentmentScaleAmount;
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, 1));
            Vector3 transformedPoint = matrix.MultiplyPoint3x4(new Vector3(0, 0, 0));
            return new VertexModification(transformedPoint.x, transformedPoint.y);
        });
    }

    IEnumerator AlertnessEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(alertnessScale, alertnessScale, 1));
            Vector3 transformedPoint = matrix.MultiplyPoint3x4(new Vector3(0, 0, 0));
            return new VertexModification(transformedPoint.x, transformedPoint.y);
        });
    }

    IEnumerator SadnessEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            Vector3 moveVector = new Vector3(0, Mathf.Sin(Time.time) * sadnessMoveAmount, 0);
            return new VertexModification(moveVector.x, moveVector.y);
        });
    }

    IEnumerator PraiseEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float floatAmount = Mathf.Sin(Time.time + i) * praiseFloatAmount;
            return new VertexModification(0, floatAmount);
        });
    }

    IEnumerator InsaneEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float randX = Random.Range(-insaneShakeRange, insaneShakeRange);
            float randY = Random.Range(-insaneShakeRange, insaneShakeRange);
            return new VertexModification(randX, randY);
        });
    }

    IEnumerator SeasonsSpringEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float growth = Mathf.Abs(Mathf.Sin(Time.time + i)) * springGrowthAmount;
            return new VertexModification(0, growth);
        });
    }

    IEnumerator SeasonsSummerEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float pulse = Mathf.Sin(Time.time * summerPulseFrequency + i) * summerPulseAmount;
            return new VertexModification(pulse, pulse);
        });
    }

    IEnumerator SeasonsAutumnEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float fall = Mathf.Sin(Time.time + i) * autumnFallAmount;
            return new VertexModification(0, -fall); // Negative for descending effect.
        });
    }

    IEnumerator SeasonsWinterEffect()
    {
        return ApplyEffectToText((charInfo, i) =>
        {
            float shimmer = Mathf.Sin(Time.time * winterShimmerFrequency + i) * winterShimmerAmount;
            float randX = Random.Range(-shimmer, shimmer);
            float randY = Random.Range(-shimmer, shimmer);
            return new VertexModification(randX, randY);
        });
    }


    private void ResetEffects()
    {
        textComponent.transform.localScale = originalScale;
        textComponent.transform.localPosition = originalPosition;
    }
}

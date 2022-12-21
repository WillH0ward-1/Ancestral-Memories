using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shake : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;

    public bool start = false;
    public AnimationCurve curve;
    public float shakeDuration = 1f;
    public float shakeStrengthMultiplier;

    private void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/Earthquake");
        StartCoroutine(SmoothScreenShake(shakeDuration, shakeStrengthMultiplier));
    }


    public IEnumerator ScreenShake(float duration, float strengthMultiplier)
    {
        Vector3 startPosition = transform.position;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float strength = curve.Evaluate(time / duration);
            strength *= strengthMultiplier;
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;

        yield break;
    }

    public IEnumerator SmoothScreenShake(float duration, float strengthMultiplier)
    {
        Vector3 startPosition = transform.position;

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime / duration;
            float strength = curve.Evaluate(time / duration);
            strength *= strengthMultiplier;
            transform.position += Vector3.Lerp(transform.position, startPosition + Random.insideUnitSphere * strength, time);
            yield return null;
        }

        transform.position = startPosition;

        yield break;
    }
}

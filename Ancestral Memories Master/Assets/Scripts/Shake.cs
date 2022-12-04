using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shake : MonoBehaviour
{
    private FMOD.Studio.EventInstance instance;

    public bool start = false;
    public AnimationCurve curve;
    public float duration = 1f;

    private void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance("event:/Earthquake");
    }

    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking(duration));
        }
    }

    IEnumerator Shaking(float duration)
    {
        instance.start();

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
            instance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }
        transform.position = startPosition;
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
}

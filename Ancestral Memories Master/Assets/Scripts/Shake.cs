using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{

    public bool start = false;
    public AnimationCurve curve;
    public float shakeDuration = 1f;
    public float shakeStrengthMultiplier;

    public bool screenShakeEnabled = false;

    private void Start()
    {
        //instance = FMODUnity.RuntimeManager.CreateInstance("event:/Earthquake");
        //StartCoroutine(Float(transform.gameObject));
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

    [SerializeField]
    private float amplitude = 0.2f;

    [SerializeField]
    private float period = 1f;

    private bool shaking = false;

    float bobObjectX;
    float bobObjectY;
    float bobObjectZ;

    float targetY;

    public IEnumerator Float(GameObject bobObject)
    {
        shaking = true;

        bobObjectX = bobObject.transform.position.x;
        bobObjectY = bobObject.transform.position.y;
        bobObjectZ = bobObject.transform.position.z;

        targetY = bobObjectY - bobObjectY / 5;

        float time = 0;
        while (shaking)
        {
            time += Time.deltaTime;

            float theta = time / period;
            float distance = amplitude * Mathf.Sin(theta);
            bobObject.transform.position = bobObject.transform.position + (Vector3.up * distance);

            yield return null;
        }

        yield break;
    
    }
}


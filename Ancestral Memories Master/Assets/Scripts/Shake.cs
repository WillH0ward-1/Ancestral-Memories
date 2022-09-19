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
            StartCoroutine(Shaking());
        }
    }

    IEnumerator Shaking()
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
}

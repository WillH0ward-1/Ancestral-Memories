using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeatherControl : MonoBehaviour
{

    private FMOD.Studio.EventInstance instance;

    public FMODUnity.EventReference fmodEvent;

    [SerializeField]
    [Range(-0f, 1f)]
    private float pitch;

    private bool isRaining = false;
    private bool isThunder = false;
    private bool isSnowing = false;

    private float windStrength = 0;

    public float windDuration = 0;

    public float retriggerBuffer = 1;
    // Start is called before the first frame update
    void Start()
    {
        instance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);
        instance.start();

        StartCoroutine(WindStrength());
    }

    private float currentWindStrength = 0;
    private float targetWindStrength;

    private IEnumerator WindStrength()
    {
        targetWindStrength = Random.Range(0, 1);
        windDuration = Random.Range(1, 5);

        float timeElapsed = 0;

        while (timeElapsed <= windDuration)
        {
            float windStrength = Mathf.Lerp(currentWindStrength, targetWindStrength, timeElapsed / windDuration);

            timeElapsed += Time.deltaTime;

            instance.setParameterByName("Pitch", windStrength);

            yield return new WaitForSeconds(retriggerBuffer);

            StartCoroutine(WindStrength());
        }
    }
}

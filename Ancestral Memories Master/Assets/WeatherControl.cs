using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherControl : MonoBehaviour
{
    const string WIND_LOOP = ("event:/Wind");

    private FMOD.Studio.EventInstance instance;

    public FMODUnity.EventReference windEvent;

    [SerializeField]
    [Range(-0f, 1f)]
    private float pitch;

    private bool isRaining = false;
    private bool isThunder = false;
    private bool isSnowing = false;

    private float windStrength = 0;

    public float windDuration = 0;

    public float retriggerBuffer = 1;

    private float currentWindStrength = 0;
    private float targetWindStrength;

    // Start is called before the first frame update
    void Start()
    {
        StartFMODInstance(WIND_LOOP);

        /*
        StartCoroutine(WindStrength());
        */
    }

    private void StartFMODInstance(string instance)
    {
        FMOD.Studio.EventInstance sound = FMODUnity.RuntimeManager.CreateInstance(instance);
        sound.start();
    }


    /*
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
    */
}

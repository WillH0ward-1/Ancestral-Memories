using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class WeatherControl : MonoBehaviour
{
    public EventReference windEvent;

    private float windStrength = 0;

    private int minWindStrength = 0;
    private int maxWindStrength = 1;

    private float currentWindStrength = 0;
    private float targetWindStrength;

    public bool windIsActive;

    // Start is called before the first frame update
    void Start()
    {
        EventInstance windSFX = RuntimeManager.CreateInstance(windEvent);
        windStrength = currentWindStrength;
        StartCoroutine(WindStrength(windSFX));
    }


    private IEnumerator WindStrength(EventInstance windSFX)
    {
        windIsActive = true;

        windSFX.start();

        while (windIsActive)
        {
            windStrength = targetWindStrength;

            windSFX.setParameterByName("WindStrength", windStrength);
            Debug.Log("WindStrength:" + windStrength);
            yield return null;
        }

        if (!windIsActive)
        {
            StartCoroutine(StopWind(windSFX));
            yield break;
        }
    }

    private IEnumerator StopWind(EventInstance windSFX)
    {
        windSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        windIsActive = false;
        yield break;
    }

    [SerializeField]
    private CharacterClass player;

    private void OnEnable() => player.OnFaithChanged += WindStrength;
    private void OnDisable() => player.OnFaithChanged -= WindStrength;

    private void WindStrength(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float output = Mathf.Lerp(minWindStrength, maxWindStrength, t);

        targetWindStrength = output;
    }

}

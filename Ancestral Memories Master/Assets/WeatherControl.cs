using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class WeatherControl : MonoBehaviour
{
    public EventReference windEvent;

    public float windStrength = 0;

    private int minWindStrength = 0;
    private int maxWindStrength = 1;

    private float currentWindStrength = 0;
    private float targetWindStrength;

    public bool windIsActive;

    private MapObjGen mapObjGen;

    private List<GameObject> mapObjectList;

    Renderer[] renderers;

   [SerializeField] private List<Renderer> windAffectedRenderers;

    // Start is called before the first frame update
    void Start()
    {
        EventInstance windSFX = RuntimeManager.CreateInstance(windEvent);
        windStrength = currentWindStrength;
        StartCoroutine(WindStrength(windSFX));
       
        foreach (GameObject mapObject in mapObjGen.mapObjectList)
        {
             renderers = mapObject.GetComponentsInChildren<Renderer>();

     
        }
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


            foreach (Renderer r in renderers)
            {
                r.material.SetFloat("WindStrength", windStrength);
            }

            

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

    float newMin = 0;
    float newMax = 1;

    [SerializeField]
    private CharacterClass player;

    private void OnEnable() => player.OnFaithChanged += WindStrength;
    private void OnDisable() => player.OnFaithChanged -= WindStrength;

    private void WindStrength(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float output = Mathf.Lerp(newMin, newMax, t);

        targetWindStrength = output;
    }

}

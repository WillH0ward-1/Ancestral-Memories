using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Linq;

public class WeatherControl : MonoBehaviour
{
    public EventReference windEvent;

    public float windStrength = 0;

    private int minWindStrength = 0;
    private int maxWindStrength = 1;

    private float currentWindStrength = 0;

    [SerializeField] private float targetWindStrength;
    [SerializeField] private float targetLeafShakeStrength;

    public bool windIsActive;

    private MapObjGen mapObjGen;

    private List<GameObject> mapObjectList;

    private Renderer[] windAffectedRenderers;

    //[SerializeField] private List<Renderer> windAffectedRenderers;

    // Start is called before the first frame update

    public List<Transform> windAffectedRendererList = new List<Transform>();

    void Start()
    {
        ListCleanup();
        EventInstance windSFX = RuntimeManager.CreateInstance(windEvent);
        windStrength = 0;
        StartCoroutine(WindStrength(windSFX));
     
    }

    void ListCleanup()
    {
        for (var i = windAffectedRendererList.Count - 1; i > -1; i--)
        {
            if (windAffectedRendererList[i] == null)
                windAffectedRendererList.RemoveAt(i);
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

            foreach (Transform t in windAffectedRendererList)
            {
                foreach (Material m in t.GetComponentInChildren<Renderer>().sharedMaterials)
                {
                    m.SetFloat("_NoiseFactor", targetLeafShakeStrength);
                }
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

    [SerializeField] float newMin = 0;
    [SerializeField] float newMax = 1;

    [SerializeField] float leafShakeMin = 0;
    [SerializeField] float leafShakeMax = 1000;

    [SerializeField]
    private CharacterClass player;

    private void OnEnable() => player.OnFaithChanged += WindStrength;
    private void OnDisable() => player.OnFaithChanged -= WindStrength;

    private void WindStrength(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float output = Mathf.Lerp(newMin, newMax, t);
        float leafOutput = Mathf.Lerp(leafShakeMin, leafShakeMax, t);

        targetWindStrength = output;
        targetLeafShakeStrength = leafOutput;
    }

}

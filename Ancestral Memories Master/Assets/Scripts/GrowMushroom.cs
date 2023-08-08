using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class MushroomGrowth : MonoBehaviour
{
    [SerializeField] private float minGrowDelay = 5;
    [SerializeField] private float maxGrowDelay = 10;
    [SerializeField] private float minGrowDuration = 1;
    [SerializeField] private float maxGrowDuration = 5;
    [SerializeField] private Vector3 growScale = new Vector3(0.5f, 0.5f, 0.5f);
    [SerializeField] private Vector3 shrinkScale = new Vector3(0.00001f, 0.00001f, 0.00001f);
    [SerializeField] private float minShrinkDuration = 1;
    [SerializeField] private float maxShrinkDuration = 5;

    [SerializeField] private EventReference growthEvent;
    private EventInstance growthInstance;
    private MapObjGen mapObjGen;
    private ScaleControl scaleControl;
    private SeasonManager seasonManager;

    public bool growMushrooms = true;

    public Player player;
    private GameObject mushroom;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        seasonManager = FindObjectOfType<SeasonManager>();
        mapObjGen = FindObjectOfType<MapObjGen>();
        mushroom = transform.gameObject;
    }

    private void Start()
    {
        scaleControl = transform.GetComponent<ScaleControl>();
        StartCoroutine(GrowAndShrink());
    }

    private IEnumerator GrowMushroom()
    {
        yield return new WaitForSeconds(Random.Range(minGrowDelay, maxGrowDelay));

        if (player.faith > player.maxStat / 2 && seasonManager._currentSeason != SeasonManager.Season.Winter)
        {
            growMushrooms = true;
            growthInstance = RuntimeManager.CreateInstance(growthEvent);
            RuntimeManager.AttachInstanceToGameObject(growthInstance, transform);

            growthInstance.start();
            mapObjGen.foodSourcesList.Add(mushroom);
            yield return StartCoroutine(scaleControl.LerpScale(transform.gameObject, shrinkScale, growScale, Random.Range(minGrowDuration, maxGrowDuration), 0));
            growthInstance.release();
        }
    }

    private IEnumerator ShrinkMushroom()
    {
        float lifetime = Random.Range(minGrowDelay, maxGrowDelay);
        float elapsedTime = 0f;

        while (elapsedTime < lifetime && player.faith > player.maxStat / 2 && seasonManager._currentSeason != SeasonManager.Season.Winter)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (growMushrooms || seasonManager._currentSeason == SeasonManager.Season.Winter)
        {
            growMushrooms = false;
            mapObjGen.foodSourcesList.Remove(mushroom);
            yield return StartCoroutine(scaleControl.LerpScale(transform.gameObject, growScale, shrinkScale, Random.Range(minShrinkDuration, maxShrinkDuration), 0));
        }
    }

    private IEnumerator GrowAndShrink()
    {
        while (true)
        {
            if (player.faith > player.maxStat / 2 && seasonManager._currentSeason != SeasonManager.Season.Winter)
            {
                yield return StartCoroutine(GrowMushroom());
            }

            if (seasonManager._currentSeason == SeasonManager.Season.Winter || player.faith <= player.maxStat / 2)
            {
                if (growMushrooms)
                {
                    StopCoroutine(GrowMushroom());
                    StartCoroutine(ShrinkMushroom());
                }
            }
            yield return null;
        }
    }
}

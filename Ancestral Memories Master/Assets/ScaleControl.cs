using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleControl : MonoBehaviour
{
    [SerializeField] private float xAxisGrowMultiplier = 1f;
    [SerializeField] private float yAxisGrowMultiplier = 1f;
    [SerializeField] private float zAxisGrowMultiplier = 1f;

    public bool isFullyGrown = false;

    public GameObject scaleObjectRef;
    public float durationRef;
    public Vector3 scaleStartRef;
    public Vector3 scaleDestinationRef;
    public float delayRef;

    public float growthPercent;

    private TreeDeathManager treeKillManager;
    private KillBuffer killBuffer;

    public RainControl rainControl;
    public TreeAudioSFX treeAudio;

    public bool ignoreLayer = false;


    //[SerializeField] private RainControl rain;

    private void Awake()
    {
        if (transform.CompareTag("Trees"))
        {
            treeKillManager = transform.GetComponent<TreeDeathManager>();
            killBuffer = transform.GetComponent<KillBuffer>();
            treeAudio = transform.GetComponent<TreeAudioSFX>();
        } else
        {
            return;
        }
    }

    float time;

    public IEnumerator LerpScale(GameObject scaleObject, Vector3 scaleStart, Vector3 scaleDestination, float duration, float delay)
    {

        if (scaleObject == null)
        {
            yield break;
        }

        else if (scaleObject != null)
        {
            isFullyGrown = false;

            if (scaleObject.transform.CompareTag("Trees") && !treeKillManager.treeFalling)
            {
                treeAudio.StartCoroutine(treeAudio.StartTreeGrowthSFX());
            }
       
            durationRef = duration;
            scaleObjectRef = scaleObject;
            scaleStartRef = scaleStart;
            scaleDestinationRef = scaleDestination;
            delayRef = delay;

            scaleObject.transform.localScale = scaleStart;

            yield return new WaitForSeconds(delay);

            time = 0f;

            float localScaleX = scaleObject.transform.localScale.x;
            float localScaleY = scaleObject.transform.localScale.y;
            float localScaleZ = scaleObject.transform.localScale.z;

            float rainMultiplier = 1f;
            float initialMultiplier = 1f;

            while (time <= 1f) //&& rain.isRaining)
            {
                if (rainControl.isRaining) //&& transform.CompareTag("Trees"))
                {
                    rainMultiplier = 20f;
                } else
                {
                    rainMultiplier = 1f;
                }
                
                time += Time.deltaTime * rainMultiplier * initialMultiplier / duration;

                growthPercent = time;

                if (scaleObject.transform.CompareTag("Trees") && !treeKillManager.treeFalling)
                {
                    localScaleX = Mathf.Lerp(scaleStart.x, scaleDestination.x, time * xAxisGrowMultiplier);
                    localScaleY = Mathf.Lerp(scaleStart.y, scaleDestination.y, time * yAxisGrowMultiplier);
                    localScaleZ = Mathf.Lerp(scaleStart.z, scaleDestination.z, time * zAxisGrowMultiplier);
                } else
                {
                    localScaleX = Mathf.Lerp(scaleStart.x, scaleDestination.x, time);
                    localScaleY = Mathf.Lerp(scaleStart.y, scaleDestination.y, time);
                    localScaleZ = Mathf.Lerp(scaleStart.z, scaleDestination.z, time);
                }

                scaleObject.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);

                if (growthPercent <= 0.25f)
                {
                    ignoreLayer = true;
                    initialMultiplier = 25f;
                }
                else if (growthPercent >= 0.25f)
                {
                    ignoreLayer = false;
                    initialMultiplier = 1;
                }

                yield return null;

            }

            if (time >= 1f || scaleObject.transform.localScale == scaleDestination)
            {
                //scaleObject.transform.localScale = scaleDestination;

                isFullyGrown = true;

                if (transform.CompareTag("Trees") && !treeKillManager.treeDead)
                {
                    killBuffer.StartCoroutine(killBuffer.ExpiryBuffer());
                }

                yield break;
            }
        }
    }
}

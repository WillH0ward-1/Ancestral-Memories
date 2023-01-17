using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleControl : MonoBehaviour
{
    [SerializeField] private float xAxisGrowMultiplier = 1;
    [SerializeField] private float yAxisGrowMultiplier = 1;
    [SerializeField] private float zAxisGrowMultiplier = 1;

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
            if (scaleObject.transform.CompareTag("Trees") && !treeKillManager.treeFalling)
            {
                treeAudio.StartCoroutine(treeAudio.StartTreeGrowthSFX());
            }

            isFullyGrown = false;
       
            durationRef = duration;
            scaleObjectRef = scaleObject;
            scaleStartRef = scaleStart;
            scaleDestinationRef = scaleDestination;
            delayRef = delay;

            scaleObject.transform.localScale = scaleStart;

            yield return new WaitForSeconds(delay);

            time = 0;

            float localScaleX = scaleObject.transform.localScale.x;
            float localScaleY = scaleObject.transform.localScale.y;
            float localScaleZ = scaleObject.transform.localScale.z;

            float rainMultiplier = 1;

            while (time <= 1f) //&& rain.isRaining)
            {
                if (rainControl.isRaining) //&& transform.CompareTag("Trees"))
                {
                    rainMultiplier = 20;
                } else
                {
                    rainMultiplier = 1;
                }
                
                time += Time.deltaTime * rainMultiplier / duration;

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

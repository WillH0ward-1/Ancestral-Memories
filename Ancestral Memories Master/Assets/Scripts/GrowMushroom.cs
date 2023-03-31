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

    private ScaleControl scaleControl;

    public bool growMushrooms = true;

    public Player player;

    private void Start()
    {
        scaleControl = transform.GetComponent<ScaleControl>();

        StartCoroutine(WaitForPlayerAssignmentAndGrowShrink());
    }

    private IEnumerator WaitForPlayerAssignmentAndGrowShrink()
    {
        while (player == null) 
        {
            yield return null;
        }

        StartCoroutine(GrowAndShrink());
    }


    private IEnumerator GrowAndShrink()
    {
        while (true)
        {
            if (player.faith > 50)
            {
                // Start grow buffer
                yield return new WaitForSeconds(Random.Range(minGrowDelay, maxGrowDelay));

                // Grow the mushroom if player faith is still above 50
                if (player.faith > 50)
                {
                    growMushrooms = true;
                    growthInstance = RuntimeManager.CreateInstance(growthEvent);
                    RuntimeManager.AttachInstanceToGameObject(growthInstance, transform);

                    growthInstance.start();
                    yield return StartCoroutine(scaleControl.LerpScale(transform.gameObject, shrinkScale, growScale, Random.Range(minGrowDuration, maxGrowDuration), 0));
                    growthInstance.release();

                    // Wait for the mushroom's Lifetime
                    float lifetime = Random.Range(minGrowDelay, maxGrowDelay);
                    float elapsedTime = 0f;

                    while (elapsedTime < lifetime && player.faith > 50)
                    {
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    // Shrink the mushroom
                    if (growMushrooms)
                    {
                        growMushrooms = false;
                        yield return StartCoroutine(scaleControl.LerpScale(transform.gameObject, growScale, shrinkScale, Random.Range(minShrinkDuration, maxShrinkDuration), 0));
                    }
                }
            }
            else
            {
                if (growMushrooms)
                {
                    growMushrooms = false;
                    yield return StartCoroutine(scaleControl.LerpScale(transform.gameObject, growScale, shrinkScale, Random.Range(minShrinkDuration, maxShrinkDuration), 0));
                }

                yield return null;
            }
        }
    }
}
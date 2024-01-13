using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdDensity : MonoBehaviour
{
    [SerializeField] private FlockController birds;

    public int birdDensity = 0;

    private int currentBirdDensity = 0;
    private int targetBirdDensity;

    public bool birdsActive;

    [SerializeField] private Camera cam;
    [SerializeField] private MeshSettings meshSettings;

    [SerializeField] private TimeCycleManager todManager;

    private void Awake()
    {
        cam = Camera.main;
        player = FindObjectOfType<Player>();
    }

    void Start()
    {
        birds.cam = cam;
        birds._childAmount = birdDensity;
        birds._childAmount = currentBirdDensity;

        birds._spawnSphereDepth = meshSettings.meshWorldSize;

        StartCoroutine(GetBirdDensity());
    }

    private IEnumerator GetBirdDensity()
    {

        birdsActive = true;


        while (birdsActive)
        {
            birds._childAmount = targetBirdDensity;

//            Debug.Log("Bird Density:" + birdDensity);

            yield return null;
        }

        if (!birdsActive)
        {
            StartCoroutine(KillBirds());
            yield break;
        }
    }

    private IEnumerator KillBirds()
    {
        birdDensity = 0;
        birdsActive = false;
        yield break;
    }

    [SerializeField] float minBirdDensity = 2;
    [SerializeField] float maxBirdDensity = 250;

    [SerializeField]
    private Player player;

    private void OnEnable() => player.OnFaithChanged += BirdDensityFactor;
    private void OnDisable() => player.OnFaithChanged -= BirdDensityFactor;

    private void BirdDensityFactor(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        int output = (int)Mathf.Lerp(maxBirdDensity, minBirdDensity, t);

        if (!todManager.isNightTime)
        {
            targetBirdDensity = output;
        } else
        {
            targetBirdDensity = 0;
        }
    }

}

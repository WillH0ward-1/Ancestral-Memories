using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ParticleCollision : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Player player;
    private EventInstance instance;
    [SerializeField] private float currentWindStrength = 0;
    [SerializeField] private float stability = 0;
    [SerializeField] private float instability = 1;
    [SerializeField] private float currentHarmonicStability;
    [SerializeField] private float targetHarmonicStability;
    public bool windIsActive;
    private StudioGlobalParameterTrigger globalParams;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private Transform flowerParent;
    [SerializeField] private GameObject[] flowerPrefabs;
    public int maxPoolSize = 128;
    [SerializeField] private List<GameObject> pooledObjects;
    //private ScaleControl scaleControl;
    [SerializeField] private FlowerGrow flowerGrow;
    [SerializeField] private ScaleControl scaleControl;
    private void Awake()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject flower = Instantiate(flowerPrefabs[Random.Range(0, flowerPrefabs.Length)]);
            flower.transform.SetParent(flowerParent, true);
            flower.SetActive(false);
            pooledObjects.Add(flower);
        }
        foreach (GameObject flower in pooledObjects)
        {
            ScaleControl scaleControl = flower.transform.GetComponent<ScaleControl>();
            //FlowerGrow flowerGrow = flower.transform.GetComponent<FlowerGrow>();

            scaleControl.rainControl = rainManager;

        }
        StartCoroutine(HarmonicStability());
    }

    private void GenerateFlower(Vector3 position)
    {
        if (player.faith >= 50)
        {
            GameObject flower = GetPooledObject();
            FlowerGrow flowerGrow = flower.transform.GetComponent<FlowerGrow>();
            ScaleControl scaleControl = flower.transform.GetComponent<ScaleControl>();
            flower.transform.position = position;
            flower.transform.localScale = new(0, 0, 0);
            flower.SetActive(true);
            StartCoroutine(FlowerLifeTime(flowerGrow, scaleControl));
        }
        //flower.transform.position = position;

    }
    private IEnumerator FlowerLifeTime(FlowerGrow flowerGrow, ScaleControl scaleControl)
    {

        flowerGrow.GrowFlower();
        yield return new WaitUntil(() => scaleControl.isFullyGrown);
        flowerGrow.ShrinkFlower(); // The object is returned to the pool from here
        yield break;
    }

    public GameObject GetPooledObject()
    {
        if (pooledObjects.Count == 0)
        {
            return null;
        }
        for (int i = 0; i < maxPoolSize; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
    [SerializeField] private RainControl rainManager;
    private Vector3 particlePos;
    private GameObject particleGameObject;
    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    public MusicManager musicManager;

    private void OnParticleCollision(GameObject other)
    {
        collisionEvents.Clear();
        rainManager.rainParticles.GetCollisionEvents(other, collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in collisionEvents)
        {
            particlePos = collisionEvent.intersection;
            particleGameObject = collisionEvent.colliderComponent.gameObject;
        }

        Debug.Log("Particle hit ground!");
        // RaycastHit hitFloor;
        //var ray = Physics.Raycast(transform.position, Vector3.down, out hitFloor, Mathf.Infinity, groundLayerMask);
        Vector3 hitLocation = particlePos;
        Vector3 screenCoords = cam.WorldToViewportPoint(hitLocation);

        bool onScreen =     // ScottGameSounds! https://youtu.be/PGwq4K5j6B4?t=1673  
            screenCoords.x > 0 &&
            screenCoords.x < 1 &&
            screenCoords.y > 0 &&
            screenCoords.y < 1;

        GenerateFlower(particlePos);

        Debug.Log(hitLocation);

        musicManager.PlayOneShot(MusicManager.Instruments.PianoTail.ToString(), particleGameObject, true);

        //RuntimeManager.PlayOneShot(rainSFX, hitLocation);
        

        //lightningStrikeEvent.setVolume();
    }
    private bool rainIsActive = false;
    private bool harmonicStabilityActive;

    private IEnumerator HarmonicStability()
    {
        harmonicStabilityActive = true;

        while (harmonicStabilityActive)
        {
            if (!harmonicStabilityActive)
            {
                yield break;
            }
            currentHarmonicStability = targetHarmonicStability;

            yield return null;
        }

        yield break;
    }

    private void OnEnable()
    {
        player.OnFaithChanged += HarmonicStability;
    }

    private void OnDisable()
    {
        player.OnFaithChanged -= HarmonicStability;
    }

    private void HarmonicStability(float faith, float minFaith, float maxFaith)
    {
        var t = Mathf.InverseLerp(minFaith, maxFaith, faith);
        float output = Mathf.Lerp(instability, stability, t);
        targetHarmonicStability = output;
    }

}

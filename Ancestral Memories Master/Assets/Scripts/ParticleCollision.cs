using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ParticleCollision : MonoBehaviour
{

    [SerializeField] private Camera cam;
    [SerializeField] private Player player;

    [SerializeField] private EventReference rainSFX;
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

    private void Awake()
    {
        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject flower = Instantiate(flowerPrefabs[Random.Range(0, flowerPrefabs.Length)]);

            flower.transform.SetParent(flowerParent, true);
            flower.SetActive(false);
            pooledObjects.Add(flower);
        }

        StartCoroutine(HarmonicStability());
    }

    private void GenerateFlower(Vector3 position)
    {
        GameObject flower = GetPooledObject();
        //flower.transform.position = position;

        flower.transform.position = position;
        
        flower.SetActive(true);
        StartCoroutine(FlowerLifeTime(flower));
    }

    private IEnumerator FlowerLifeTime(GameObject flower)
    {
        float flowerLifeTime = Random.Range(5f, 10f);
        yield return new WaitForSeconds(flowerLifeTime);

        flower.SetActive(false); // Return to pool
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

    private List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    private void OnParticleCollision(GameObject other)
    {
        collisionEvents.Clear();
        rainManager.rainParticles.GetCollisionEvents(other, collisionEvents);

        foreach (ParticleCollisionEvent collisionEvent in collisionEvents)
        {
             particlePos = collisionEvent.intersection;
        }

        Debug.Log("Particle hit ground!");
        // RaycastHit hitFloor;
        //var ray = Physics.Raycast(transform.position, Vector3.down, out hitFloor, Mathf.Infinity, groundLayerMask);

        Vector3 hitLocation = particlePos;

        Vector3 screenCoords = cam.WorldToViewportPoint(hitLocation);

        bool onScreen =
            screenCoords.x > 0 &&
            screenCoords.x < 1 &&
            screenCoords.y > 0 &&
            screenCoords.y < 1;

        GenerateFlower(particlePos);

        if (onScreen)
        {
            Debug.Log(hitLocation);


            RuntimeManager.PlayOneShot(rainSFX, hitLocation);

        }
        

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

            RuntimeManager.StudioSystem.setParameterByName("HarmonicStability", currentHarmonicStability);
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

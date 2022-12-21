using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Deform;

public class MapObjGen : MonoBehaviour
{
    //public int density;

    [Header("Map Object Generator")]
    [Header("========================================================================================================================")]

    public GameObject terrain;

    [Header("Terrain Mesh Data")]
    [Space(10)]

    [SerializeField] private MeshData meshData;
    [SerializeField] private MeshSettings meshSettings;
    [SerializeField] private GameObject mapObject;
    [SerializeField] private GameObject hierarchyRoot;

    [SerializeField] private float sampleWidth = 0;
    [SerializeField] private float sampleHeight = 0;

    //private MeshFilter meshFilter;

    [Header("========================================================================================================================")]
    [Header("Spawnable Objects")]
    [Space(10)]

    [SerializeField] private GameObject[] trees;
    [SerializeField] private GameObject[] appleTrees;
    [SerializeField] private GameObject[] grass;
    [SerializeField] private GameObject[] foliage;
    [SerializeField] private GameObject[] rocks;
    [SerializeField] private GameObject[] mushrooms;
    [SerializeField] private GameObject[] flies;
    [SerializeField] private GameObject[] fish;
    [SerializeField] private GameObject[] animals;
    [SerializeField] private GameObject[] fireWood;
    [SerializeField] private GameObject[] seaShells;
    [SerializeField] private GameObject[] pedestals;

    [Header("========================================================================================================================")]
    [Header("Layer Masks")]
    [Space(10)]

    [SerializeField] private LayerMask caveLayerMask;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask deadZoneLayerMask;

    [Header("========================================================================================================================")]
    [Header("Object Scaling")]
    [Space(10)]

    [SerializeField] Vector3 minTreeScale;
    [SerializeField] Vector3 maxTreeScale;

    [SerializeField] Vector3 minAppleScale;
    [SerializeField] Vector3 maxAppleScale;

    [SerializeField] Vector3 minGrassScale;
    [SerializeField] Vector3 maxGrassScale;

    [SerializeField] Vector3 minFoliageScale;
    [SerializeField] Vector3 maxFoliageScale;

    [SerializeField] Vector3 minRockScale;
    [SerializeField] Vector3 maxRockScale;

    [SerializeField] Vector3 minMushroomScale;
    [SerializeField] Vector3 maxMushroomScale;

    [SerializeField] Vector3 minFliesScale;
    [SerializeField] Vector3 maxFliesScale;

    [SerializeField] Vector3 minFishScale;
    [SerializeField] Vector3 maxFishScale;

    [SerializeField] Vector3 minFireWoodScale;
    [SerializeField] Vector3 maxFireWoodScale;

    [SerializeField] Vector3 minSeaShellScale;
    [SerializeField] Vector3 maxSeaShellScale;

    [SerializeField] Vector3 minPedestalScale;
    [SerializeField] Vector3 maxPedestalScale;

    [Header("========================================================================================================================")]

    [Header("Object Rotation")]
    [Space(10)]

    [SerializeField, Range(0, 1)] float rotateTowardsNormal;
    [SerializeField] Vector2 rotationRange;

    [Header("========================================================================================================================")]

    [Header("Density")]
    [Space(10)]

    [SerializeField] float minimumTreeRadius = 70;
    [SerializeField] float minimumAppleTreeRadius = 70;
    [SerializeField] float minimumGrassRadius = 70;
    [SerializeField] float minimumFoliageRadius = 70;
    [SerializeField] float minimumRockRadius = 70;
    [SerializeField] float minimumMushroomRadius = 70;
    [SerializeField] float minimumFliesRadius = 70;
    [SerializeField] float minimumFishRadius = 70;
    [SerializeField] float minimumAnimalRadius = 70;
    [SerializeField] float minimumFireWoodRadius = 70;
    [SerializeField] float minimumSeaShellRadius = 70;
    [SerializeField] float minimumPedestalRadius = 70;

    [Header("========================================================================================================================")]

    [Header("Navmesh")]
    [Space(10)]

    [SerializeField] private NavMeshObstacle navMeshObstacle;
    [SerializeField] private NavMeshModifier navModifier;

    [SerializeField] float obstacleSizeX = 1;
    [SerializeField] float obstacleSizeY = 14;
    [SerializeField] float obstacleSizeZ = 1;

    [Header("========================================================================================================================")]

    private readonly string treeTag = "Trees";
    private readonly string appleTreeTag = "AppleTree";
    private readonly string waterTag = "Water";
    private readonly string groundTag = "Ground";
    private readonly string grassTag = "Grass";
    private readonly string foliageTag = "Foliage";
    private readonly string rockTag = "Rocks";
    private readonly string fliesTag = "Flies";
    private readonly string fishTag = "Fish";
    private readonly string animalTag = "Animal";
    private readonly string mushroomTag = "Mushrooms";
    private readonly string fireWoodTag = "FireWood";
    private readonly string seaShellTag = "SeaShell";
    private readonly string pedestalTag = "Pedestal";

    [Header("========================================================================================================================")]

    [Header("========================================================================================================================")]

    [Header("Positioning")]
    [Space(10)]

    [SerializeField] float yOffset;

    [SerializeField] private float mapSizeX = 0;
    [SerializeField] private float mapSizeY = 0;
    [SerializeField] private float mapSizeZ = 0;

    [SerializeField] private float xOffset = 0;
    [SerializeField] private float zOffset = 0;
    [SerializeField] private float initY = 0;

    [SerializeField] private Player player;

    public RadialMenu radialMenu;

    public Camera cam;

    [SerializeField] private Interactable interactable;
    [SerializeField] private Interactable interact;


    [SerializeField] private GameObject emitter;
    private GameObject emitterInstance;

    private Mesh mesh;

    int sampleDensity;

    [SerializeField] int vertSampleFactor;

    [SerializeField] private bool invertSpreadOrigin = false;

    private FireController fireManager;

    Vector3 scale;
    private Vector3 mapCenter;

    //public Interactable treeInteraction;


    public GameObject GetRandomMapObject(GameObject[] mapElements)
    {
        return mapElements[Random.Range(0, mapElements.Length)];
    }

    private void Awake()
    {
        mapCenter = Vector3.zero;
    }


    private Vector3[] vertices;

    public void GenerateMapObjects()
    {

       // ResetPosOffset(mapObject.transform);
        ResetPosOffset();

        sampleWidth = meshSettings.meshWorldSize; 
        sampleHeight = meshSettings.meshWorldSize; 

        xOffset = -sampleWidth / 2;
        zOffset = -sampleHeight / 2;

        //mapObjectList.Clear();

        //GenerateOceanEmitters(sampleWidth, sampleHeight);

        PoissonDiscSampler treeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumTreeRadius);
        PoissonDiscSampler appleTreeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumAppleTreeRadius);
        PoissonDiscSampler grassSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumGrassRadius);
        PoissonDiscSampler foliageSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFoliageRadius);
        PoissonDiscSampler rockSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumRockRadius);
        PoissonDiscSampler fliesSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFliesRadius);
        PoissonDiscSampler animalSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumAnimalRadius);
        PoissonDiscSampler mushroomSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumMushroomRadius);
        PoissonDiscSampler fireWoodSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFireWoodRadius);
        PoissonDiscSampler seaShellSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumSeaShellRadius);
        PoissonDiscSampler pedestalSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumPedestalRadius);

        TreePoissonDisc(treeSampler);
        AppleTreePoissonDisc(appleTreeSampler);
        GrassPoissonDisc(grassSampler);
        FoliagePoissonDisc(foliageSampler);
        RocksPoissonDisc(rockSampler);
        FliesPoissonDisc(fliesSampler);
        AnimalPoissonDisc(animalSampler);
        MushroomPoissonDisc(mushroomSampler);
        FireWoodPoissonDisc(fireWoodSampler);
        SeaShellPoissonDisc(seaShellSampler);
        PedestalPoissonDisc(pedestalSampler);

        SetOffset();

        GroundCheck();
        DestroyDeadZones();

        vertices = meshData.vertices;

    }

    private List<Vector3> XspawnPositions;
    private List<Vector3> ZspawnPositions;

    [SerializeField] GameObject EmitterHierarchyParent;
    [SerializeField] GameObject OceanSoundEmitter;

    void GenerateOceanEmitters(float sampleWidth, float sampleHeight)
    {
        float Z = sampleWidth / 2;
        float X = sampleHeight / 2;
        float initY = 0;

        Vector3 positiveX = new(X + xOffset, initY, 0);
        Vector3 positiveZ = new(0, initY, Z += zOffset);

        Vector3 negativeX = new(-X - zOffset, initY, 0);
        Vector3 negativeZ = new(0, initY, -Z);

        XspawnPositions.Add(positiveX);
        XspawnPositions.Add(negativeX);

        ZspawnPositions.Add(positiveZ);
        ZspawnPositions.Add(negativeZ);

        foreach (Vector3 spawnPoint in XspawnPositions)
        {
            GameObject perimeterPoint = Instantiate(OceanSoundEmitter, spawnPoint, Quaternion.identity, EmitterHierarchyParent.transform);
            mapObjectList.Add(perimeterPoint);
            perimeterPoint.transform.SetParent(hierarchyRoot.transform);
        }

        foreach (Vector3 spawnPoint in ZspawnPositions)
        {
            GameObject perimeterPoint = Instantiate(OceanSoundEmitter, spawnPoint, Quaternion.identity, EmitterHierarchyParent.transform);
            mapObjectList.Add(perimeterPoint);
            perimeterPoint.transform.SetParent(hierarchyRoot.transform);
        }
    }


    void AnimalPoissonDisc(PoissonDiscSampler animalSampler)
    {
        foreach (Vector2 sample in animalSampler.Samples())
        {
            GameObject randomAnimal = GetRandomMapObject(animals);

            GameObject animalInstance = Instantiate(randomAnimal, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            AnimalAI animalAI = animalInstance.GetComponent<AnimalAI>();

            animalAI.player = player;
        
            animalInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            animalInstance.tag = animalTag;

            int animalsLayer = LayerMask.NameToLayer("Animals");
            animalInstance.layer = animalsLayer;

            animalInstance.transform.SetParent(hierarchyRoot.transform);

            LerpDeformation deform = animalInstance.GetComponent<LerpDeformation>();
            CorruptionControl corruption = animalInstance.GetComponent<CorruptionControl>();

            deform.player = player;
            corruption.player = player;

            mapObjectList.Add(animalInstance);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

    void PedestalPoissonDisc(PoissonDiscSampler pedestalSampler)
    {
        foreach (Vector2 sample in pedestalSampler.Samples())
        {
            GameObject randomPedestal = GetRandomMapObject(pedestals);

            GameObject pedestalInstance = Instantiate(randomPedestal, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            pedestalInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            /*
            new Vector3(
            Random.Range(minTreeScale.x, maxTreeScale.x),
            Random.Range(minTreeScale.y, maxTreeScale.y),
            Random.Range(minTreeScale.z, maxTreeScale.z));
            */

            pedestalInstance.tag = pedestalTag;

            //int pedestalLayer = LayerMask.NameToLayer("Pedestal");
            //pedestalInstance.layer = pedestalLayer;

            mapObjectList.Add(pedestalInstance);

            pedestalInstance.transform.SetParent(hierarchyRoot.transform);


            //GroundCheck(instantiatedPrefab);
            //WaterCheck();

           // GrowTrees(treeInstance);

        }
    }


    private int distanceFromCenter;

    private float GetDistanceFromCenter(GameObject mapObject)
    {
        float distanceFromCenter = Vector3.Distance(mapObject.transform.position, mapCenter);

        return distanceFromCenter;
    }

    private float minDistanceFromCenter;

    void SeaShellPoissonDisc(PoissonDiscSampler seaShellSampler)
    {
        minDistanceFromCenter = sampleHeight / 2;

        foreach (Vector2 sample in seaShellSampler.Samples())
        {
            GameObject randomSeaShell = GetRandomMapObject(seaShells);

            GameObject seaShellInstance = Instantiate(randomSeaShell, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            seaShellInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            float distanceFromCenter = GetDistanceFromCenter(seaShellInstance);

            if (distanceFromCenter <= minDistanceFromCenter)
            {
                if (Application.isEditor)
                {
                    Debug.Log("Object destroyed in Editor.");
                    DestroyImmediate(seaShellInstance);
                }
                else
                {
                    Debug.Log("Object destroyed in game.");
                    Destroy(seaShellInstance);
                }

                continue;
            }

            if (distanceFromCenter >= minDistanceFromCenter)
            {

                seaShellInstance.transform.SetParent(hierarchyRoot.transform);

                mapObjectList.Add(seaShellInstance);
            }
            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

    [Header("Generated Objects")]
    [Space(10)]

    public List<GameObject> mapObjectList;
    public List<GameObject> treeList;

    private Vector3 zeroScale = new Vector3(0.001f, 0.001f, 0.001f);

    void TreePoissonDisc(PoissonDiscSampler treeSampler)
    {

        foreach (Vector2 sample in treeSampler.Samples())
        {
            GameObject randomTree = GetRandomMapObject(trees);

            GameObject treeInstance = Instantiate(randomTree, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            treeInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            /*
            new Vector3(
            Random.Range(minTreeScale.x, maxTreeScale.x),
            Random.Range(minTreeScale.y, maxTreeScale.y),
            Random.Range(minTreeScale.z, maxTreeScale.z));
            */

            //treeInstance.tag = treeTag;

            int treeLayer = LayerMask.NameToLayer("Trees");
            treeInstance.layer = treeLayer;

            mapObjectList.Add(treeInstance);

            treeInstance.transform.SetParent(hierarchyRoot.transform);


            //GroundCheck(instantiatedPrefab);
            //WaterCheck();

            GrowTrees(treeInstance);

        }


    }


    void AppleTreePoissonDisc(PoissonDiscSampler appleTreeSampler) {

        foreach (Vector2 sample in appleTreeSampler.Samples())
        {

            GameObject randomAppleTree = GetRandomMapObject(appleTrees);

            GameObject appleTreeInstance = Instantiate(randomAppleTree, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            appleTreeInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            /*
            new Vector3(
            Random.Range(minTreeScale.x, maxTreeScale.x),
            Random.Range(minTreeScale.y, maxTreeScale.y),
            Random.Range(minTreeScale.z, maxTreeScale.z));
            */

            //treeInstance.tag = treeTag;

            int appleTreeLayer = LayerMask.NameToLayer("AppleTree");
            appleTreeInstance.layer = appleTreeLayer;

            mapObjectList.Add(appleTreeInstance);

            appleTreeInstance.transform.SetParent(hierarchyRoot.transform);


            //GroundCheck(instantiatedPrefab);
            //WaterCheck();

            GrowTrees(appleTreeInstance);
        }

    }


    [Header("Tree Growth + Fruit Growth")]
    [Space(10)]

    [SerializeField] private int minTreeGrowDuration = 15;
    [SerializeField] private int maxTreeGrowDuration = 30;
    [SerializeField] private int minTreeGrowDelay = 15;
    [SerializeField] private int maxTreeGrowDelay = 30;

    [SerializeField] private int minAppleGrowDuration = 15;
    [SerializeField] private int maxAppleGrowDuration = 30;
    [SerializeField] private int minAppleGrowDelay = 15;
    [SerializeField] private int maxAppleGrowDelay = 30;

    private int treeGrowDuration;
    private int appleGrowDuration;

    float appleGrowthDelay;
    float treeGrowthDelay;

    [SerializeField] private CorruptionControl corruptionControl;

    private void GrowTrees(GameObject tree)
    {
        Vector3 treeScaleDestination = new(maxTreeScale.x, maxTreeScale.y, maxTreeScale.z);

        ScaleControl treeGrowControl = tree.transform.GetComponent<ScaleControl>();

        corruptionControl = tree.transform.GetComponent<CorruptionControl>();
        corruptionControl.player = player;

        TreeShaders treeshader = tree.GetComponent<TreeShaders>();

        treeGrowDuration = Random.Range(minTreeGrowDuration, maxTreeGrowDuration);
        appleGrowDuration = Random.Range(minAppleGrowDuration, maxAppleGrowDuration);

        appleGrowthDelay = Random.Range(minAppleGrowDelay, maxAppleGrowDelay);
        treeGrowthDelay = Random.Range(minTreeGrowDelay, maxTreeGrowDelay);

        if (!tree.transform.CompareTag("AppleTree"))
        {
            StartCoroutine(treeGrowControl.LerpScale(tree, zeroScale, treeScaleDestination, treeGrowDuration, treeGrowthDelay));
        }

        else if (tree.transform.CompareTag("AppleTree"))
        {
            StartCoroutine(treeGrowControl.LerpScale(tree, zeroScale, treeScaleDestination, treeGrowDuration, treeGrowthDelay));

            foreach (Transform apple in tree.transform)
            {
                apple.GetComponent<Renderer>().enabled = false;

                if (!apple.transform.CompareTag("Apple"))
                {
                    continue;
                }

                ScaleControl appleGrowControl = apple.transform.GetComponent<ScaleControl>();
             

                apple.GetComponent<Rigidbody>().isKinematic = true;

                Vector3 appleScaleDestination = new(maxAppleScale.x, maxAppleScale.y, maxAppleScale.z);
                apple.GetComponent<Renderer>().enabled = true;

                StartCoroutine(appleGrowControl.LerpScale(apple.transform.gameObject, zeroScale, appleScaleDestination, appleGrowDuration, appleGrowthDelay));
                StartCoroutine(WaitUntilGrown(apple.gameObject, appleGrowControl));
            }

        }
       //StartCoroutine(treeShader.GrowLeaves(30f));
    }

    [SerializeField] private float minDecayDuration = 5f;
    [SerializeField] public float maxDecayDuration = 10f;

    [SerializeField] public float minDecayDelayTime = 5f;
    [SerializeField] public float maxDecayDelayTime = 10f;

    [SerializeField] public float minFruitFallBuffer = 1f;
    [SerializeField] public float maxFruitFallBuffer = 60f;

    public IEnumerator WaitUntilGrown(GameObject growObject, ScaleControl scaleControl)
    {
     
        yield return new WaitUntil(() => scaleControl.isFullyGrown);

        float fallBuffer = Random.Range(minFruitFallBuffer, maxFruitFallBuffer);

        yield return new WaitForSeconds(fallBuffer);

        if (growObject.transform.CompareTag("Apple") || growObject.transform.CompareTag("Stick"))
        {
            Rigidbody rigidBody = growObject.transform.GetComponent<Rigidbody>();
            Collider collider = growObject.transform.GetComponent<Collider>();
            HitGround hitGround = growObject.GetComponent<HitGround>();

            rigidBody.useGravity = true; // Object falling to ground.
            rigidBody.isKinematic = false;

            yield return new WaitUntil(() => hitGround.hit);

            float decayDuration = Random.Range(minDecayDuration, maxDecayDuration);
            float decayDelay = Random.Range(minDecayDelayTime, maxDecayDelayTime);

            StartCoroutine(scaleControl.LerpScale(growObject, growObject.transform.localScale, Vector3.zero, decayDuration, decayDelay));

            Destroy(growObject, decayDelay + decayDuration);

            yield break;

        } else if (growObject.transform.CompareTag("AppleTree"))
        {
            yield break;
        }
    }


    void GrassPoissonDisc(PoissonDiscSampler grassSampler)
    {
        foreach (Vector2 sample in grassSampler.Samples())
        {
            GameObject randomGrass = GetRandomMapObject(grass);

            GameObject grassInstance = Instantiate(randomGrass, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            grassInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            grassInstance.transform.localScale = new Vector3(
            Random.Range(minGrassScale.x, maxGrassScale.x),
            Random.Range(minGrassScale.y, maxGrassScale.y),
            Random.Range(minGrassScale.z, maxGrassScale.z));

            grassInstance.tag = grassTag;

            int grassLayer = LayerMask.NameToLayer("Grass");
            grassInstance.layer = grassLayer;

            grassInstance.transform.SetParent(hierarchyRoot.transform);

            grassInstance.AddComponent<NavMeshModifier>();

            mapObjectList.Add(grassInstance);
        }
    }

    void FoliagePoissonDisc(PoissonDiscSampler foliageSamples)
    {
        foreach (Vector2 sample in foliageSamples.Samples())
        {
            GameObject randomFoliage = GetRandomMapObject(foliage);

            GameObject foliageInstance = Instantiate(randomFoliage, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            foliageInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            foliageInstance.transform.localScale = new Vector3(
            Random.Range(minFoliageScale.x, maxFoliageScale.x),
            Random.Range(minFoliageScale.y, maxFoliageScale.y),
            Random.Range(minFoliageScale.z, maxFoliageScale.z));

            foliageInstance.tag = foliageTag;

            int foliageLayer = LayerMask.NameToLayer("Foliage");
            foliageInstance.layer = foliageLayer;

            foliageInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(foliageInstance);
        }
    }

    void RocksPoissonDisc(PoissonDiscSampler rockSamples)
    {
        foreach (Vector2 sample in rockSamples.Samples())
        {
            GameObject randomRocks = GetRandomMapObject(rocks);

            GameObject rockInstance = Instantiate(randomRocks, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            rockInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            rockInstance.transform.localScale = new Vector3(
            Random.Range(minRockScale.x, maxRockScale.x),
            Random.Range(minRockScale.y, maxRockScale.y),
            Random.Range(minRockScale.z, maxRockScale.z));

            rockInstance.tag = rockTag;

            corruptionControl = rockInstance.transform.GetComponent<CorruptionControl>();
            corruptionControl.player = player;

            int rockLayer = LayerMask.NameToLayer("Rocks");
            rockInstance.layer = rockLayer;

            rockInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(rockInstance);
        }
    }

    void MushroomPoissonDisc(PoissonDiscSampler mushroomSampler)
    {
        foreach (Vector2 sample in mushroomSampler.Samples())
        {
            GameObject randomMushroom = GetRandomMapObject(mushrooms);

            GameObject mushroomInstance = Instantiate(randomMushroom, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            mushroomInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            mushroomInstance.transform.localScale = new Vector3(
            Random.Range(minMushroomScale.x, maxMushroomScale.x),
            Random.Range(minMushroomScale.y, maxMushroomScale.y),
            Random.Range(minMushroomScale.z, maxMushroomScale.z));


            mushroomInstance.tag = mushroomTag;

            mushroomInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(mushroomInstance);
        }
    }

    void FliesPoissonDisc(PoissonDiscSampler fliesSampler)
    {
        foreach (Vector2 sample in fliesSampler.Samples())
        {
            GameObject randomFlies = GetRandomMapObject(flies);

            GameObject fliesInstance = Instantiate(randomFlies, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            fliesInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            fliesInstance.transform.localScale = new Vector3(
            Random.Range(minFliesScale.x, maxFliesScale.x),
            Random.Range(minFliesScale.y, maxFliesScale.y),
            Random.Range(minFliesScale.z, maxFliesScale.z));


            fliesInstance.tag = fliesTag;

            int fliesLayer = LayerMask.NameToLayer("Flies");
            fliesInstance.layer = fliesLayer;

            fliesInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(fliesInstance);
        }
    }

    private float emitterY;
    [SerializeField] private float emitterYOffset = 10f;
    [SerializeField] private LayerMask emitterLayerMask;

    public Transform emitterHierarchyRoot;

    public IEnumerator GenerateEmitterCheckers(List<GameObject> emitters)
    {
        foreach (GameObject emitter in emitters)
        {

            emitterY = emitter.transform.position.y;
            emitterY += emitterYOffset;

            if (Physics.Raycast(emitter.transform.position, Vector3.down, out RaycastHit downHit, Mathf.Infinity, emitterLayerMask))
            {
                Debug.DrawRay(emitter.transform.position, Vector3.down, Color.red);

                if (downHit.collider.CompareTag("Ground") || downHit.collider.CompareTag("CaveGround"))
                {
                    DestroyObject(emitter);
                } else if (downHit.collider.CompareTag("Water"))
                {
                    continue;
                } else
                {
                    DestroyObject(emitter);
                }
            }

            static void DestroyObject(GameObject emitter)
            {
                if (Application.isEditor)
                {
                    Debug.Log("Object destroyed in Editor.");
                    DestroyImmediate(emitter);
                }
                else
                {
                    Debug.Log("Object destroyed in game.");
                    Destroy(emitter);
                }
            }
            yield return null;
        }

    }

    void FireWoodPoissonDisc(PoissonDiscSampler fireWoodSampler)
    {
        foreach (Vector2 sample in fireWoodSampler.Samples())
        {
            GameObject randomFireWood = GetRandomMapObject(fireWood);

            GameObject fireWoodInstance = Instantiate(randomFireWood, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            fireWoodInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            fireWoodInstance.transform.localScale = new Vector3(
            Random.Range(minFireWoodScale.x, maxFireWoodScale.x),
            Random.Range(minFireWoodScale.y, maxFireWoodScale.y),
            Random.Range(minFireWoodScale.z, maxFireWoodScale.z));


            fireWoodInstance.tag = fireWoodTag;

            int fireWoodLayer = LayerMask.NameToLayer("FireWood");
            fireWoodInstance.layer = fireWoodLayer;

            fireWoodInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(fireWoodInstance);
        }
    }

    void GroundCheck()
    {

        foreach (GameObject mapObject in mapObjectList)
        {
            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit downHit, Mathf.Infinity))
            {
                //Debug.DrawRay(mapObject.transform.position, Vector3.down, Color.red);

                if (downHit.collider.CompareTag(waterTag))
                {
                    Debug.Log("Water Ahoy!");
                    DestroyObject();
                }

                if (downHit.collider == null)
                {
                    DestroyObject();
                }
            }


            void DestroyObject()
            {
                if (Application.isEditor)
                {
                    Debug.Log("Object destroyed in Editor.");
                    DestroyImmediate(mapObject);
                }
                else
                {
                    Debug.Log("Object destroyed in game.");
                    Destroy(mapObject);
                }
            }
        }

        ListCleanup();


        void ListCleanup()
        {
            for (var i = mapObjectList.Count - 1; i > -1; i--)
            {
                if (mapObjectList[i] == null)
                    mapObjectList.RemoveAt(i);
            }
        }

        AnchorToGround();
    }

        
    void AnchorToGround()
    {

        foreach (GameObject mapObject in mapObjectList)
        {

            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, groundLayerMask))
            {
                float distance = hitFloor.distance;

                float x = mapObject.transform.position.x;
                float y = mapObject.transform.position.y - distance;
                float z = mapObject.transform.position.z;

                Vector3 newPosition = new Vector3(x, y, z);

                mapObject.transform.position = newPosition;

                //Debug.Log("Clamped to Ground!");
                //Debug.Log("Distance: " + distance);
            }
        }

    }

    void DestroyDeadZones()
    {
        foreach (GameObject mapObject in mapObjectList)
        {
            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, deadZoneLayerMask))
            {
                if (hitFloor.collider.CompareTag("DeadZone"))
                {
                    //Debug.Log("DeadZone hit.");
                    DestroyObject();
                }
            }


            void DestroyObject()
            {
                if (Application.isEditor)
                {
                    Debug.Log("Object destroyed in Editor.");
                    DestroyImmediate(mapObject);
                }
                else
                {
                    Debug.Log("Object destroyed in game.");
                    Destroy(mapObject);
                }
            }
        }
    }

    void SetOffset()
    {
        mapObject.transform.position = new Vector3(xOffset, initY, zOffset);
    }

    void ResetPosOffset()
    {
        mapObject.transform.position = new Vector3(0, initY, 0);
    }

    public void Clear()
    {
        mapObjectList.Clear();

        if (Application.isEditor)
        {

            ResetPosOffset();

            while (hierarchyRoot.transform.childCount != 0)
            {
                foreach (Transform child in hierarchyRoot.transform)
                {
                    DestroyImmediate(child.gameObject);
                    if (hierarchyRoot.transform.childCount != 0)
                    {
                        continue;
                    }

                }
            }

            while (EmitterHierarchyParent.transform.childCount != 0)
            {
                foreach (Transform child in EmitterHierarchyParent.transform)
                {
                    DestroyImmediate(child.gameObject);
                    if (EmitterHierarchyParent.transform.childCount != 0)
                    {
                        continue;
                    }

                }
            }

        }

    }
}

    // Update is called once per frame



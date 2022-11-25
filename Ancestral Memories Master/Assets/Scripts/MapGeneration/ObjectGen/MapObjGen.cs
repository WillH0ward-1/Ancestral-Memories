using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MapObjGen : MonoBehaviour
{
    //public int density;

    [Header("Map Object Generator")]
    [Header("========================================================================================================================")]

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

    public GameObject player;

    public RadialMenu radialMenu;

    public Camera cam;


    //public Interactable treeInteraction;

    private void Awake()
    {
        Clear();
        Generate();
    }

    public GameObject GetRandomMapObject(GameObject[] mapElements)
    {
        return mapElements[Random.Range(0, mapElements.Length)];
    }

    public void Generate()
    {
        //sampleWidth = meshSettings.meshWorldSize;
        //sampleHeight = meshSettings.meshWorldSize;

        ResetPosOffset();

        sampleWidth = meshSettings.meshWorldSize;
        sampleHeight = meshSettings.meshWorldSize;

        xOffset = -sampleWidth / 2;
        zOffset = -sampleHeight / 2;

        //mapObjectList.Clear();

        PoissonDiscSampler treeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumTreeRadius);
        PoissonDiscSampler appleTreeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumAppleTreeRadius);
        PoissonDiscSampler grassSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumGrassRadius);
        PoissonDiscSampler foliageSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFoliageRadius);
        PoissonDiscSampler rockSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumRockRadius);
        PoissonDiscSampler fliesSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFliesRadius);
        PoissonDiscSampler animalSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumAnimalRadius);
        PoissonDiscSampler mushroomSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumMushroomRadius);

        TreePoissonDisc(treeSampler);
        AppleTreePoissonDisc(appleTreeSampler);
        GrassPoissonDisc(grassSampler);
        FoliagePoissonDisc(foliageSampler);
        RocksPoissonDisc(rockSampler);
        FliesPoissonDisc(fliesSampler);
        AnimalPoissonDisc(animalSampler);
        MushroomPoissonDisc(mushroomSampler);

        SetOffset();

        GroundCheck();
        //SortTreeTypes(treeList);
    }

    void AnimalPoissonDisc(PoissonDiscSampler animalSampler)

    {
        foreach (Vector2 sample in animalSampler.Samples())
        {
            GameObject randomAnimal = GetRandomMapObject(animals);

            GameObject animalInstance = Instantiate(randomAnimal, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            animalInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            animalInstance.tag = animalTag;

            int animalsLayer = LayerMask.NameToLayer("Animals");
            animalInstance.layer = animalsLayer;

            animalInstance.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(animalInstance);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

    [Header("Generated Objects")]
    [Space(10)]

    public List<GameObject> mapObjectList;
    public List<GameObject> treeList;

    private Vector3 zeroScale = new Vector3(0, 0, 0);

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

    float randomFallDuration;

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            KillTrees();
        }
    }

    void KillTrees()
    {
        foreach(GameObject tree in treeList)
        {
            randomFallDuration  = Random.Range(2, 4);
            Fall(tree.transform.gameObject, randomFallDuration);
        }
    }

    public void Fall(GameObject treeObject, float duration)
    {
        Vector2 pointOnCircle = Random.insideUnitCircle * treeObject.transform.localScale.y;

        Vector3 fallPoint = treeObject.transform.position +
            pointOnCircle.x * treeObject.transform.right +
            pointOnCircle.y * treeObject.transform.forward;

        Vector3 updatedUpVector = Vector3.Normalize(fallPoint - treeObject.transform.position);

        StartCoroutine(UpdateUpVector(treeObject, updatedUpVector, duration, 0.001f));
    }

    public IEnumerator UpdateUpVector(GameObject target, Vector3 upVector, float duration, float threshold = 0.001f)
    {
        while (Vector3.Distance(upVector, target.transform.up) > threshold)
        {
            target.transform.up = Vector3.Lerp(target.transform.up, upVector, duration * Time.deltaTime);
            yield return new WaitForEndOfFrame();
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

    private void GrowTrees(GameObject tree)
    {
        Vector3 treeScaleDestination = new(maxTreeScale.x, maxTreeScale.y, maxTreeScale.z);

        GrowControl treeGrowControl = tree.transform.GetComponent<GrowControl>();
        TreeShaders treeshader = tree.GetComponent<TreeShaders>();

        treeGrowDuration = Random.Range(minTreeGrowDuration, maxTreeGrowDuration);
        appleGrowDuration = Random.Range(minAppleGrowDuration, maxAppleGrowDuration);

        appleGrowthDelay = Random.Range(minAppleGrowDelay, maxAppleGrowDelay);
        treeGrowthDelay = Random.Range(minTreeGrowDelay, maxTreeGrowDelay);



        if (!tree.transform.CompareTag("AppleTree"))
        {
            StartCoroutine(treeGrowControl.Grow(tree, zeroScale, treeScaleDestination, treeGrowDuration, treeGrowthDelay));
        }
        else if (tree.transform.CompareTag("AppleTree"))
        {
            StartCoroutine(treeGrowControl.Grow(tree, zeroScale, treeScaleDestination, treeGrowDuration, treeGrowthDelay));

            foreach (Transform apple in tree.transform)
            {
                apple.GetComponent<Renderer>().enabled = false;

                if (!apple.transform.CompareTag("Apple"))
                {
                    continue;
                }
         
                GrowControl appleGrowControl = apple.transform.GetComponent<GrowControl>();

                apple.GetComponent<Rigidbody>().isKinematic = true;

                Vector3 appleScaleDestination = new(maxAppleScale.x, maxAppleScale.y, maxAppleScale.z);
                apple.GetComponent<Renderer>().enabled = true;

                StartCoroutine(appleGrowControl.Grow(apple.transform.gameObject, zeroScale, appleScaleDestination, appleGrowDuration, appleGrowthDelay));
                StartCoroutine(WaitUntilGrown(apple.gameObject, appleGrowControl));
            }

        }
        // StartCoroutine(treeShader.GrowLeaves(30f));
    }

    [SerializeField] private float minDecayDuration = 5f;
    [SerializeField] public float maxDecayDuration = 10f;

    [SerializeField] public float minDecayDelayTime = 5f;
    [SerializeField] public float maxDecayDelayTime = 10f;

    private IEnumerator WaitUntilGrown(GameObject growObject, GrowControl scaleControl)
    {
     
        yield return new WaitUntil(() => scaleControl.isFullyGrown);

        if (growObject.transform.CompareTag("Apple"))
        {
            Rigidbody rigidBody = growObject.transform.GetComponent<Rigidbody>();
            Collider collider = growObject.transform.GetComponent<Collider>();
            HitGround hitGround = growObject.GetComponent<HitGround>();

            rigidBody.isKinematic = false;
            rigidBody.useGravity = true; // Object falling to ground.

            //collider.isTrigger = true;
            yield return new WaitUntil(() => hitGround.hit);

            float decayDuration = Random.Range(minDecayDuration, maxDecayDuration);
            float decayDelay = Random.Range(minDecayDelayTime, maxDecayDelayTime);

            StartCoroutine(scaleControl.Grow(growObject, growObject.transform.localScale, Vector3.zero, decayDuration, decayDelay));

            Destroy(growObject, decayDelay + decayDuration);

            yield break;

        } else if (growObject.transform.CompareTag("AppleTree"))
        {
            //GrowApples(growObject, growControl);

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

    private string spawnTag = "Spawn";


    void GroundCheck()
    {
        foreach (GameObject mapObject in mapObjectList)
        {
            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit downHit, Mathf.Infinity))
            {
                Debug.DrawRay(mapObject.transform.position, Vector3.down, Color.red);

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



    void AddColliders()
    {
        foreach (GameObject mapObject in mapObjectList)
        {

            if (!mapObject.CompareTag(grassTag) && !mapObject.CompareTag(fliesTag) && !mapObject.CompareTag(animalTag) && !mapObject.CompareTag(foliageTag) && !mapObject.CompareTag(mushroomTag))
            {
                mapObject.AddComponent<MeshCollider>();

                navMeshObstacle = GetComponent<NavMeshObstacle>();
                navMeshObstacle = mapObject.AddComponent<NavMeshObstacle>();

                navMeshObstacle.enabled = true;
                navMeshObstacle.center = new Vector3(0, 0, 0);
                ;
                navMeshObstacle.shape = NavMeshObstacleShape.Capsule;

                if (mapObject.CompareTag(rockTag))
                {
                    navMeshObstacle.radius = 0.5f;
                }

                if (mapObject.CompareTag(treeTag))
                {
                    navMeshObstacle.radius = 0.2f;
                }

                navMeshObstacle.carving = true;
                //navMeshObstacle.carveOnlyStationary = true;

                //navMeshObstacle.size = new Vector3(obstacleSizeX, obstacleSizeY, obstacleSizeZ);
            }
            continue;
        }

        Debug.Log("Colliders Generated!");

        //AddInteractivity();
    }
        
    void AnchorToGround()
    {
        //LayerMask groundMask = LayerMask.GetMask("Ground");

        foreach (GameObject mapObject in mapObjectList)
        {

            int groundLayerIndex = LayerMask.NameToLayer("Ground");
            int groundLayerMask = (1 << groundLayerIndex);

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

        foreach (GameObject mapObject in mapObjectList)
        {

            int groundLayerIndex = LayerMask.NameToLayer("Ground");
            int groundLayerMask = (1 << groundLayerIndex);

            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, groundLayerMask))
            {
                float distance = hitFloor.distance;

                float x = mapObject.transform.position.x;
                float y = mapObject.transform.position.y - distance;
                float z = mapObject.transform.position.z;

                Vector3 newPosition = new Vector3(x, y, z);

                mapObject.transform.position = newPosition;

            }
        }

        DestroyDeadZones();
    }



    void DestroyDeadZones()
    {
        foreach (GameObject mapObject in mapObjectList)
        {
            int deadZoneLayerIndex = LayerMask.NameToLayer("DeadZone");
            int deadZoneLayerMask = (23 << deadZoneLayerIndex);

            int caveLayerIndex = LayerMask.NameToLayer("Cave");
            int caveLayerMask = (24 << caveLayerIndex);

            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity, deadZoneLayerMask))
            {
                if (hitFloor.collider.CompareTag("DeadZone"))
                {
                    //Debug.Log("DeadZone hit.");
                    DestroyObject();
                }
            }

            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitCave, Mathf.Infinity, caveLayerMask))
            {
                if (hitFloor.collider.CompareTag("Cave"))
                {
                    //Debug.Log("Cannot generate objects in cave!");

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

        }
        else

        {
            ResetPosOffset();

            while (hierarchyRoot.transform.childCount != 0)
            {
                foreach (Transform child in hierarchyRoot.transform)
                {
                    Destroy(child.gameObject);
                    if (hierarchyRoot.transform.childCount != 0)
                    {
                        continue;

                    }
                }
            }
        }
    }
}

    // Update is called once per frame



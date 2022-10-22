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
    private readonly string waterTag = "Water";
    private readonly string groundTag = "Ground";
    private readonly string grassTag = "Grass";
    private readonly string foliageTag = "Foliage";
    private readonly string rockTag = "Rocks";
    private readonly string fliesTag = "Flies";
    private readonly string fishTag = "Fish";
    private readonly string animalTag = "Animal";

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

    [Header("Generated Objects")]
    [Space(10)]

    public List<GameObject> mapObjectList;


    //public MeshSettings meshSettings;

    // Start is called before the first frame update

    void OnSceneGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            SceneView.RepaintAll();
        }
    }

    private void Awake()
    {
        Clear();
        Generate();
    }

    public GameObject GetRandomObject(GameObject[] mapElements)
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
        PoissonDiscSampler grassSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumGrassRadius);
        PoissonDiscSampler foliageSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFoliageRadius);
        PoissonDiscSampler rockSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumRockRadius);
        PoissonDiscSampler fliesSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFliesRadius);
        PoissonDiscSampler  animalSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumAnimalRadius);

        TreePoissonDisc(treeSampler);
        GrassPoissonDisc(grassSampler);
        FoliagePoissonDisc(foliageSampler);
        RocksPoissonDisc(rockSampler);
        FliesPoissonDisc(fliesSampler);
        AnimalPoissonDisc(animalSampler);

        SetOffset();

        GroundCheck();
    }

    void AnimalPoissonDisc(PoissonDiscSampler animalSampler)

    {
        foreach (Vector2 sample in animalSampler.Samples())
        {
            GameObject randomAnimal = GetRandomObject(animals);

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


        void TreePoissonDisc(PoissonDiscSampler treeSampler)
    {
        foreach (Vector2 sample in treeSampler.Samples())
        {
            GameObject randomTree = GetRandomObject(trees);

            GameObject instantiatedTree = Instantiate(randomTree, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            instantiatedTree.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedTree.transform.localScale = new Vector3(
            Random.Range(minTreeScale.x, maxTreeScale.x),
            Random.Range(minTreeScale.y, maxTreeScale.y),
            Random.Range(minTreeScale.z, maxTreeScale.z));


            instantiatedTree.tag = treeTag;

            int treeLayer = LayerMask.NameToLayer("Trees");
            instantiatedTree.layer = treeLayer;

            instantiatedTree.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(instantiatedTree);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }

    }

    void GrassPoissonDisc(PoissonDiscSampler grassSampler)
    {
        foreach (Vector2 sample in grassSampler.Samples())
        {
            GameObject randomGrass = GetRandomObject(grass);

            GameObject instantiatedGrass = Instantiate(randomGrass, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            instantiatedGrass.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedGrass.transform.localScale = new Vector3(
            Random.Range(minGrassScale.x, maxGrassScale.x),
            Random.Range(minGrassScale.y, maxGrassScale.y),
            Random.Range(minGrassScale.z, maxGrassScale.z));

            instantiatedGrass.tag = grassTag;

            int grassLayer = LayerMask.NameToLayer("Grass");
            instantiatedGrass.layer = grassLayer;

            instantiatedGrass.transform.SetParent(hierarchyRoot.transform);

            instantiatedGrass.AddComponent<NavMeshModifier>();

            mapObjectList.Add(instantiatedGrass);

//            navModifier.ignoreFromBuild = true;

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }

    }

    void FoliagePoissonDisc(PoissonDiscSampler foliageSamples)
    {
        foreach (Vector2 sample in foliageSamples.Samples())
        {
            GameObject randomFoliage = GetRandomObject(foliage);

            GameObject instantiatedFoliage = Instantiate(randomFoliage, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            instantiatedFoliage.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedFoliage.transform.localScale = new Vector3(
            Random.Range(minFoliageScale.x, maxFoliageScale.x),
            Random.Range(minFoliageScale.y, maxFoliageScale.y),
            Random.Range(minFoliageScale.z, maxFoliageScale.z));

            instantiatedFoliage.tag = foliageTag;

            int foliageLayer = LayerMask.NameToLayer("Foliage");
            instantiatedFoliage.layer = foliageLayer;

            instantiatedFoliage.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(instantiatedFoliage);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }

    }

    void RocksPoissonDisc(PoissonDiscSampler rockSamples)
    {
        foreach (Vector2 sample in rockSamples.Samples())
        {
            GameObject randomRocks = GetRandomObject(rocks);

            GameObject instantiatedRock = Instantiate(randomRocks, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            instantiatedRock.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedRock.transform.localScale = new Vector3(
            Random.Range(minRockScale.x, maxRockScale.x),
            Random.Range(minRockScale.y, maxRockScale.y),
            Random.Range(minRockScale.z, maxRockScale.z));


            instantiatedRock.tag = rockTag;

            int rockLayer = LayerMask.NameToLayer("Rocks");
            instantiatedRock.layer = rockLayer;

            instantiatedRock.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(instantiatedRock);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

    void FliesPoissonDisc(PoissonDiscSampler fliesSampler)
    {
        foreach (Vector2 sample in fliesSampler.Samples())
        {
            GameObject randomFlies = GetRandomObject(flies);

            GameObject instantiatedFlies = Instantiate(randomFlies, new Vector3(sample.x, initY, sample.y), Quaternion.identity);

            instantiatedFlies.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedFlies.transform.localScale = new Vector3(
            Random.Range(minFliesScale.x, maxFliesScale.x),
            Random.Range(minFliesScale.y, maxFliesScale.y),
            Random.Range(minFliesScale.z, maxFliesScale.z));


            instantiatedFlies.tag = fliesTag;

            int fliesLayer = LayerMask.NameToLayer("Flies");
            instantiatedFlies.layer = fliesLayer;

            instantiatedFlies.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(instantiatedFlies);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }

    }

    void GroundCheck()
    {

        foreach (GameObject mapObject in mapObjectList)
        {
            /*
            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitGround, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                Debug.DrawRay(mapObject.transform.position, Vector3.down, Color.red);

                var LayerGround = LayerMask.NameToLayer(groundTag);

                if (hitGround.transform.gameObject.layer == LayerGround)
                {
                    ;
                }
            }
            */
            var LayerWater = LayerMask.NameToLayer(waterTag);
            var LayerGround = LayerMask.NameToLayer(groundTag);

            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit down, Mathf.Infinity)) // TRY A CAPSULE CAST!! This will prevent things spawning too close to water.
            {

                Debug.DrawRay(mapObject.transform.position, Vector3.down, Color.red);

                if (down.transform.gameObject.layer == LayerWater)
                {
                    Debug.Log("Water Ahoy!");
                    DestroyObject();
                }

                if (down.transform.gameObject.layer == LayerGround)
                {
                    Debug.Log("Fish can't walk.");
                    DestroyObject();
                }

                if (down.collider == null)
                {
                    DestroyObject();
                }
                else
                {
                    continue;
                }
            }

            if (Physics.Raycast(mapObject.transform.position, Vector3.up, out RaycastHit up, Mathf.Infinity)) // TRY A CAPSULE CAST!! This will prevent things spawning too close to water.
            {
                if (up.transform.gameObject.layer == LayerWater)
                {
                    DestroyObject();
                }

                if (up.transform.gameObject.layer == LayerWater && up.transform.gameObject.layer == LayerGround)
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

        //Mesh mesh = GetComponent<MeshFilter>().mesh;
        //Bounds bounds = mesh.bounds;

    
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

                    //Debug.Log("Clamped to Ground!");
                    //Debug.Log("Distance: " + distance);
                }
            }

            AddColliders();
        }

        void AddColliders()
        {
            foreach (GameObject mapObject in mapObjectList)
            {
                if (!mapObject.CompareTag(grassTag) && !mapObject.CompareTag(fliesTag) && !mapObject.CompareTag(animalTag))
                {
                    mapObject.AddComponent<MeshCollider>();

                    navMeshObstacle = GetComponent<NavMeshObstacle>();
                    navMeshObstacle = mapObject.AddComponent<NavMeshObstacle>();

                    navMeshObstacle.enabled = true;
                    //navMeshObstacle.carveOnlyStationary = true;
                    navMeshObstacle.carving = true;

                    //navMeshObstacle.size = new Vector3(obstacleSizeX, obstacleSizeY, obstacleSizeZ);
                }
                continue;
            }
            Debug.Log("Colliders Generated!");
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

    void ClearList()
    {
        ResetPosOffset();
    }

    public void Clear()
    {
        mapObjectList.Clear();

        if (Application.isEditor)
        {
            
            ClearList();

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
            ClearList();

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



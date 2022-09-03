using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class MapObjGen : MonoBehaviour
{
    //public int density;

    public GameObject mapObject;
    public MeshData meshData;

    //private MeshFilter meshFilter;
    private Mesh mesh;

    [SerializeField] Vector3 minTreeScale;
    [SerializeField] Vector3 maxTreeScale;

    [SerializeField] Vector3 minGrassScale;
    [SerializeField] Vector3 maxGrassScale;

    [SerializeField] Vector3 minFoliageScale;
    [SerializeField] Vector3 maxFoliageScale;

    [SerializeField] Vector3 minRockScale;
    [SerializeField] Vector3 maxRockScale;

    [SerializeField, Range(0, 1)] float rotateTowardsNormal;

    [SerializeField] Vector2 rotationRange;

    public List<GameObject> mapObjectList;

    public GameObject[] trees;
    public GameObject[] grass;
    public GameObject[] foliage;
    public GameObject[] rocks;

    [SerializeField] float sampleWidth = 1000;

    [SerializeField] float sampleHeight = 1000;

    [SerializeField] float minimumTreeRadius = 70;
    [SerializeField] float minimumGrassRadius = 70;
    [SerializeField] float minimumFoliageRadius = 70;
    [SerializeField] float minimumRockRadius = 70;

    [SerializeField] GameObject hierarchyRoot;

    private readonly string treeTag = "Trees";
    private readonly string waterTag = "Water";
    private readonly string groundTag = "Ground";
    private readonly string grassTag = "Grass";
    private readonly string foliageTag = "Foliage";
    private readonly string rockTag = "Rocks";

    [SerializeField] private float xPosition = -500;
    [SerializeField] private float yPosition = 10; // This determines how high the trees will raycast from, and thus where they will spawn relative to height.
    [SerializeField] private float zPosition = -500;

    [SerializeField] float obstacleSizeX = 1;
    [SerializeField] float obstacleSizeY = 14;
    [SerializeField] float obstacleSizeZ = 1;

    [SerializeField] float yOffset;

    [SerializeField] private NavMeshObstacle navMeshObstacle;


    //public MeshSettings meshSettings;

    // Start is called before the first frame update

    private void Awake()
    {
        Clear();
        Generate();
    }

    public GameObject GetRandomObject(GameObject[] mapElements)
    {
        return mapElements[Random.Range(0, mapElements.Length - 1)];
    }

    public void Generate()
    {
        ResetOffset();

        //mapObjectList.Clear();

        PoissonDiscSampler treeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumTreeRadius);
        PoissonDiscSampler grassSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumGrassRadius);
        PoissonDiscSampler foliageSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumFoliageRadius);
        PoissonDiscSampler rockSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumRockRadius);

        TreePoissonDisc(treeSampler);
        GrassPoissonDisc(grassSampler);
        FoliagePoissonDisc(foliageSampler);
        RocksPoissonDisc(rockSampler);

        GroundCheck();
    }

    void TreePoissonDisc(PoissonDiscSampler treeSampler)
    {
        foreach (Vector2 sample in treeSampler.Samples())
        {
            GameObject randomTree = GetRandomObject(trees);

            GameObject instantiatedTree = Instantiate(randomTree, new Vector3(sample.x, 0, sample.y), Quaternion.identity);

            instantiatedTree.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedTree.transform.localScale = new Vector3(
            Random.Range(minTreeScale.x, maxTreeScale.x),
            Random.Range(minTreeScale.y, maxTreeScale.y),
            Random.Range(minTreeScale.z, maxTreeScale.z));


            instantiatedTree.tag = treeTag;

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

            GameObject instantiatedGrass = Instantiate(randomGrass, new Vector3(sample.x, 0, sample.y), Quaternion.identity);

            instantiatedGrass.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedGrass.transform.localScale = new Vector3(
            Random.Range(minGrassScale.x, maxGrassScale.x),
            Random.Range(minGrassScale.y, maxGrassScale.y),
            Random.Range(minGrassScale.z, maxGrassScale.z));


            instantiatedGrass.tag = grassTag;

            instantiatedGrass.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(instantiatedGrass);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }

    }

    void FoliagePoissonDisc(PoissonDiscSampler foliageSamples)
    {
        foreach (Vector2 sample in foliageSamples.Samples())
        {
            GameObject randomFoliage = GetRandomObject(foliage);

            GameObject instantiatedFoliage = Instantiate(randomFoliage, new Vector3(sample.x, 0, sample.y), Quaternion.identity);

            instantiatedFoliage.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedFoliage.transform.localScale = new Vector3(
            Random.Range(minFoliageScale.x, maxFoliageScale.x),
            Random.Range(minFoliageScale.y, maxFoliageScale.y),
            Random.Range(minFoliageScale.z, maxFoliageScale.z));


            instantiatedFoliage.tag = foliageTag;

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

            GameObject instantiatedRock = Instantiate(randomRocks, new Vector3(sample.x, 0, sample.y), Quaternion.identity);

            instantiatedRock.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedRock.transform.localScale = new Vector3(
            Random.Range(minRockScale.x, maxRockScale.x),
            Random.Range(minRockScale.y, maxRockScale.y),
            Random.Range(minRockScale.z, maxRockScale.z));


            instantiatedRock.tag = rockTag;

            instantiatedRock.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(instantiatedRock);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }
    }

    void GroundCheck()
    {
        SetOffset();

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

            Debug.DrawRay(mapObject.transform.position, Vector3.down, Color.red);

            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitWater, Mathf.Infinity))
            {

                var LayerWater = LayerMask.NameToLayer(waterTag);

                if (hitWater.transform.gameObject.layer == LayerWater)
                {
                    Debug.Log("Water Ahoy!");
                    DestroyObject();
                }
                else
                {
                    continue;
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

        void AnchorToGround()
        {
            foreach (GameObject mapObject in mapObjectList)
            {
                if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitFloor, Mathf.Infinity))
                {

                    float distance = hitFloor.distance;

                    float x = mapObject.transform.position.x;
                    float y = mapObject.transform.position.y - distance + yOffset;
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
                mapObject.AddComponent<MeshCollider>();

                navMeshObstacle = GetComponent<NavMeshObstacle>();
                navMeshObstacle = mapObject.AddComponent<NavMeshObstacle>();

                navMeshObstacle.enabled = true;
                //navMeshObstacle.carveOnlyStationary = true;
                navMeshObstacle.carving = false;

                navMeshObstacle.size = new Vector3(obstacleSizeX, obstacleSizeY, obstacleSizeZ);

            }

            Debug.Log("Colliders Generated!");
        }
    }

    void SetOffset()
    {
        mapObject.transform.position = new Vector3(xPosition, yPosition, zPosition);
    }

    void ResetOffset()
    {
        mapObject.transform.position = new Vector3(0, 0, 0);
    }

    [SerializeField] private float newY;

    void ClearList()
    {
        for (var i = 0; i < mapObjectList.Count; i++)
        {
            mapObjectList.RemoveAt(i);
        }

        ResetOffset();
    }

    public void Clear()
    {
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



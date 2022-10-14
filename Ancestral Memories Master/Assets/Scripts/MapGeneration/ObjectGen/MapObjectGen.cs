using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MapObjectGen : MonoBehaviour
{
    //public int density;

    public GameObject mapObject;

    //public MeshData meshData;

    //public MeshSettings meshSettings;

    //private MeshFilter meshFilter;
    private Mesh mesh;

    [SerializeField] Vector3 minTreeScale;
    [SerializeField] Vector3 maxTreeScale;

    [SerializeField] Vector3 maxFishScale;

    [SerializeField, Range(0, 1)] float rotateTowardsNormal;

    [SerializeField] Vector2 rotationRange;

    public List<GameObject> mapObjectList;

    public GameObject[] trees;

     float sampleWidth = 0;

     float sampleHeight = 0;

    [SerializeField] float minimumTreeRadius = 70;

    [SerializeField] GameObject hierarchyRoot;

    private readonly string treeTag = "Trees";
    private readonly string waterTag = "Water";
    private readonly string groundTag = "Ground";


    [SerializeField] private float xOffset = 0;
    [SerializeField] private float yPos = 10; // This determines how high the trees will raycast from, and thus where they will spawn relative to height.
    [SerializeField] private float zOffset = 0;

    [SerializeField] float obstacleSizeX = 1;
    [SerializeField] float obstacleSizeY = 14;
    [SerializeField] float obstacleSizeZ = 1;

    [SerializeField] float yOffset;

    [SerializeField] private NavMeshObstacle navMeshObstacle;

    [SerializeField] private float mapSizeX = 0;
    [SerializeField] private float mapSizeY = 0;
    [SerializeField] private float mapSizeZ = 0;

    //[SerializeField] private NavMeshModifier navModifier;



    //public MeshSettings meshSettings;

    // Start is called before the first frame update

    protected void SetMapSize()
    {
        Vector3 mapScale = new Vector3(mapSizeX, mapSizeY, mapSizeZ);
        transform.localScale = mapScale;
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
        SetMapSize();

        //sampleWidth = meshSettings.meshWorldSize;
        //sampleHeight = meshSettings.meshWorldSize;

        ResetPosOffset();

        sampleWidth = mapSizeZ * 10;
        sampleHeight = mapSizeX * 10;

        xOffset = -sampleWidth / 2;
        zOffset = -sampleHeight / 2;

        PoissonDiscSampler treeSampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumTreeRadius);
        TreePoissonDisc(treeSampler);

        SetOffset();

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

            int treeLayer = LayerMask.NameToLayer("Trees");
            instantiatedTree.layer = treeLayer;

            instantiatedTree.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(instantiatedTree);

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

            if (Physics.Raycast(mapObject.transform.position, Vector3.down, out RaycastHit hitWater, Mathf.Infinity)) // TRY A CAPSULE CAST!! This will prevent things spawning too close to water.
            {

                var LayerWater = LayerMask.NameToLayer(waterTag);
                var LayerGround = LayerMask.NameToLayer(groundTag);

                if (hitWater.transform.gameObject.layer == LayerWater && !mapObject.CompareTag("Fish"))
                {
                    Debug.Log("Water Ahoy!");
                    DestroyObject();
                }
                else if (hitWater.transform.gameObject.layer == LayerGround && mapObject.CompareTag("Fish"))
                {
                    Debug.Log("Fish can't walk.");
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
                if (!mapObject.CompareTag("Grass") && !mapObject.CompareTag("Flies"))
                {
                    mapObject.AddComponent<MeshCollider>();

                    //navMeshObstacle = GetComponent<NavMeshObstacle>();
                    //navMeshObstacle = mapObject.AddComponent<NavMeshObstacle>();

                    //navMeshObstacle.enabled = true;
                    //navMeshObstacle.carveOnlyStationary = true;
                    //navMeshObstacle.carving = true;

                    //navMeshObstacle.size = new Vector3(obstacleSizeX, obstacleSizeY, obstacleSizeZ);
                }
                continue;
            }
            Debug.Log("Colliders Generated!");
        }
    }

    void SetOffset()
    {
        mapObject.transform.position = new Vector3(xOffset, yOffset, zOffset);
    }

    void ResetPosOffset()
    {
        mapObject.transform.position = new Vector3(0, 0, 0);
    }

    [SerializeField] private float newY;

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



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

    [SerializeField] Vector3 minScale;
    [SerializeField] Vector3 maxScale;

    [SerializeField, Range(0, 1)] float rotateTowardsNormal;

    [SerializeField] Vector2 rotationRange;

    public List<GameObject> mapObjectList;

    public GameObject[] mapElements;

    [SerializeField] float sampleWidth = 1000;

    [SerializeField] float sampleHeight = 1000;

    [SerializeField] float minimumRadius = 70;

    [SerializeField] GameObject hierarchyRoot;

    private readonly string treeTag = "Trees";
    private readonly string waterTag = "Water";
    private readonly string groundTag = "Ground";

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

    public GameObject GetRandomTree(GameObject[] mapElements)
    {
        return mapElements[Random.Range(0, mapElements.Length - 1)];
    }

    public void Generate()
    {
        ResetOffset();

        //mapObjectList.Clear();

        PoissonDiscSampler sampler = new PoissonDiscSampler(sampleWidth, sampleHeight, minimumRadius);

        PoissonDiscSampling(sampler);

    }

    void PoissonDiscSampling(PoissonDiscSampler sampler)
    {
        foreach (Vector2 sample in sampler.Samples())
        {
            GameObject randomTree = GetRandomTree(mapElements);

            GameObject instantiatedPrefab = Instantiate(randomTree, new Vector3(sample.x, 0, sample.y), Quaternion.identity);

            instantiatedPrefab.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            instantiatedPrefab.transform.localScale = new Vector3(
            Random.Range(minScale.x, maxScale.x),
            Random.Range(minScale.y, maxScale.y),
            Random.Range(minScale.z, maxScale.z));

            instantiatedPrefab.tag = treeTag;

            instantiatedPrefab.transform.SetParent(hierarchyRoot.transform);

            mapObjectList.Add(instantiatedPrefab);

            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }

        GroundCheck();
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
                    Debug.Log("Trees destroyed in Editor.");
                    DestroyImmediate(mapObject);
                }
                else
                {
                    Debug.Log("Trees destroyed in game.");
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



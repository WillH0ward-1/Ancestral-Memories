using System.Collections.Generic;
using UnityEngine;

public class GenerateApples : MonoBehaviour
{
    [SerializeField] private GameObject hierarchyRoot;

    [SerializeField] private float sampleWidth = 0;
    [SerializeField] private float sampleHeight = 0;
    [SerializeField] private float sampleVolume = 0;

    [SerializeField] private GameObject[] apples;

    Vector3 regionSize;

    List<Vector3> points;

    [SerializeField] Vector3 minAppleScale;
    [SerializeField] Vector3 maxAppleScale;

    [SerializeField] float minimumAppleRadius = 70;

    [SerializeField] int tries = 30;

    public List<GameObject> appleList;

    private readonly string appleTag = "Food";

    [SerializeField, Range(0, 1)] float rotateTowardsNormal;
    [SerializeField] Vector2 rotationRange;

    [SerializeField] private float xOffset;
    [SerializeField] private float zOffset;
    // Start is called before the first frame update

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
        ResetPosOffset();

        // Volume instance has sample points & grid information (grid size & unit length).

        Renderer r = GetComponent<Renderer>();

        sampleWidth = r.bounds.size.x;
        sampleHeight = r.bounds.size.y;
        sampleVolume = r.bounds.size.z;

        xOffset = -sampleWidth / 2;
        zOffset = -sampleVolume / 2;

        regionSize = new Vector3(sampleWidth, sampleHeight, sampleVolume);

        points = PoissonDiscVolume.GeneratePoints(minimumAppleRadius, regionSize, tries);
        ApplePoissonDisc();

        SetOffset();

        DeleteStrayApples();
    }


    void ApplePoissonDisc()
    {
        foreach (Vector3 point in points)
        {
            Vector3 position = new Vector3(point.x, point.y, point.z);

            GameObject randomApple = GetRandomMapObject(apples);

            GameObject appleInstance = Instantiate(randomApple, position, Quaternion.identity);

            appleInstance.transform.Rotate(Vector3.up, Random.Range(rotationRange.x, rotationRange.y), Space.Self);

            appleInstance.tag = appleTag;

            Rigidbody rigidBody = appleInstance.GetComponent<Rigidbody>();

            rigidBody.useGravity = false;

            int appleLayer = LayerMask.NameToLayer(appleTag);
            appleInstance.layer = appleLayer;

            appleInstance.transform.SetParent(hierarchyRoot.transform);

            
            appleList.Add(appleInstance);
            //GroundCheck(instantiatedPrefab);
            //WaterCheck();
        }

    }

    [SerializeField] private Vector3 center = Vector3.one;
    [SerializeField] private float radius = 2;

    

    void DeleteStrayApples()
    {
        Collider[] hitColliders;

        foreach (GameObject apple in appleList)
        {

            hitColliders = Physics.OverlapSphere(apple.transform.position, radius);

            foreach (var hitCollider in hitColliders)
            {
                if (!hitCollider.CompareTag("TreeTrunk") || !hitCollider.CompareTag("Trees"))
                {
                    DestroyObject();
                }
            }

            void DestroyObject()
            {
                if (Application.isEditor)
                {
                    Debug.Log("Object destroyed in Editor.");
                    DestroyImmediate(apple);
                }
                else
                {
                    Debug.Log("Object destroyed in game.");
                    Destroy(apple);
                }
            }
        }

        ListCleanup();

        void ListCleanup()
        {
            for (var i = appleList.Count - 1; i > -1; i--)
            {
                if (appleList[i] == null)
                    appleList.RemoveAt(i);
            }
        }
    }

    void TriggerAppleFall()
    {
        foreach (GameObject apple in appleList)
        {
            apple.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    void SetOffset()
    {
        hierarchyRoot.transform.position = new Vector3(xOffset, 0, zOffset);
    }

    void ResetPosOffset()
    {
        hierarchyRoot.transform.position = new Vector3(0, 0, 0);
    }

    public void Clear()
    {
        appleList.Clear();

        if (Application.isEditor)
        {

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

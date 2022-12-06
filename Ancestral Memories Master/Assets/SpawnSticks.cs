using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSticks : MonoBehaviour
{
    private Transform tree;
    [SerializeField] private GameObject[] sticks;
    [SerializeField] private Mesh mesh;
    private Vector3[] vertices;

    private float dropRate;

    [SerializeField] private float minDropRate = 5;
    [SerializeField] private float maxDropRate = 30;


    [SerializeField] Vector3 minStickScale;
    [SerializeField] Vector3 maxStickScale;

    [SerializeField] float minStickGrowDuration;
    [SerializeField] float maxStickGrowDuration;

    [SerializeField] float minStickGrowthDelay;
    [SerializeField] float maxStickGrowthDelay;

    [SerializeField] float stickGrowthDelay;

    [SerializeField] private float stickGrowDuration = 30;

    [SerializeField] private MapObjGen generator;

    private void Awake()
    {
        tree = transform.parent;
        mesh = transform.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        StartCoroutine(
                DropStick());

        dropRate = Random.Range(minDropRate, maxDropRate);

        stickGrowDuration = Random.Range(minStickGrowDuration, maxStickGrowDuration);
        minStickGrowthDelay = 1f;
        maxStickGrowthDelay = 2f;
    }

    Vector3 zeroScale = new Vector3(0,0,0);

    // Update is called once per frame
    private IEnumerator DropStick()
    {
        Vector3 randomVertices = vertices[Random.Range(0, vertices.Length)];

        GameObject stickInstance = Instantiate(sticks[Random.Range(0, sticks.Length)], tree.localToWorldMatrix.MultiplyPoint3x4(randomVertices), Quaternion.identity);
        Rigidbody stickRigidBody = stickInstance.GetComponent<Rigidbody>();

        ScaleControl stickGrowControl = stickRigidBody.transform.GetComponent<ScaleControl>();

        Vector3 stickScaleDestination = new(maxStickScale.x, minStickScale.y, minStickScale.z);
        stickRigidBody.GetComponent<Renderer>().enabled = true;

        StartCoroutine(stickGrowControl.LerpScale(stickInstance.transform.gameObject, zeroScale, stickScaleDestination, stickGrowDuration, stickGrowthDelay));
        StartCoroutine(generator.WaitUntilGrown(stickInstance, stickGrowControl));
        stickInstance.transform.SetParent(stickInstance.transform, true);


        yield return new WaitForSeconds(dropRate);
            StartCoroutine(DropStick());

            yield break;

    }
}

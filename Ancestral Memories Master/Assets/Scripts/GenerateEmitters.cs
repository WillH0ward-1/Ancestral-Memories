using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateEmitters : MonoBehaviour
{
    /*
    [SerializeField] private GameObject emitter;
    private GameObject emitterInstance;

    private Mesh mesh;
    private Vector3[] vertices;

    [SerializeField] private MapObjGen mapObjGen;

    [SerializeField] int vertSampleFactor;

    public List<GameObject> emitters;

    private void Start()
    {
        mesh = transform.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
    }

    public IEnumerator EmitterGen()
    {
        int sampleDensity = vertices.Length / vertSampleFactor;

        for (int i = 0; i <= vertices.Length; i++)
        {
            // vertices[i] += Vector3.up * Time.deltaTime;

            if (i >= vertices.Length)
            {
                mapObjGen.StartCoroutine(mapObjGen.GenerateEmitterCheckers(emitters));
                yield break;
            }

            emitterInstance = Instantiate(emitter, transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]), Quaternion.identity, mapObjGen.emitterHierarchyRoot);

            emitters.Add(emitterInstance);

            i += sampleDensity;
        }


    }
    */
}
       





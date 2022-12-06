using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flammable : MonoBehaviour
{
    [SerializeField] private  GameObject fire;
    private Mesh mesh;
    private Vector3[] vertices;
    int sampleDensity;
    private Vector3 firePosition;
    private GameObject flammableObject;

    private ScaleControl scaleControl;

    float xMultiplier;
    float yMultiplier;
    float zMultiplier;

    [SerializeField] int vertSampleFactor;

    [SerializeField] private bool invertSpreadOrigin = false;

    [SerializeField] private int minFireSpreadDelay = 0;
    [SerializeField] private int maxFireSpreadDelay = 5;

    private FireController fireManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fire"))
        {
            StartCoroutine(CatchFire());
            return;
        }
    }

    Vector3 scale;

    private void Awake()
    {
        flammableObject = transform.gameObject;
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        scaleControl = GetComponent<ScaleControl>();
    }

    private Vector3 ScalarValue()
    {
        Vector3 scalar;

        xMultiplier = flammableObject.transform.localScale.x;
        yMultiplier = flammableObject.transform.localScale.y;
        zMultiplier = flammableObject.transform.localScale.z;

        scalar = new  (xMultiplier, yMultiplier, zMultiplier);

        return scalar;
    }

    private IEnumerator CatchFire()
    {
        int sampleDensity = vertices.Length / vertSampleFactor;

        int fireSpreadDelay = Random.Range(minFireSpreadDelay, maxFireSpreadDelay);

        Debug.Log(transform.gameObject.name + " has caught fire!" + "Sample Density: " + sampleDensity);

        if (!invertSpreadOrigin)
        {
            for (int i = 0; i < vertices.Length; i++)
            {

                // vertices[i] += Vector3.up * Time.deltaTime;

                if (i >= vertices.Length)
                {
                    yield break;
                }

                fireManager.StartFire(flammableObject.transform, flammableObject.transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]));

                i += sampleDensity;

                yield return new WaitForSeconds(fireSpreadDelay);

                yield return null;
            }

            StartCoroutine(BurnToGround());

        } else if (invertSpreadOrigin)
        {
            for (int i = vertices.Length; i --> 0;)
            {

                // vertices[i] += Vector3.up * Time.deltaTime;

                if (i <= 0)
                {
                    yield break;
                }

                GameObject fireInstance = Instantiate(fire, flammableObject.transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]), Quaternion.identity);
                fireInstance.transform.SetParent(flammableObject.transform, true);

                i -= sampleDensity;

                yield return new WaitForSeconds(fireSpreadDelay);

                yield return null;
            }

            StartCoroutine(BurnToGround());
        }

        yield return null;
    }

    [SerializeField] private float fallDuration = 2;

    private IEnumerator BurnToGround()
    {
        StartCoroutine(scaleControl.LerpScale(transform.gameObject, transform.localScale, new Vector3(transform.localScale.x, 0, transform.localScale.y), fallDuration, 0f));

        yield return new WaitForSeconds(fallDuration);

        Destroy(transform.gameObject);
        yield return null;

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerGrow : MonoBehaviour
{
    [SerializeField] private ScaleControl scaleControl;

    [SerializeField] private Vector3 scaleStart = new (0.001f, 0.001f, 0.001f);
    [SerializeField] private Vector3 growTarget = new(3f, 3f, 3f);

    [SerializeField] private Vector3 shrinkTarget = new(0.0001f, 0.0001f, 0.0001f);

    [SerializeField] private float growDuration;
    [SerializeField] private float shrinkDuration;

    [SerializeField] private float minGrowDuration;
    [SerializeField] private float maxGrowDuration;

    private float delay = 0f;

    private void Awake()
    {
        scaleControl = transform.GetComponent<ScaleControl>();
    }

    public void GrowFlower()
    {
        StartCoroutine(scaleControl.LerpScale(transform.gameObject, shrinkTarget, growTarget, growDuration, delay));
    }

    public void ShrinkFlower()
    {

        StartCoroutine(scaleControl.LerpScale(transform.gameObject, growTarget, shrinkTarget, shrinkDuration, delay));
        StartCoroutine(WaitUntilShrunk());
    }

    public IEnumerator WaitUntilShrunk()
    {
        yield return new WaitUntil(() => scaleControl.isFullyGrown);

        transform.gameObject.SetActive(false); // Return the object to the pool.
    }

}

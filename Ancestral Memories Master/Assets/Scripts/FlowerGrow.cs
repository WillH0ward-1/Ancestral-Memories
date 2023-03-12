using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerGrow : MonoBehaviour
{
    [SerializeField] private ScaleControl scaleControl;

    [SerializeField] private Vector3 scaleStart;
    [SerializeField] private Vector3 scaleTarget;

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
        scaleStart = new(0, 0, 0);
        scaleTarget = new(5, 5, 5);

        StartCoroutine(scaleControl.LerpScale(transform.gameObject, scaleStart, scaleTarget, growDuration, delay));
    }

    public void ShrinkFlower()
    {
        scaleTarget = new(0.1f, 0.1f, 0.1f);
        shrinkDuration = 2f;

        StartCoroutine(scaleControl.LerpScale(transform.gameObject, transform.localScale, scaleTarget, shrinkDuration, delay));
        StartCoroutine(WaitUntilShrunk());
    }

    public IEnumerator WaitUntilShrunk()
    {
        yield return new WaitUntil(() => scaleControl.isFullyGrown);

        transform.gameObject.SetActive(false); // Return the object to the pool.
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDeathManager : MonoBehaviour
{
    public bool treeDead = false;

    public MapObjGen mapObjGen;
    public ScaleControl scaleControl;

    public Vector3 newScale;

    public float fallDuration = 1;

    public void Fall()
    {
        StartCoroutine(FallToGround());
    }

    [SerializeField] private float deathSinkBuffer = 2;
    [SerializeField] private float deathSinkSpeed = 2;

    public bool treeFalling = false;
    public float yVal = 0.001f;

    private IEnumerator FallToGround()
    {
        treeDead = true;
        treeFalling = true;

        newScale = new Vector3(transform.localScale.x / 2, yVal, transform.localScale.y / 2);

        // StopCoroutine(scaleControl.LerpScale(transform.gameObject, transform.localScale, transform.localScale, 0, 0));
        StartCoroutine(scaleControl.LerpScale(transform.gameObject, transform.localScale, newScale, fallDuration, 0.1f));

        yield return new WaitUntil(() => scaleControl.isFullyGrown);
        
        treeFalling = false;

        StartCoroutine(Regrow());
        // StartCoroutine(PullUnderground(deathSinkSpeed));

        yield break;
    }

    float yPos;
    float regrowBuffer = 10;

    public IEnumerator PullUnderground(float duration)
    {
        float time = 0;

        yPos = transform.position.y - 10;

        Vector3 newPos = new Vector3(transform.localPosition.x, yPos, transform.localPosition.y);
        yPos = transform.position.y - 10;

        Vector3 position = transform.localPosition;

        while (time <= 1)
        {
            time += Time.deltaTime / duration;

            yPos = transform.position.y;

            transform.localPosition = Vector3.Lerp(position, newPos, time);

            yield return null;
        }

        StartCoroutine(Regrow());

        yield break;
    }

    public IEnumerator Regrow()
    {
        yield return new WaitForSeconds(regrowBuffer);
        treeDead = false;
        transform.gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        mapObjGen.GrowTrees(transform.gameObject);

        yield break;
    }
}

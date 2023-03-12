using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDeathManager : MonoBehaviour
{
    public bool treeDead = false;

    public MapObjGen mapObjGen;
    public ScaleControl scaleControl;

    [SerializeField] private Vector3 newScale;

    [SerializeField] private float fallDuration = 0.1f;

    private Interactable interactable;

    public TreeAudioSFX treeAudioSFX;

    private void Awake()
    {
        scaleControl = transform.GetComponent<ScaleControl>();
    }

    public void Fall()
    {
        interactable = transform.GetComponentInChildren<Interactable>();
        interactable.enabled = false;

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
        newScale = new Vector3(transform.localScale.x / 5, yVal, transform.localScale.y / 5);

        StopCoroutine(scaleControl.LerpScale(transform.gameObject, transform.localScale, transform.localScale, 0, 0));
        scaleControl.StartCoroutine(scaleControl.LerpScale(transform.gameObject, transform.localScale, newScale, fallDuration, 0));

        yield return new WaitForSeconds(fallDuration);
        treeAudioSFX.StartTreeHitGroundSFX();

        treeFalling = false;

        StartCoroutine(scaleControl.LerpScale(transform.gameObject, transform.localScale, transform.localScale / 100, 2, 0));

        StartCoroutine(Regrow());
        //tartCoroutine(PullUnderground(deathSinkSpeed));

        yield break;
    }

    float yPos;
    float regrowBuffer = 10;

    /*
    public IEnumerator PullUnderground(float duration)
    {
        float time = 0;

        yPos = transform.position.y - 10;

        Vector3 newPos = new Vector3(transform.localPosition.x, transform.localPosition.y - 10, transform.localPosition.z);

        Vector3 position = transform.localPosition;

        while (time <= 1)
        {
            time += Time.deltaTime / duration;

            yPos = transform.position.y;

            transform.localPosition = Vector3.Lerp(transform.localPosition , newPos, time);

            yield return null;
        }

        StartCoroutine(Regrow());

        yield break;
    }
    */

    public IEnumerator Regrow()
    {
        regrowBuffer = Random.Range(10, 20);

        yield return new WaitForSeconds(regrowBuffer);
        treeDead = false;
        transform.gameObject.transform.localRotation = new Quaternion(0, 0, 0, 0);

        if (transform.CompareTag("Trees"))
        {
            mapObjGen.GrowTrees(transform.gameObject);
        }

        yield break;
    }
}

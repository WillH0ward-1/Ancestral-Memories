using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseEffectControl : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private GameObject scaleObject;

    [SerializeField] private Vector3 scaleStart;
    [SerializeField] private Vector3 scaleDestination;
    [SerializeField] private float duration;
    [SerializeField] private float delay;

    private ScaleControl scaleControl;
    private GameObject pulseObject;

    void Start()
    {
        scaleControl = scaleObject.GetComponent<ScaleControl>(); 
    }

    public void Pulse()
    {
        pulseObject = Instantiate(scaleObject, player.transform);
        scaleControl = pulseObject.GetComponent<ScaleControl>();

        StartCoroutine(scaleControl.LerpScale(pulseObject, scaleStart, scaleDestination, duration, delay));
        StartCoroutine(WaitUntilGrown(pulseObject));
    }

    private IEnumerator WaitUntilGrown(GameObject pulseObject)
    {
        ScaleControl scaleControl = pulseObject.GetComponent<ScaleControl>();
        yield return new WaitUntil(() => scaleControl.isFullyGrown);
        Destroy(pulseObject);
        yield break;
    }
   
}

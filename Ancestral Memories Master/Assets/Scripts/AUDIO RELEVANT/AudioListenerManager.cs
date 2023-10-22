using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using FMODUnity;

public class AudioListenerManager : MonoBehaviour
{
    [SerializeField] private GameObject attenuationObject;
    [SerializeField] private GameObject defaultAttenuator;

    [SerializeField] LerpParams lerpParams;

    //private StudioListener listener;

    [SerializeField] private float toDefaultSpeed = 1f;

    private Player player;

    //[SerializeField] private string listenerTag = "ListenerOrigin";
    private void Start()
    {
     
        //defaultAttenuator = player.transform.root.gameObject;

        attenuationObject.transform.position = defaultAttenuator.transform.position;
        attenuationObject.transform.SetParent(defaultAttenuator.transform);

        //SetDefaultAttenuation();wea
    }

    public void SetDefaultAttenuation()
    {
        StartCoroutine(MoveAudioListener(defaultAttenuator, toDefaultSpeed));
        attenuationObject.transform.SetParent(defaultAttenuator.transform);
    }

    public IEnumerator MoveAudioListener(GameObject target, float duration)
    {
        float time = 0f;

        Vector3 newPos = target.transform.position;
        attenuationObject.transform.SetParent(target.transform);

        System.Func<float, float> func = Lerp.GetLerpFunction(lerpParams.lerpType);

        while (time <= 1)
        {
            attenuationObject.transform.position = Vector3.Lerp(attenuationObject.transform.position, newPos, func(time));
            time += Time.deltaTime / duration;

            yield return null;
        }

        if (time >= 1)
        {
            attenuationObject.transform.position = newPos;
            yield break;
        }
    }

}

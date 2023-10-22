using UnityEngine;
//using FMODUnity;
//using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using System;

public class CrowAudioManager : MonoBehaviour
{
    public GameObject target;
    public Camera cam;

    [SerializeField] private Rigidbody rigidBody;
    // [EventRef] private string wingflapEvent;
    private float distance;
    private Animator animator;
    private Animation animation;
    private FlockChild flock;

    // Start is called before the first frame update
    void Awake()
    {
        target = transform.gameObject;
        flock = transform.GetComponentInChildren<FlockChild>();
        animation = flock._model.GetComponent<Animation>();
    }

    // Update is called once per frame


    private bool IsVisible(Camera c, GameObject target)
    {
        var planes = GeometryUtility.CalculateFrustumPlanes(c);

        var point = target.transform.position;

        foreach (var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
            {
                return false;
            }
        }
        return true;
    }

    /*
    private void Update()
    {
        var targetRender = target.GetComponent<Renderer>();

        if (IsVisible(cam, target) && animation.clip.name == flock._spawner._flapAnimation)
        {
            Debug.Log("On Screen!");
            
        }

         else
        {

        }

        return;
    }
    */


    public void PlayWingFlap()
    {
        /*
        EventInstance wingFlapInstance = RuntimeManager.CreateInstance(wingflapEvent);
        RuntimeManager.AttachInstanceToGameObject(wingFlapInstance, transform, rigidBody);

        wingFlapInstance.start();
        wingFlapInstance.release();
        */
    }
}

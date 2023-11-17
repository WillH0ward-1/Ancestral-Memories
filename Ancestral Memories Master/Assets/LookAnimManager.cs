using System.Collections;
using System.Collections.Generic;
using FIMSpace.FLook;
using UnityEngine;

public class LookAnimManager : MonoBehaviour
{
    private FLookAnimator lookAnimator;

    void Awake()
    {
        lookAnimator = GetComponentInChildren<FLookAnimator>();
        if (lookAnimator == null)
        {
            Debug.LogWarning("FLookAnimator not found on " + gameObject.name);
        }
    }

    public void LookAt(Transform target)
    {
        if (lookAnimator != null && !lookAnimator.enabled)
        {
            lookAnimator.enabled = true;
            lookAnimator.ObjectToFollow = target;
        }
    }

    public void DisableLookAt()
    {
        if (lookAnimator != null && lookAnimator.enabled)
        {
            lookAnimator.enabled = false;
        }
    }
}

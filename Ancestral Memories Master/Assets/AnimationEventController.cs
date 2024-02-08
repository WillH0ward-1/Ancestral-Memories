using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventController : MonoBehaviour
{
    AudioFootStepManager audioFootStepManager;

    private void Awake()
    {
        audioFootStepManager = transform.GetComponentInChildren<AudioFootStepManager>();
    }

    public void TriggerFootStep()
    {
        audioFootStepManager.TriggerFootstep();
    }
}

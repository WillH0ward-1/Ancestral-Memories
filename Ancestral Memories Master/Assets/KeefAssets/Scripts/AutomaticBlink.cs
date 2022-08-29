using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticBlink : StateMachineBehaviour
{

    readonly float blinkMinTime = 1;
    readonly float blinkMaxTime = 3;

    float blinkTimer = 0;

    string[] blinkTriggers = { "Blink" };

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (blinkTimer <= 0)
        {

            BlinkRandomEye(animator);
            blinkTimer = Random.Range(blinkMinTime, blinkMaxTime);
        } else
        {
            blinkTimer -= Time.deltaTime;
        }
    }

    void BlinkRandomEye(Animator animator)
    {
        System.Random rnd = new System.Random();
        int eyePosition = rnd.Next(blinkTriggers.Length);
        string blinkTrigger = blinkTriggers[eyePosition];
        animator.SetTrigger(blinkTrigger);
    }
}

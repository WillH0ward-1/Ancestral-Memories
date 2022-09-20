using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> activeAnimators = new List<GameObject>();
    [SerializeField] private List<GameObject> inactiveAnimators = new List<GameObject>();

    private ControlAlpha alphaControl;

    [SerializeField] private float animationCrossFade = 2f;

    private Animator activeAnimator, inactiveAnimator;

    private string currentState;

    private void Awake()
    {
        InitAnimators();
    }

    void InitAnimators()
    {
        var humanState = alphaControl.humanObject;
        var monkeyState = alphaControl.monkeyObject;


        if (alphaControl.playerIsHuman == false) // If player is monkey
        {
            activeAnimators.Remove(humanState);
            activeAnimators.Add(monkeyState);

            inactiveAnimators.Remove(monkeyState);
            inactiveAnimators.Add(humanState);

        }
        else if (alphaControl.playerIsHuman == true)
        {// If player is Human

            activeAnimators.Remove(monkeyState);
            activeAnimators.Add(humanState);

            inactiveAnimators.Remove(humanState);
            inactiveAnimators.Add(monkeyState);
        }
    }

   public void changeState(string newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;

        ChangeAnimationState(newState);
    }


    public void SwitchAnimators()
    {
        AssignAnimators();
        AssignInactiveAnimators();
    }

    void AssignAnimators()
    {
        foreach (GameObject g in activeAnimators)
        {
            foreach (Animator a in g.GetComponentsInChildren<Animator>())
            {
                activeAnimator = a;
            }
        }
    }

    void AssignInactiveAnimators()
    {
        foreach (GameObject g in inactiveAnimators)
        {
            foreach (Animator a in g.GetComponentsInChildren<Animator>())
            {
                inactiveAnimator = a;
            }
        }
    }

    //private float crossFadeLength;

    public void ChangeAnimationState(string newState)
    {
        SwitchAnimators();

        float crossFadeLength = animationCrossFade;

        if (currentState == newState)
        {
            return;
        }

        activeAnimator.CrossFadeInFixedTime(newState, crossFadeLength);
        inactiveAnimator.CrossFadeInFixedTime(newState, crossFadeLength);

        currentState = newState;
    }

    public void AdjustAnimationSpeed(float newSpeed)
    {
        activeAnimator.speed = newSpeed;
        inactiveAnimator.speed = newSpeed;
    }
}

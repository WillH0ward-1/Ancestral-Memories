using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBloom : MonoBehaviour
{
    public Animator flowerAnim;

    public bool bloom;

    const string FLOWER_OPEN = "DaisyOpen";
    const string FLOWER_CLOSE = "Daisyclose";
    private string currentState;
    private Animator animator;

    [SerializeField] private float animationCrossFade = 0.5f;

    void Awake()
    {
        animator = transform.GetComponent<Animator>();
    }

    void OnEnable()
    {
        ChangeAnimationState(FLOWER_OPEN);
    }

    public virtual void ChangeAnimationState(string newState)
    {

        float crossFadeLength = animationCrossFade;

        if (currentState == newState)
        {
            return;
        }

        animator.CrossFadeInFixedTime(newState, crossFadeLength);

        currentState = newState;
    }
}

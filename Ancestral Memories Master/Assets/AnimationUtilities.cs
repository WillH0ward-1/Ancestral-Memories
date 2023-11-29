using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationUtilities
{
    public static IEnumerator AnimateWithVariableSpeed(Animator animator, string animationState, float minSpeed, float maxSpeed, float duration, System.Action onCompletion = null)
    {
        float time = 0;
        float currentSpeed = animator.speed;

        while (time <= duration)
        {
            float targetSpeed = Random.Range(minSpeed, maxSpeed);
            animator.speed = Mathf.Lerp(currentSpeed, targetSpeed, time / duration);

            animator.Play(animationState, -1, time / duration);

            time += Time.deltaTime;
            yield return null;
        }

        animator.speed = 1f; // Reset to default speed
        onCompletion?.Invoke();
    }

    public static IEnumerator PlayRandomAnimations(Animator animator, List<string> animations, float minDurationFactor, float maxDurationFactor)
    {
        while (true) // This loop will run indefinitely
        {
            // Select a random animation
            string selectedAnimation = animations[Random.Range(0, animations.Count)];

            // Play the selected animation
            animator.Play(selectedAnimation);

            // Calculate duration based on the current animation's length
            float animLength = GetAnimLength(animator);
            float randomDuration = animLength * Random.Range(minDurationFactor, maxDurationFactor);

            // Wait for the duration of the animation
            yield return new WaitForSeconds(randomDuration);
        }
    }

    public static float GetAnimLength(Animator animator)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length / (stateInfo.speed != 0 ? stateInfo.speed : 1);
    }
}

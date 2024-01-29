using FIMSpace.FLook;
using UnityEngine;

public class LookAnimManager : MonoBehaviour
{
    [SerializeField] private FLookAnimator lookAnimator;
    private Transform currentTarget;

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
        if (lookAnimator != null)
        {
            if (currentTarget != target) // If the new target is different from the current one
            {
                lookAnimator.enabled = true;
                lookAnimator.ObjectToFollow = target;
                currentTarget = target; // Update the current target reference
            }
            // If the target is the same as the current one, no need to update
        }
    }

    public void DisableLookAt()
    {
        if (lookAnimator != null && lookAnimator.enabled)
        {
            lookAnimator.enabled = false;
            currentTarget = null; // Clear the current target reference
        }
    }
}

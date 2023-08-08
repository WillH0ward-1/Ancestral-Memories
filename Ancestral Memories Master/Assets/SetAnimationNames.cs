using UnityEngine;

public class AnimationConstants : MonoBehaviour
{
    public string[] animationNames;

    public bool AreNamesDifferent(string[] newNames)
    {
        if (animationNames == null || animationNames.Length != newNames.Length)
        {
            return true;
        }

        for (int i = 0; i < animationNames.Length; i++)
        {
            if (animationNames[i] != newNames[i])
            {
                return true;
            }
        }

        return false;
    }

    public void SetAnimationNames(string[] newNames)
    {
        animationNames = newNames;
    }
}

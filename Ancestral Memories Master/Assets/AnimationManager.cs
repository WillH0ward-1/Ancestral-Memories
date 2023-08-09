using UnityEngine;
using System.Collections.Generic;

public static class AnimationManager
{
    private static Dictionary<int, string> lastAnimations = new Dictionary<int, string>();

    public static string GetRandomAnimation(GameObject character, params string[] animations)
    {
        // Check if there are any animations in the array
        if (animations == null || animations.Length == 0)
        {
            Debug.LogError("No animations provided!");
            return null;
        }

        // Get the unique identifier for this character
        int characterId = character.GetInstanceID();

        // If there's only one animation, return it
        if (animations.Length == 1)
        {
            lastAnimations[characterId] = animations[0];
            return animations[0];
        }

        // If there are more animations, make sure not to repeat the last one for this character
        string randomAnimation;
        string lastAnimationName;
        lastAnimations.TryGetValue(characterId, out lastAnimationName);
        do
        {
            randomAnimation = animations[Random.Range(0, animations.Length)];
        } while (randomAnimation == lastAnimationName);

        lastAnimations[characterId] = randomAnimation;
        return randomAnimation;
    }
}

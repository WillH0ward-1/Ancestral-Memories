using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationGroupFilter : MonoBehaviour
{

    public static Dictionary<string, List<string>> FilterAnimations(string[] animationNames)
    {
        Dictionary<string, List<string>> animationGroups = new Dictionary<string, List<string>>();

        foreach (string name in animationNames)
        {
            // Split by underscore to identify the category and state
            string[] parts = name.Split('_');
            string state = parts[0];
            string category = null;

            // Check if the animation name contains any of the specific categories
            if (parts.Length > 1)
            {
                string potentialCategory = parts[1];
                if (potentialCategory.Contains("Neanderthal") ||
                    potentialCategory.Contains("MidSapien") ||
                    potentialCategory.Contains("Sapien") ||
                    potentialCategory.Contains("Elder"))
                {
                    category = potentialCategory.Split(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' })[0];
                }
            }

            string groupKey = category != null ? $"{state}_{category}" : state;

            if (!animationGroups.ContainsKey(groupKey))
            {
                animationGroups[groupKey] = new List<string>();
            }
            animationGroups[groupKey].Add(name);
        }

        return animationGroups;
    }


}


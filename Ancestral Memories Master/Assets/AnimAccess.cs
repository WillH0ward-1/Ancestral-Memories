using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

public class AnimAccess : MonoBehaviour
{
    public static AnimAccess Instance { get; private set; }
    private HumanControllerAnimGroups animGroups;
    private Dictionary<string, string> lastPlayedAnimations = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            animGroups = GetComponent<HumanControllerAnimGroups>();
            if (animGroups == null)
            {
                Debug.LogError("HumanControllerAnimGroups component not found!");
            }
            DontDestroyOnLoad(gameObject); // Optional: Keep instance alive across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance exists
        }
    }

    public string GetRandomAnimation(string prefix)
    {
        if (animGroups == null) return null;

        FieldInfo fieldInfo = typeof(HumanControllerAnimGroups).GetField(prefix, BindingFlags.Public | BindingFlags.Instance);
        if (fieldInfo == null)
        {
            Debug.LogError($"No animation list found with the prefix: {prefix}");
            return null;
        }

        var animations = fieldInfo.GetValue(animGroups) as List<string>;
        if (animations == null || animations.Count == 0)
        {
            Debug.LogError($"Animation list for prefix '{prefix}' is empty or not a List<string>.");
            return null;
        }

        return GetUniqueRandomAnimation(animations, prefix);
    }

    private string GetUniqueRandomAnimation(List<string> animations, string prefix)
    {
        string selectedAnimation;

        do
        {
            selectedAnimation = animations[Random.Range(0, animations.Count)];
        }
        while (animations.Count > 1 && lastPlayedAnimations.ContainsKey(prefix) && selectedAnimation == lastPlayedAnimations[prefix]);

        lastPlayedAnimations[prefix] = selectedAnimation;
        return selectedAnimation;
    }
}

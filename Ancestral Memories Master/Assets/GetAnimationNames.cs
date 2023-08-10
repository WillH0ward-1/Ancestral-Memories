using UnityEngine;
using UnityEditor.Animations;
using System.Text;
using System.IO;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;

public class GetAnimationNames : MonoBehaviour
{
    public Animator targetAnimator; // Manually set the Animator target

    public bool GetAllAnimationNames(out (string AnimatorName, string AnimatorController, string Avatar, string[] AnimationNames) result)
    {
        result = (null, null, null, null);

        if (targetAnimator == null)
        {
            Debug.LogError("Animator is not set!");
            return false;
        }

        AnimatorController ac = targetAnimator.runtimeAnimatorController as AnimatorController;
        string avatarName = targetAnimator.avatar != null ? targetAnimator.avatar.name : "None";

        int totalStates = 0;
        foreach (AnimatorControllerLayer layer in ac.layers)
        {
            totalStates += layer.stateMachine.states.Length;
        }

        string[] names = new string[totalStates];
        int index = 0;

        foreach (AnimatorControllerLayer layer in ac.layers)
        {
            foreach (ChildAnimatorState state in layer.stateMachine.states)
            {
                names[index] = state.state.name;
                index++;
            }
        }

        System.Array.Sort(names);
        result = (targetAnimator.name, ac.name, avatarName, names);
        return true;
    }

    public void WriteAnimationConstantsClass(string[] animationNames)
    {
        AnimatorController ac = targetAnimator.runtimeAnimatorController as AnimatorController;
        string scriptName = $"{ac.name}Animations";
        StringBuilder content = new StringBuilder();
        content.AppendLine("using UnityEngine;");
        content.AppendLine("public class " + scriptName + " : MonoBehaviour");
        content.AppendLine("{");

        foreach (string name in animationNames)
        {
            string validName = name.Replace(" ", "_");
            content.AppendLine($"    public const string {validName} = \"{name}\";");
        }

        content.AppendLine("}");

        WriteAndAttachScript(scriptName, content);
    }

    public void WriteAnimationGroupsClass(string[] animationNames)
    {
        AnimatorController ac = targetAnimator.runtimeAnimatorController as AnimatorController;
        string scriptName = $"{ac.name}AnimGroups";

        // Organize animations by their states
        Dictionary<string, List<string>> animationGroups = new Dictionary<string, List<string>>();
        foreach (string name in animationNames)
        {
            // Assuming state name is split by the underscore, and the specific category is in the second part
            string[] parts = name.Split('_');
            string state = parts[0];
            string category = parts.Length > 1 ? parts[1] : "General";

            // Combine the state and category to create a specific group
            string groupKey = $"{state}_{category}";
            if (!animationGroups.ContainsKey(groupKey))
            {
                animationGroups[groupKey] = new List<string>();
            }
            animationGroups[groupKey].Add(name);
        }

        StringBuilder content = new StringBuilder();
        content.AppendLine("using UnityEngine;");
        content.AppendLine("using System.Collections.Generic;");
        content.AppendLine("public class " + scriptName + " : MonoBehaviour");
        content.AppendLine("{");

        foreach (var group in animationGroups)
        {
            string groupName = group.Key.Replace(" ", "_");
            content.AppendLine($"    public List<string> {groupName} = new List<string> {{ {string.Join(", ", group.Value.Select(n => $"\"{n}\""))} }};");
        }

        content.AppendLine("}");

        WriteAndAttachScript(scriptName, content);
    }


    private void WriteAndAttachScript(string scriptName, StringBuilder content)
    {
        string path = $"Assets/Scripts/{scriptName}.cs";
        File.WriteAllText(path, content.ToString());
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

        Type scriptType = Type.GetType(scriptName + ", Assembly-CSharp");
        if (scriptType != null)
        {
            if (gameObject.GetComponent(scriptType) == null)
            {
                gameObject.AddComponent(scriptType);
                Debug.Log($"{scriptName} component added to the GameObject.");
            }
            else
            {
                Debug.LogWarning($"{scriptName} component already added to the GameObject. Skipping addition.");
            }
        }
        else
        {
            Debug.LogError($"Unable to find type {scriptName}. Make sure the script has compiled successfully.");
        }

        Debug.Log($"{scriptName} class updated or created");
    }
}

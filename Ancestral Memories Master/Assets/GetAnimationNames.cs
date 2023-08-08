using UnityEngine;
using UnityEditor.Animations;
using System.Text;
using System.IO;
using UnityEditor;
using System;

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
        string path = $"Assets/Scripts/{scriptName}.cs";

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

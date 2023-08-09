using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

public class RenameAnimationClips : MonoBehaviour
{
    public RuntimeAnimatorController animatorController; // Reference to the Animator Controller asset

    private Dictionary<string, string> originalNames = new Dictionary<string, string>(); // Store original names

    [ContextMenu("Rename Animation Clips")]
    public void RenameAllAnimationClips()
    {
        if (animatorController == null)
        {
            Debug.LogError("(Clip Rename) Animator Controller Reference is not set. Please select an Animator Controller.");
            return;
        }

        AnimatorController ac = animatorController as AnimatorController;
        if (ac == null)
        {
            Debug.LogError("(Clip Rename) The provided asset is not an AnimatorController.");
            return;
        }

        originalNames.Clear(); // Clear previous original names

        foreach (AnimatorControllerLayer layer in ac.layers)
        {
            foreach (ChildAnimatorState state in layer.stateMachine.states)
            {
                AnimationClip clip = state.state.motion as AnimationClip;
                if (clip != null)
                {
                    string clipPath = AssetDatabase.GetAssetPath(clip);
                    originalNames[clipPath] = clip.name; // Store the original name

                    string newName = state.state.name; // Use state's name
                    AssetDatabase.RenameAsset(clipPath, newName);
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("(Clip Rename) Animation Clips successfully renamed.");
    }

    [ContextMenu("Undo Rename")]
    public void UndoRename()
    {
        foreach (var item in originalNames)
        {
            string clipPath = item.Key;
            string originalName = item.Value;
            AssetDatabase.RenameAsset(clipPath, originalName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("(Undo Clip Rename) Animation Clip names reverted.");
        originalNames.Clear(); // Optionally clear the dictionary after undoing
    }
}

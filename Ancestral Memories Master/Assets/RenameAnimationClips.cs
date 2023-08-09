using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Animations;

public class RenameAnimationClips : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component

    private Dictionary<string, string> originalNames = new Dictionary<string, string>(); // Store original names

    [ContextMenu("Rename Animation Clips")]
    public void RenameAllAnimationClips()
    {
        if (animator == null)
        {
            Debug.LogError("(Clip Rename) Animator Reference is not set. Please select an Animator.");
            return;
        }

        originalNames.Clear(); // Clear previous original names

        AnimatorController ac = animator.runtimeAnimatorController as AnimatorController;
        if (ac == null)
        {
            Debug.LogError("(Clip Rename) The provided Animator does not use an AnimatorController.");
            return;
        }

        AnimatorControllerLayer[] layers = ac.layers;
        for (int i = 0; i < layers.Length; i++)
        {
            ChildAnimatorState[] states = layers[i].stateMachine.states;
            for (int j = 0; j < states.Length; j++)
            {
                AnimationClip clip = states[j].state.motion as AnimationClip;
                if (clip != null)
                {
                    string clipPath = AssetDatabase.GetAssetPath(clip);
                    originalNames[clipPath] = clip.name; // Store the original name

                    string newName = clip.name; // Use clip's name
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

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.Collections.Generic;

public class RenameAnimationClips : MonoBehaviour
{
    public RuntimeAnimatorController runtimeAnimatorController;
    public bool CancelOperation = false;

    private Dictionary<AnimationClip, string> originalNames = new Dictionary<AnimationClip, string>();

    [ContextMenu("Rename Animation Clips")]
    public void RenameAllAnimationClips()
    {
        if (runtimeAnimatorController == null)
        {
            Debug.LogError("(Clip Rename) Animator Controller Reference is not set. Please select a Runtime Animator Controller.");
            return;
        }

        AnimatorController animatorController = runtimeAnimatorController as AnimatorController;
        if (animatorController == null)
        {
            Debug.LogError("(Clip Rename) The provided asset is not an AnimatorController.");
            return;
        }

        Debug.Log("(Clip Rename) Starting rename process...");

        originalNames.Clear();
        HashSet<AnimationClip> processedClips = new HashSet<AnimationClip>();

        foreach (AnimatorControllerLayer layer in animatorController.layers)
        {
            Debug.Log("(Clip Rename) Processing layer: " + layer.name);

            foreach (ChildAnimatorState state in layer.stateMachine.states)
            {
                Debug.Log("(Clip Rename) Processing state: " + state.state.name);

                if (CancelOperation)
                {
                    Debug.LogWarning("(Clip Rename) Operation was canceled.");
                    CancelOperation = false;
                    return;
                }

                AnimationClip oldClip = state.state.motion as AnimationClip;
                if (oldClip != null && !processedClips.Contains(oldClip))
                {
                    processedClips.Add(oldClip); // Mark as processed

                    string originalName = oldClip.name;
                    string newName = state.state.name;

                    if (originalName != newName)
                    {
                        Debug.Log("(Clip Rename) Renaming GameObject and clip: " + newName);

                        // Duplicate the old clip
                        AnimationClip newClip = Instantiate(oldClip);

                        // Rename the new clip
                        newClip.name = newName;

                        // Save and refresh the new clip
                        AssetDatabase.CreateAsset(newClip, AssetDatabase.GetAssetPath(oldClip));
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        // Store the original name
                        originalNames[newClip] = originalName;

                        // Delete the old clip
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(oldClip));
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();

                        // Get the root GameObject from the path of the child GameObject
                        GameObject rootObject = GetRootObjectFromChildPath(AssetDatabase.GetAssetPath(newClip));

                        if (rootObject != null)
                        {
                            rootObject.name = newName;
                            EditorUtility.SetDirty(rootObject);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                    }
                }
            }
        }

        Debug.Log("(Clip Rename) Animation Clips and GameObjects successfully renamed.");
    }

    [ContextMenu("Undo Rename")]
    public void UndoRename()
    {
        foreach (var item in originalNames)
        {
            AnimationClip newClip = item.Key;
            string originalName = item.Value;

            string newClipPath = AssetDatabase.GetAssetPath(newClip);
            string oldClipPath = newClipPath.Replace(newClip.name + ".anim", originalName + ".anim");

            AssetDatabase.DeleteAsset(newClipPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssetDatabase.MoveAsset(oldClipPath, newClipPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            GameObject rootObject = GetRootObjectFromChildPath(newClipPath);

            if (rootObject != null)
            {
                rootObject.name = originalName;
                EditorUtility.SetDirty(rootObject);
            }
        }

        Debug.Log("(Undo Clip Rename) Animation Clip names and GameObjects reverted.");
        originalNames.Clear();
    }

    private GameObject GetRootObjectFromChildPath(string childPath)
    {
        string fullPath = childPath.Substring(0, childPath.Length - (childPath.LastIndexOf('/') + 1));
        return AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
    }
}

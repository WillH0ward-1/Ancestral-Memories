using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RenameAnimationClips))]
public class RenameAnimationClipsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RenameAnimationClips script = (RenameAnimationClips)target;

        if (GUILayout.Button("Rename Animation Clips"))
        {
            script.RenameAllAnimationClips();
        }

        if (GUILayout.Button("Undo Rename"))
        {
            script.UndoRename();
        }

        if (GUILayout.Button("Cancel Operation"))
        {
            script.CancelOperation = true;
        }
    }
}

using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(GetAnimationNames))]
public class GetAnimationNamesEditor : Editor
{
    private string folderName = "AnimationNameRefs";
    private string fileName = "AnimationRefs.txt";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // Draw the default inspector

        GetAnimationNames myScript = (GetAnimationNames)target;

        if (GUILayout.Button("Write Animation Names To File"))
        {
            if (myScript.GetAllAnimationNames(out var result))
            {
                WriteToFile(result.AnimatorName, result.AnimatorController, result.Avatar, result.AnimationNames);
            }

        }

        if (GUILayout.Button("Set Animation Names In Scene"))
        {
            if (myScript.GetAllAnimationNames(out var result))
            {
                myScript.WriteAnimationConstantsClass(result.AnimationNames); // Create the constant class
                myScript.WriteAnimationGroupsClass(result.AnimationNames); // Create the groups class
            }
        }

    }

#if UNITY_EDITOR
    public static void WriteToFile(string animatorName, string animatorController, string avatar, string[] animationNames)
    {
        string folderName = "AnimationNameRefs";
        string fileName = "AnimationRefs.txt";

        // Check if the folder exists and create it if not
        if (!AssetDatabase.IsValidFolder("Assets/" + folderName))
        {
            AssetDatabase.CreateFolder("Assets", folderName);
        }

        // Prepare the content
        string timestamp = "Last Updated: " + System.DateTime.Now.ToString("hh:mm:ss tt d MMMM yyyy");
        string targetAnimName = "Target Animator: " + animatorName;
        string controllerName = "Controller: " + animatorController;
        string avatarName = "Rig Avatar: " + avatar;
        string[] content = new string[animationNames.Length + 5]; // Add extra lines for the line breaks
        content[0] = targetAnimName;
        content[1] = controllerName;
        content[2] = avatarName;
        content[3] = timestamp;
        content[4] = ""; // This is the line break
        System.Array.Copy(animationNames, 0, content, 5, animationNames.Length); // Start copying from index 5

        string newAssetPath = "Assets/" + folderName + "/" + fileName;
        System.IO.File.WriteAllLines(newAssetPath, content);

        // Refresh the Asset Database to recognize the new file
        AssetDatabase.Refresh();

        Debug.Log("Animation names written to " + newAssetPath);
    }
#endif
}

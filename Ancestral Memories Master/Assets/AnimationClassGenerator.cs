using System.Text;
using UnityEditor;
using UnityEngine;

public static class AnimationClassGenerator
{
    public static void GenerateClass(string[] animationNames)
    {
        StringBuilder classContent = new StringBuilder();

        classContent.AppendLine("public static class AnimationConstants");
        classContent.AppendLine("{");

        foreach (string animationName in animationNames)
        {
            string constantName = animationName.Replace(" ", "_"); // Replace spaces with underscores if needed
            classContent.AppendLine($"    public const string {constantName} = \"{animationName}\";");
        }

        classContent.AppendLine("}");

        string folderPath = "Assets/AnimationNameRefs";
        string filePath = folderPath + "/AnimationConstants.cs";

        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "AnimationNameRefs");
        }

        System.IO.File.WriteAllText(filePath, classContent.ToString());

        AssetDatabase.Refresh();

        Debug.Log("Animation constants class written to " + filePath);
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TempleGenerator))]
public class TempleGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TempleGenerator templeGen = (TempleGenerator)target;

        if (GUILayout.Button("Generate Temple"))
        {
            templeGen.GenerateTemple();
        }

        if (GUILayout.Button("Clear Temple"))
        {
            templeGen.ClearTemple();
        }
    }
}

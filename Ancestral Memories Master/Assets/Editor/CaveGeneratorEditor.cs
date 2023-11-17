// CaveGeneratorEditor.cs
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaveGenerator))]
public class CaveGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        CaveGenerator caveGen = (CaveGenerator)target;

        if (GUILayout.Button("Update Cave"))
        {
            caveGen.GenerateCave();
        }

        // Add this button for clearing the cave
        if (GUILayout.Button("Clear Cave"))
        {
            caveGen.ClearExistingCave();
        }
    }
}

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CaveGenerator))]
public class CaveGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Add a button
        CaveGenerator myScript = (CaveGenerator)target;
        if (GUILayout.Button("Update Cave"))
        {
            myScript.GenerateCave();
        }
    }
}

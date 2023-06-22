using UnityEngine;
using UnityEditor;
using ProceduralModeling;

[CustomEditor(typeof(ProceduralTree))]
public class ProceduralTreeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // This will draw the default inspector with all fields
        DrawDefaultInspector();

        ProceduralTree myScript = (ProceduralTree)target;
        if (GUILayout.Button("Generate"))
        {
            myScript.Rebuild();
        }

        if (GUILayout.Button("Clear Leaves"))
        {
            myScript.ClearLeaves();
        }
    }
}

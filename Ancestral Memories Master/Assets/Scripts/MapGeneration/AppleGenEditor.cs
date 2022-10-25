using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(GenerateApples))]
public class AppleGenEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            (target as GenerateApples).Generate();
        }

        if (GUILayout.Button("Destroy"))
        {
            (target as GenerateApples).Clear();
        }
    }
}

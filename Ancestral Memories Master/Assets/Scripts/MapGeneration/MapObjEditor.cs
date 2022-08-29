using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapObjGen))]
public class MapObjEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            (target as MapObjGen).Generate();
        }

        if (GUILayout.Button("Destroy"))
        {
            (target as MapObjGen).Clear();
        }
    }
}

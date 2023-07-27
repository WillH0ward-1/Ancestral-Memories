using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapObjGen))]
public class MapObjEditor : Editor
{
    [SerializeField] private MeshSettings mesh;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {

            (target as MapObjGen).GenerateMapObjects();
        }

        if (GUILayout.Button("Destroy"))
        {
            (target as MapObjGen).Clear();
        }
    }
}

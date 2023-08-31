#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Folder Reference")]
public class FolderReference : ScriptableObject
{
    public DefaultAsset folder;
}
#endif
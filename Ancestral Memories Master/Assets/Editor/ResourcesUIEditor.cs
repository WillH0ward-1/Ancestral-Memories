#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourcesUI))]
[CanEditMultipleObjects] // This attribute allows multi-object editing
public class ResourcesUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draws the default inspector

        // Apply the same logic to all selected ResourcesUI objects
        if (GUILayout.Button("Clear UI Elements"))
        {
            foreach (Object targetObject in targets)
            {
                ResourcesUI resourcesUI = (ResourcesUI)targetObject;
                ClearUIElements(resourcesUI);
            }
        }

        if (GUILayout.Button("Generate UI Elements"))
            {
                EditorApplication.delayCall += () =>
                {
                    foreach (Object targetObject in targets)
                    {
                        ResourcesManager resourcesManager = ((ResourcesUI)targetObject).GetComponent<ResourcesManager>();
                        if (resourcesManager != null)
                        {
                            resourcesManager.InitializeResources();
                        }
                    }
                };
            }
    }

    private void ClearUIElements(ResourcesUI resourcesUI)
    {
        // Ensure we have a valid context
        if (resourcesUI == null) return;

        // Record all destructions for undo functionality
        Undo.RecordObject(resourcesUI, "Clear UI Elements");

        // In edit mode, defer the destruction to avoid issues with OnValidate
        if (!Application.isPlaying)
        {
            // Use EditorApplication.delayCall to defer the destruction of elements
            EditorApplication.delayCall += () =>
            {
                DestroyUIElements(resourcesUI);
            };
        }
        else
        {
            // If in play mode, destroy elements immediately
            DestroyUIElements(resourcesUI);
        }
    }

    private void DestroyUIElements(ResourcesUI resourcesUI)
    {
        RectTransform[] children = resourcesUI.GetComponentsInChildren<RectTransform>(true);
        foreach (RectTransform child in children)
        {
            if (child != resourcesUI.transform) // Make sure not to delete the parent component
            {
                Undo.DestroyObjectImmediate(child.gameObject);
            }
        }
        // Clear the list of elements after destruction
        resourcesUI.ClearUIElementsList();
    }
    private void GenerateUIElements(ResourcesManager resourcesManager)
    {
        // Ensure we have a valid context
        if (resourcesManager == null) return;

        // Record this action for undo functionality
        Undo.RecordObject(resourcesManager, "Generate UI Elements");

        // Use the public InitializeResources method from ResourcesManager
        resourcesManager.InitializeResources();
    }
}
#endif

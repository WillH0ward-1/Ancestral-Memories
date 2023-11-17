using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ResourcesManager : MonoBehaviour
{
    public List<GameObject> FoodList;
    public List<GameObject> WoodList;
    public List<GameObject> PopulationList;
    public List<GameObject> StoneList;

    private Dictionary<string, List<GameObject>> resourceObjects = new Dictionary<string, List<GameObject>>();
    private ResourcesUI resourcesUI;

    private void OnEnable()
    {
        FoodList = new List<GameObject>();
        WoodList = new List<GameObject>();
        PopulationList = new List<GameObject>();
        StoneList = new List<GameObject>();

        resourceObjects["Food"] = FoodList;
        resourceObjects["Wood"] = WoodList;
        resourceObjects["Population"] = PopulationList;
        resourceObjects["Stone"] = StoneList;

        if (resourcesUI == null)
        {
            resourcesUI = GetComponent<ResourcesUI>();
            if (resourcesUI == null)
            {
                resourcesUI = gameObject.AddComponent<ResourcesUI>();
            }
        }

        InitializeResources();
    }

    public void InitializeResources()
    {
        // Assuming you want to clear and reinitialize the lists
        FoodList.Clear();
        WoodList.Clear();
        PopulationList.Clear();
        StoneList.Clear();

        UpdateUI();
    }

    public void AddResourceObject(string resourceType, GameObject resourceObject)
    {
        // Ensure the resource type is in the dictionary
        if (!resourceObjects.ContainsKey(resourceType))
        {
            resourceObjects[resourceType] = new List<GameObject>();
        }

        // Check if the object is already in the list before adding
        if (!resourceObjects[resourceType].Contains(resourceObject))
        {
            resourceObjects[resourceType].Add(resourceObject);
            UpdateUI();
        }
        else
        {
            Debug.Log($"Object already in {resourceType} list.");
        }
    }

    public void RemoveResourceObject(string resourceType, GameObject resourceObject)
    {
        // Check if the resource type exists and the object is in the list before removing
        if (resourceObjects.ContainsKey(resourceType) && resourceObjects[resourceType].Contains(resourceObject))
        {
            resourceObjects[resourceType].Remove(resourceObject);
            UpdateUI();
        }
        else
        {
            Debug.Log($"Object not found in {resourceType} list.");
        }
    }

    private void UpdateUI()
    {
        if (resourcesUI != null)
        {
            var resourceCounts = new Dictionary<string, int>();
            foreach (var pair in resourceObjects)
            {
                resourceCounts[pair.Key] = pair.Value.Count;
            }
            resourcesUI.InitializeUI(resourceCounts);
        }
    }
}

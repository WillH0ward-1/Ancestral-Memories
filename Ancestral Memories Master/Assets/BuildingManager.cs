using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    [System.Serializable]
    public class BuildingOption
    {
        public string type; // Changed from enum to string
        public Color color;
        public Sprite sprite;
        public string title;
        public GameObject buildingPrefab;
    }

    public GameObject buildMenuCanvasPrefab;
    private BuildingMenu menuPrefab;
    private BuildingMenu activeMenu;
    public List<BuildingOption> options;

    public bool menuIsActive = false;

    public void SpawnMenu()
    {
        if (!menuIsActive)
        {
            menuIsActive = true;

            GameObject canvasInstance = Instantiate(buildMenuCanvasPrefab);
            activeMenu = canvasInstance.GetComponentInChildren<BuildingMenu>();

            if (activeMenu == null)
            {
                Debug.LogError("BuildingMenu component not found on the instantiated prefab!");
                return;
            }

            // canvasInstance.transform.SetParent(someTransform, false);

            RectTransform rectTransform = canvasInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero; // Center the menu on the screen

            activeMenu.SpawnButtons(options, this);
        }
    }


    private Vector3 GetMenuPositionCenterScreen()
    {
        // Convert screen center to world point for positioning
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);
        return Camera.main.ScreenToWorldPoint(screenCenter);
    }

    public void SelectBuildingOption(string buildingType)
    {
        Debug.Log("Selected building type: " + buildingType);

        switch (buildingType)
        {
            case "Temple":
                // Logic for selecting a Temple
                break;
            case "Fire":
                // Logic for selecting Fire
                break;
            // Add more cases as needed
            default:
                Debug.LogWarning("Unknown building type selected: " + buildingType);
                break;
        }

        CloseMenu();
    }


    public void CloseMenu()
    {
        if (activeMenu != null && activeMenu.IsActive)
        {
            menuIsActive = false;
            activeMenu.CloseMenu();
        }
    }
}

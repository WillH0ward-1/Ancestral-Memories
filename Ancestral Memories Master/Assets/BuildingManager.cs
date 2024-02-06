using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BuildingManager : MonoBehaviour
{
    public InitialBuildingTypes initialBuildingsAvailable;

    [Flags]
    public enum InitialBuildingTypes
    {
        None = 0,
        Temple = 1 << 0,
        Settlement = 1 << 1,
        Campfire = 1 << 2,
        // Add more building types as needed
        All = Temple | Settlement | Campfire // Example for selecting all
    }

    public enum BuildingType
    {
        Temple,
        Settlement,
        Campfire
    }

    [System.Serializable]
    public class BuildingOption
    {
        public BuildingType type; // Changed from enum to string
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

    private CamControl camControl;
    private Camera cam;

    [SerializeField] private GameObject decal;

    private void Start()
    {
        InitializeAvailableBuildings();
    }

    private void InitializeAvailableBuildings()
    {
        // Loop through all BuildingType values
        foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
        {
            // Check if the buildingType is set in initialBuildingsAvailable
            if (initialBuildingsAvailable.HasFlag((InitialBuildingTypes)Enum.Parse(typeof(InitialBuildingTypes), buildingType.ToString())))
            {
                // If so, find the building option and add it (assuming options are predefined or loaded from somewhere)
                BuildingOption option = options.Find(o => o.type == buildingType);
                if (option != null)
                {
                    AddBuildingOption(option);
                }
                else
                {
                    // If the option is not found in the predefined list, you might need to log an error or add it differently
                    Debug.LogError($"Initial Building Option for {buildingType} not found.");
                }
            }
        }
    }


    private void Awake()
    {
        cam = Camera.main;
        camControl = cam.GetComponentInChildren<CamControl>();
    }

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

            RectTransform rectTransform = canvasInstance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero; // Center the menu on the screen

            // Filter options based on initialBuildingsAvailable
            List<BuildingOption> filteredOptions = options.FindAll(option => initialBuildingsAvailable.HasFlag((InitialBuildingTypes)Enum.Parse(typeof(InitialBuildingTypes), option.type.ToString())));
            activeMenu.SpawnButtons(filteredOptions, this);
        }
    }



    private Vector3 GetMenuPositionCenterScreen()
    {
        // Convert screen center to world point for positioning
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane);
        return Camera.main.ScreenToWorldPoint(screenCenter);
    }

    public void SelectBuildingOption(BuildingType buildingType)
    {
        Debug.Log("Selected building type: " + buildingType);

        // Find the building option based on the selected type
        BuildingOption selectedOption = options.Find(option => option.type == buildingType);
        if (selectedOption == null)
        {
            Debug.LogWarning("Unknown building type selected: " + buildingType);
            return;
        }

        StartCoroutine(camControl.BuildMode(selectedOption, selectedOption.buildingPrefab, decal));

        CloseMenu();
    }

    public void AddBuildingOption(BuildingOption newOption)
    {
        // Check if the option already exists
        if (!options.Exists(option => option.type == newOption.type))
        {
            options.Add(newOption);
        }
        else
        {
            Debug.LogWarning($"Building option for {newOption.type} already exists.");
        }
    }

    public void RemoveBuildingOption(BuildingType typeToRemove)
    {
        // Find and remove the option if it exists
        BuildingOption optionToRemove = options.Find(option => option.type == typeToRemove);
        if (optionToRemove != null)
        {
            options.Remove(optionToRemove);
        }
        else
        {
            Debug.LogWarning($"Building option for {typeToRemove} not found.");
        }
    }


    public void SelectBuildingOptionFromString(string buildingTypeString)
    {
        if (Enum.TryParse(buildingTypeString, true, out BuildingType buildingType))
        {
            SelectBuildingOption(buildingType);
        }
        else
        {
            Debug.LogWarning("Invalid building type string: " + buildingTypeString);
        }
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

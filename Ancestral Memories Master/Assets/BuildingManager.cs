using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    private CamControl camControl;
    private Camera cam;

    [SerializeField] private GameObject decal; 

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



    public void CloseMenu()
    {
        if (activeMenu != null && activeMenu.IsActive)
        {
            menuIsActive = false;
            activeMenu.CloseMenu();
        }
    }
}

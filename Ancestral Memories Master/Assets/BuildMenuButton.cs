using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuildMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Image circle;
    public Image icon;
    public string title;
    public BuildingManager.BuildingType buildingType;
    public BuildingMenu myMenu;

    Color defaultColor;

    public void OnPointerEnter(PointerEventData eventData)
    {
        myMenu.selected = this;
        myMenu.selectionText = title;
        defaultColor = circle.color;
        circle.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        myMenu.selected = null;
        myMenu.selectionText = "";
        circle.color = defaultColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        myMenu.SelectOption(buildingType);
    }
}

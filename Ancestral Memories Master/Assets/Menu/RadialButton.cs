using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class RadialButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image circle;
    public Image icon;
    public string title;
    public RadialMenu myMenu;

    Color defaultColor;

    public IEnumerator AnimateButtonIn(float buttonRevealSpeed, Vector3 targetButtonSize)
    {
        transform.localScale = Vector3.zero;
        float time = 0;

        while (time <= 1)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetButtonSize, time);
            time += Time.deltaTime / buttonRevealSpeed;
            yield return null;
        }

        transform.localScale = targetButtonSize;
        yield return null;
    }

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
}

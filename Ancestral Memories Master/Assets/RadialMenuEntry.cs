using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RadialMenuEntry : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Label;

    public void SetLabel(string pText)
    {
        Label.text = pText;
    }
}

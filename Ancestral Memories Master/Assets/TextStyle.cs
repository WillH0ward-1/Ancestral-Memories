using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextStyle : MonoBehaviour
{
    [SerializeField] private GUIStyle style;

    void Start()
    {
        style = new GUIStyle();
        style.richText = true;
    }
}

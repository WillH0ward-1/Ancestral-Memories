using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnableTitle : MonoBehaviour
{
    void Awake()
    {
        foreach(Transform text in transform.GetComponentInChildren<Transform>())
        {
            TextMeshProUGUI txtGUI = text.GetComponent<TextMeshProUGUI>();

            if (!txtGUI.enabled)
            {
                txtGUI.enabled = true;
            }
        }
    }

}

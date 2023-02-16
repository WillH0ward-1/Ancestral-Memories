using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineControl : MonoBehaviour
{
    public Outline outline;

    private void Awake()
    {
        outline = transform.GetComponent<Outline>();
        outline.enabled = false;

    }
}

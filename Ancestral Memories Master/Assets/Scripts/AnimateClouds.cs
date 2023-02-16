using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateClouds : MonoBehaviour
{
    Renderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = transform.GetComponent<Renderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float val = 1;
        val *= Time.deltaTime;
        renderer.material.SetFloat("Offset", val);
    }
}

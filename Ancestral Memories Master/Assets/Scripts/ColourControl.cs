using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourControl : MonoBehaviour
{

    private Renderer renderer;

    

    // Start is called before the first frame update

    void ChangeAttireColour()
    {
        renderer = GetComponent<Renderer>();
        renderer.material.color = Color.blue;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

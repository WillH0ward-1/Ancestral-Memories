using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadScaler : MonoBehaviour
{
    public Camera cam;

    private void LateUpdate()
    {
        transform.position = cam.transform.position + cam.transform.forward * 1;

        transform.LookAt(cam.transform);

        float quadWidth = GetComponent<Renderer>().bounds.size.x;
        float quadHeight = GetComponent<Renderer>().bounds.size.y;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float screenAspectRatio = screenWidth / screenHeight;
        float quadAspectRatio = quadWidth / quadHeight;

        float offset = 0.1f;

        float scaleFactor = (screenAspectRatio > quadAspectRatio) ? screenWidth / quadWidth : screenHeight / quadHeight;
        scaleFactor += offset;

        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
    }
    
}
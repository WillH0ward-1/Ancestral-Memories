using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoardCam : MonoBehaviour
{
    float yRotation;
    public Camera cam;

    private void Start()
    {
        yRotation = gameObject.transform.rotation.eulerAngles.y;
        yRotation += 180;
    }

    void Update()
    {
        if (cam == null)
        {
            return;
        }
        else
        {
            gameObject.transform.LookAt(cam.transform.position);
        }
    }
}

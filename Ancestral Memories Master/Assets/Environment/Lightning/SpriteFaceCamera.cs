using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour
{

    [SerializeField] private Camera cam;

    [ExecuteInEditMode]
    void LateUpdate()
    {
        transform.rotation = cam.transform.rotation;
    }
}

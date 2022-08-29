using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshPosition : MonoBehaviour
{

    [SerializeField] private float xPosition;
    [SerializeField] private float yPosition;
    [SerializeField] private float zPosition;

    [ExecuteInEditMode]
    public void SetOffset()
    {
        transform.position = new Vector3(0, 0, 0);
        Set();
    }

    void Set()
    {
        transform.position = new Vector3(xPosition, yPosition, zPosition);
    }

}

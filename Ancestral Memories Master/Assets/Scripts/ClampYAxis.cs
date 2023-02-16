using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampYAxis : MonoBehaviour
{
    public GameObject waterObject;
    public GameObject obstacleObject;

    private float xPos;
    private float zPos;
    private float yPos;

    private float yOffset = 0.1f;

    private void Awake()
    {
        float xPos = obstacleObject.transform.position.x;
        float zPos = obstacleObject.transform.position.z;
        float yPos = waterObject.transform.position.y + yOffset;

        transform.localPosition = new Vector3(xPos, yPos, zPos);
    }

    void Update()
    {
        float xPos = obstacleObject.transform.position.x;
        float zPos = obstacleObject.transform.position.z;
        float yPos = waterObject.transform.position.y;

        transform.localPosition = new Vector3(xPos, yPos, zPos);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;

    public bool lookAtTarget = false;

    public Vector3 lookAtOffset;

    public float smoothSpeed = 0.125f;

    public Camera cam;

    private void Start()
    {
        lookAtOffset = transform.position - target.transform.position;

    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, gameObject.transform.rotation.z * -1.0f);
    }

    void FixedUpdate()
    {
        if (lookAtTarget)
        {
            Vector3 destination = target.position + lookAtOffset;
            transform.position = Vector3.Slerp(transform.position, destination, smoothSpeed);
            transform.LookAt(target);
        }

    }
}

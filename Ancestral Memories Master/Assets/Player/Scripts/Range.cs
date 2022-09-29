using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Range : MonoBehaviour
{
    [SerializeField] private float distance = 5f;

    [SerializeField] private Transform target;

    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) < distance)
        {
            Debug.Log(target + "in range!");
        }
    }
}
